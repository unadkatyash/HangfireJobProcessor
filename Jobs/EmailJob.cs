using Hangfire;
using HangfireJobProcessor.IService;
using HangfireJobProcessor.Models;

namespace HangfireJobProcessor.Jobs
{
    public class EmailJob
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailJob> _logger;

        public EmailJob(IEmailService emailService, ILogger<EmailJob> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

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
    }
}
