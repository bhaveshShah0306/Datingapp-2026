using API.Helpers;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// add services to the container
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddCors();
                
builder.Services.AddSignalR();



// Configure the HTTP request pipeline

var app = builder.Build();




if (app.Environment.IsDevelopment())
{	
	app.UseCors(x => x.AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()
	.SetIsOriginAllowed(_ => true)
	.WithOrigins("https://localhost:4200", "http://localhost:4200"));
}
else
{
	app.UseCors(x => x.AllowAnyHeader()
		.AllowAnyMethod()
		.AllowCredentials()
		.WithOrigins("https://localhost:4200")); 
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapHub<PresenceHub>("hubs/presence");
app.MapControllers();
//app.MapHub<MessageHub>("hubs/message");
app.MapFallbackToController("Index", "Fallback");

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    await context.Database.MigrateAsync();
    await Seed.SeedUsers(userManager, roleManager);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during migration");
}
app.UseMiddleware<ExceptionMiddleware>();
await app.RunAsync();