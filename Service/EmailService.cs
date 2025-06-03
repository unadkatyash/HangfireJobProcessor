using HangfireJobProcessor.IService;
using System.Net;
using System.Net.Mail;

namespace HangfireJobProcessor.Service
{
    /// <summary>
    /// Service for sending emails using SMTP.
    /// </summary>
    public class EmailService : IEmailService
    {
        #region Fields

        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _configuration;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailService"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="configuration">The configuration instance for email settings.</param>
        public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sends an email asynchronously with optional CC, BCC, and HTML formatting.
        /// </summary>
        /// <param name="to">Recipient email address.</param>
        /// <param name="subject">Subject of the email.</param>
        /// <param name="body">Body content of the email.</param>
        /// <param name="cc">Optional list of CC recipients.</param>
        /// <param name="bcc">Optional list of BCC recipients.</param>
        /// <param name="isHtml">Flag indicating if the email body is HTML formatted. Default is true.</param>
        public async Task SendEmailAsync(string to, string subject, string body, List<string>? cc = null, List<string>? bcc = null, bool isHtml = true)
        {
            try
            {
                _logger.LogInformation("Starting email send to {To} with subject {Subject}", to, subject);

                await Task.Delay(2000); // Simulate sending delay

                using var client = new SmtpClient(_configuration["Email:SmtpHost"], int.Parse(_configuration["Email:SmtpPort"]))
                {
                    Credentials = new NetworkCredential(_configuration["Email:Username"], _configuration["Email:Password"]),
                    EnableSsl = true
                };

                var message = new MailMessage
                {
                    From = new MailAddress(_configuration["Email:FromAddress"]),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };

                message.To.Add(to);

                cc?.ForEach(c => message.CC.Add(c));
                bcc?.ForEach(b => message.Bcc.Add(b));

                await client.SendMailAsync(message);

                _logger.LogInformation("Email sent successfully to {To}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}", to);
                throw;
            }
        }

        #endregion
    }
}

