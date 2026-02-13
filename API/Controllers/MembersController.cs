namespace API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MembersController(DataContext context) : ControllerBase
	{
		[HttpGet]
		public async Task<ActionResult<IReadOnlyList<AppUser>>> GetMembers()
		{
			var members = await context.Users.ToListAsync();
			return Ok(members);
		}
		[HttpGet("{id}")]
		public async Task<ActionResult<AppUser>> GetMember(int id)
		{
			var member = await context.Users.SingleOrDefaultAsync(x => x.Id == id);

			if (member == null)
				return NotFound();

			return Ok(member);
		}
	}
}
