namespace API.Services
{
	using System;
	using System.Net;
	using System.Net.Mail;
	using System.Threading.Tasks;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.Logging;

	namespace API.Services
	{
		public class EmailService : IEmailService
		{
			private readonly IConfiguration _config;
			private readonly ILogger<EmailService> _logger;

			public EmailService(IConfiguration config, ILogger<EmailService> logger)
			{
				_config = config;
				_logger = logger;
			}

			public async Task SendPasswordResetEmailAsync(string email, string resetLink)
			{
				var subject = "Reset Your Password - Empath Dating App";
				var body = $@"
    <html>
    <body style='font-family: Arial, sans-serif; padding: 20px;'>
        <div style='max-width: 600px; margin: 0 auto; border: 1px solid #ddd; padding: 20px; border-radius: 5px;'>
            <h2 style='color: #333;'>Password Reset Request</h2>
            <p>Hello,</p>
            <p>You requested to reset your password. Click the button below to reset it:</p>
            <div style='text-align: center; margin: 30px 0;'>
                <a href='{resetLink}' style='background-color: #4CAF50; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; display: inline-block;'>Reset Password</a>
            </div>
            <p>Or copy and paste this link in your browser:</p>
            <p style='word-break: break-all; color: #666;'>{resetLink}</p>
            <p>This link will expire in {_config["PasswordReset:ExpiryHours"]} hours.</p>
            <p><strong>If you didn't request this, please ignore this email and your password will remain unchanged.</strong></p>
            <hr style='margin: 20px 0; border: none; border-top: 1px solid #ddd;'>
            <p style='color: #666; font-size: 12px;'>Best regards,<br/>Empath Dating App Team</p>
        </div>
    </body>
    </html>";

				await SendEmailAsync(email, subject, body);
			}

			public async Task SendEmailVerificationAsync(string email, string verificationLink)
			{
				var subject = "Verify Your Email";
				var body = $@"
                <html>
                <body>
                    <h2>Welcome!</h2>
                    <p>Please verify your email address by clicking the link below:</p>
                    <p><a href='{verificationLink}'>Verify Email</a></p>
                    <br/>
                    <p>Best regards,<br/>Your Dating App Team</p>
                </body>
                </html>";

				await SendEmailAsync(email, subject, body);
			}

			public async Task SendTwoFactorCodeAsync(string email, string code)
			{
				var subject = "Your Two-Factor Authentication Code";
				var body = $@"
                <html>
                <body>
                    <h2>Two-Factor Authentication</h2>
                    <p>Your verification code is:</p>
                    <h1 style='color: #4CAF50; letter-spacing: 5px;'>{code}</h1>
                    <p>This code will expire in 5 minutes.</p>
                    <p>If you didn't request this code, please secure your account immediately.</p>
                    <br/>
                    <p>Best regards,<br/>Your Dating App Team</p>
                </body>
                </html>";

				await SendEmailAsync(email, subject, body);
			}

			public async Task SendWelcomeEmailAsync(string email, string username)
			{
				var subject = "Welcome to Our Dating App!";
				var body = $@"
                <html>
                <body>
                    <h2>Welcome, {username}!</h2>
                    <p>Thank you for joining our dating community.</p>
                    <p>We're excited to have you here. Start exploring and connecting with others!</p>
                    <br/>
                    <p>Best regards,<br/>Your Dating App Team</p>
                </body>
                </html>";

				await SendEmailAsync(email, subject, body);
			}

			private async Task SendEmailAsync(string to, string subject, string body)
			{
				try
				{
					var smtpServer = _config["Email:SmtpServer"];
					var smtpPort = int.Parse(_config["Email:SmtpPort"]);
					var smtpUsername = _config["Email:Username"];
					var smtpPassword = _config["Email:Password"];
					var fromEmail = _config["Email:FromEmail"];
					var fromName = _config["Email:FromName"];

					using var message = new MailMessage
					{
						From = new MailAddress(fromEmail, fromName),
						Subject = subject,
						Body = body,
						IsBodyHtml = true
					};

					message.To.Add(new MailAddress(to));

					using var smtpClient = new SmtpClient(smtpServer, smtpPort)
					{
						EnableSsl = true,
						Credentials = new NetworkCredential(smtpUsername, smtpPassword)
					};

					await smtpClient.SendMailAsync(message);
					_logger.LogInformation($"Email sent successfully to {to}");
				}
				catch (Exception ex)
				{
					_logger.LogError($"Failed to send email to {to}: {ex.Message}");
					throw;
				}
			}
		}
	}
}
