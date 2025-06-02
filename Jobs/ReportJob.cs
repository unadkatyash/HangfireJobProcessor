using Hangfire;
using HangfireJobProcessor.IService;
using HangfireJobProcessor.Models;

namespace HangfireJobProcessor.Jobs
{
    public class ReportJob
    {
        private readonly IReportService _reportService;
        private readonly IEmailService _emailService;
        private readonly ILogger<ReportJob> _logger;

        public ReportJob(IReportService reportService, IEmailService emailService, ILogger<ReportJob> logger)
        {
            _reportService = reportService;
            _emailService = emailService;
            _logger = logger;
        }

        [AutomaticRetry(Attempts = 2, DelaysInSeconds = new[] { 60, 300 })]
        [Queue("reports")]
        public async Task ProcessReportJob(ReportJobRequest request)
        {
            try
            {
                _logger.LogInformation("Processing report job: {ReportType}", request.ReportType);

                var reportData = await _reportService.GenerateReportAsync(request.ReportType, request.Parameters, request.OutputFormat);

                // If email is specified, send the report via email
                if (!string.IsNullOrEmpty(request.EmailTo))
                {
                    var emailBody = $"Please find attached the {request.ReportType} report generated on {DateTime.UtcNow}.";
                    await _emailService.SendEmailAsync(request.EmailTo, $"{request.ReportType} Report", emailBody);
                }

                _logger.LogInformation("Report job completed successfully: {ReportType}", request.ReportType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Report job failed: {ReportType}", request.ReportType);
                throw;
            }
        }
    }
}
