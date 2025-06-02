using Hangfire;
using HangfireJobProcessor.IService;
using HangfireJobProcessor.Models;

namespace HangfireJobProcessor.Jobs
{
    public class EmailJob
    {
        #region Fields and Constructor

        private readonly IEmailService _emailService;
        private readonly ILogger<EmailJob> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailJob"/> class.
        /// </summary>
        /// <param name="emailService">The email service to send emails.</param>
        /// <param name="logger">The logger instance.</param>
        public EmailJob(IEmailService emailService, ILogger<EmailJob> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        #endregion

        #region Job Methods

        /// <summary>
        /// Processes the email job by sending an email asynchronously.
        /// Includes automatic retry on failure with specified delays.
        /// </summary>
        /// <param name="request">The email job request containing email details.</param>
        [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 30, 60, 120 })]
        [Queue("emails")]
        public async Task ProcessEmailJob(EmailJobRequest request)
        {
            try
            {
                _logger.LogInformation("Processing email job for {To}", request.To);
                await _emailService.SendEmailAsync(request.To, request.Subject, request.Body, request.Cc, request.Bcc, request.IsHtml);
                _logger.LogInformation("Email job completed successfully for {To}", request.To);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email job failed for {To}", request.To);
                throw;
            }
        }

        #endregion
    }
}