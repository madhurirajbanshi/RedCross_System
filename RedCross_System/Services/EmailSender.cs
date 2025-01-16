using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;

namespace RedCross_System.Services
{
	public class EmailSender : IEmailSender
	{
		private readonly IConfiguration _configuration;

		public EmailSender(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public async Task SendEmailAsync(string to, string subject, string body)
		{
			// Retrieve the SendGrid API key from the configuration
			var apiKey = _configuration["SendGrid:ApiKey"];
			var client = new SendGridClient(apiKey);

			// Define the sender and recipient
			var from = new EmailAddress("noreply@redcross.org", "RedCross");
			var toEmail = new EmailAddress(to);

			// Create the email message (supporting both text and HTML body)
			var msg = MailHelper.CreateSingleEmail(from, toEmail, subject, body, body);

			try
			{
				// Send the email
				var response = await client.SendEmailAsync(msg);

				// Optionally, handle the response (e.g., log it, handle errors)
				if (response.StatusCode != System.Net.HttpStatusCode.OK)
				{
					// Handle failure, log the error
					var responseBody = await response.Body.ReadAsStringAsync();
					// You could log the response or take further actions as needed
					throw new Exception($"Error sending email: {responseBody}");
				}
			}
			catch (Exception ex)
			{
				// Log the exception (optional) and rethrow or handle accordingly
				// You might want to log this using a logging framework like Serilog or NLog
				throw new Exception("An error occurred while sending the email.", ex);
			}
		}
	}
}
