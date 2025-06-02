using HangfireJobProcessor.IService;
using System.Net;
using System.Net.Mail;

namespace HangfireJobProcessor.Service
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _configuration;

        public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string body, List<string>? cc = null, List<string>? bcc = null, bool isHtml = true)
        {
            try
            {
                _logger.LogInformation("Starting email send to {To} with subject {Subject}", to, subject);

                // Simulate email sending delay
                await Task.Delay(2000);

                // In a real implementation, you would use SendGrid, AWS SES, or SMTP
                // Example with SMTP:

                using var client = new SmtpClient(_configuration["Email:SmtpHost"], int.Parse(_configuration["Email:SmtpPort"]));
                client.Credentials = new NetworkCredential(_configuration["Email:Username"], _configuration["Email:Password"]);
                client.EnableSsl = true;

                var message = new MailMessage
                {
                    From = new MailAddress(_configuration["Email:FromAddress"]),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };

                message.To.Add(to);
                if (cc != null && cc.Count > 0) cc.ForEach(c => message.CC.Add(c));
                if (bcc != null && bcc.Count > 0) bcc.ForEach(b => message.Bcc.Add(b));

                await client.SendMailAsync(message);


                _logger.LogInformation("Email sent successfully to {To}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}", to);
                throw;
            }
        }
    }
}
