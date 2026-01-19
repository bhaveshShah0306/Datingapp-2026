using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace API.Controllers;
[AllowAnonymous]
public class AccountController : BaseApiController
{
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly ILogger<AccountController> _logger;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _config;
	private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

	public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, IMapper mapper, ILogger<AccountController> logger, IEmailService emailService, IConfiguration config)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _mapper = mapper;
        _tokenService = tokenService;
        _logger = logger;
        _emailService = emailService;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");

        var user = _mapper.Map<AppUser>(registerDto);

        user.UserName = registerDto.Username.ToLower();

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded) return BadRequest(result.Errors);

        var roleResult = await _userManager.AddToRoleAsync(user, "Member");

        if (!roleResult.Succeeded) return BadRequest(result.Errors);

        return new UserDto
        {
            Username = user.UserName,
            Token = await _tokenService.CreateToken(user),
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await _userManager.Users
            //.Include(subject)
                        .SingleOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());

        if (user == null) return Unauthorized("Invalid username");
    

		var result = await _signInManager
            .CheckPasswordSignInAsync(user, loginDto.Password, false);

        if (!result.Succeeded) return Unauthorized();

		return new UserDto
        {
            Username = user.UserName,
            Token = await _tokenService.CreateToken(user),
            //PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };
    }
	[HttpPost("forgot-password")]
	public async Task<ActionResult<PasswordResetResponseDto>> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
	{
		var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);

		// Always return success to prevent email enumeration attacks
		if (user == null)
		{
			return Ok(new PasswordResetResponseDto
			{
				Success = true, 
				Message = "If an account exists with that email, a password reset link has been sent."
			});
		}

		// Generate password reset token
		var token = await _userManager.GeneratePasswordResetTokenAsync(user);

		// Create reset link (you'll need to update this with your actual frontend URL)
		var frontendUrl = _config["Frontend:Url"] ?? "http://localhost:4200";
		var encodedToken = HttpUtility.UrlEncode(token);
		var encodedEmail = HttpUtility.UrlEncode(forgotPasswordDto.Email);
		var resetLink = $"{frontendUrl}/reset-password?token={encodedToken}&email={encodedEmail}";

		try
		{
			await _emailService.SendPasswordResetEmailAsync(forgotPasswordDto.Email, resetLink);
		}
		catch (Exception ex)
		{
			// Log the error but don't expose it to the user
			Console.WriteLine($"Failed to send password reset email: {ex.Message}");
			return StatusCode(500, new PasswordResetResponseDto
			{
				Success = false,
				Message = "An error occurred while sending the email. Please try again later."
			});
		}

		return Ok(new PasswordResetResponseDto
		{
			Success = true,
			Message = "If an account exists with that email, a password reset link has been sent."
		});
	}


	private async Task<bool> UserExists(string username)
    {
        return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
    }
}