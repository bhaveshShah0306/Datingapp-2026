namespace API.Interfaces
{
	public interface IEmailService
	{
		Task SendPasswordResetEmailAsync(string email, string resetLink);
		Task SendEmailVerificationAsync(string email, string verificationLink);
		Task SendTwoFactorCodeAsync(string email, string code);
		Task SendWelcomeEmailAsync(string email, string username);
	}
}
