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
            KnownAs = user.KnownAs,
			Token = await _tokenService.CreateToken(user),
            Gender = user.Gender
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await _userManager.Users
            //.Include(subject)
                        .SingleOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower()||
												   x.Email == loginDto.Username.ToLower());

        if (user == null) return Unauthorized("Invalid username");
    

		var result = await _signInManager
            .CheckPasswordSignInAsync(user, loginDto.Password, false);

        if (!result.Succeeded) return Unauthorized();

		return new UserDto
        {
            Username = user.UserName,            
            //PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
			Token = await _tokenService.CreateToken(user),
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

		// Create reset link
		var frontendUrl = _config["Frontend:Url"] ?? "http://localhost:4200";
		var encodedToken = HttpUtility.UrlEncode(token);
		var encodedEmail = HttpUtility.UrlEncode(forgotPasswordDto.Email);
		var resetLink = $"{frontendUrl}/reset-password?token={encodedToken}&email={encodedEmail}";

		_logger.LogInformation($"Password reset requested for {forgotPasswordDto.Email}");

		try
		{
			await _emailService.SendPasswordResetEmailAsync(forgotPasswordDto.Email, resetLink);
			_logger.LogInformation($"Password reset email sent successfully to {forgotPasswordDto.Email}");
		}
		catch (Exception ex)
		{
			_logger.LogError($"Failed to send password reset email to {forgotPasswordDto.Email}: {ex.Message}");
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
	[HttpPost("reset-password")]
	public async Task<ActionResult<PasswordResetResponseDto>> ResetPassword(ResetPasswordDto resetPasswordDto)
	{
		var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);

		if (user == null)
		{
			return BadRequest(new PasswordResetResponseDto
			{
				Success = false,
				Message = "Invalid password reset request."
			});
		}

		// Verify the token
		var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);

		if (!result.Succeeded)
		{
			var errors = string.Join(", ", result.Errors.Select(e => e.Description));
			return BadRequest(new PasswordResetResponseDto
			{
				Success = false,
				Message = $"Password reset failed: {errors}"
			});
		}

		// Optional: Send confirmation email
		try
		{
			await _emailService.SendWelcomeEmailAsync(user.Email, user.UserName);
		}
		catch (Exception ex)
		{
			_logger.LogError($"Failed to send confirmation email: {ex.Message}");
			// Don't fail the reset if email fails
		}

		return Ok(new PasswordResetResponseDto
		{
			Success = true,
			Message = "Your password has been reset successfully."
		});
	}

	private async Task<bool> UserExists(string username)
    {
        return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
    }
}