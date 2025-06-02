using Hangfire;
using HangfireJobProcessor.IService;
using HangfireJobProcessor.Models;

namespace HangfireJobProcessor.Jobs
{
    public class ReportJob
    {
        #region Fields and Constructor

        private readonly IReportService _reportService;
        private readonly IEmailService _emailService;
        private readonly ILogger<ReportJob> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportJob"/> class.
        /// </summary>
        /// <param name="reportService">The report generation service.</param>
        /// <param name="emailService">The email sending service.</param>
        /// <param name="logger">The logger instance.</param>
        public ReportJob(IReportService reportService, IEmailService emailService, ILogger<ReportJob> logger)
        {
            _reportService = reportService;
            _emailService = emailService;
            _logger = logger;
        }

        #endregion

        #region Job Methods

        /// <summary>
        /// Processes a report job by generating the report and optionally sending it via email.
        /// </summary>
        /// <param name="request">The report job request containing report details and parameters.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        [AutomaticRetry(Attempts = 2, DelaysInSeconds = new[] { 60, 300 })]
        [Queue("reports")]
        public async Task ProcessReportJob(ReportJobRequest request)
        {
            try
            {
                _logger.LogInformation("Processing report job: {ReportType}", request.ReportType);

                var reportData = await _reportService.GenerateReportAsync(request.ReportType, request.Parameters, request.OutputFormat);

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

        #endregion
    }
}
