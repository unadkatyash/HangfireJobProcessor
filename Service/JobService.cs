using Hangfire;
using HangfireJobProcessor.IService;
using HangfireJobProcessor.Jobs;
using HangfireJobProcessor.Models;

namespace HangfireJobProcessor.Service
{
    /// <summary>
    /// Service responsible for queuing, scheduling, and managing Hangfire jobs.
    /// </summary>
    public class JobService : IJobService
    {
        #region [ Private Members ]

        private readonly ILogger<JobService> _logger;

        #endregion

        #region [ Constructor ]

        /// <summary>
        /// Initializes a new instance of the <see cref="JobService"/> class.
        /// </summary>
        /// <param name="logger">Logger instance.</param>
        public JobService(ILogger<JobService> logger)
        {
            _logger = logger;
        }

        #endregion

        #region [ IJobService Methods ]

        /// <summary>
        /// Enqueues an email job to be processed immediately.
        /// </summary>
        /// <param name="request">The email job request containing email details.</param>
        /// <returns>The Hangfire job ID.</returns>
        public string EnqueueEmailJob(EmailJobRequest request)
        {
            var jobId = BackgroundJob.Enqueue<EmailJob>(job => job.ProcessEmailJob(request));
            _logger.LogInformation("Email job enqueued with ID: {JobId}", jobId);
            return jobId;
        }

        /// <summary>
        /// Enqueues a report generation job to be processed immediately.
        /// </summary>
        /// <param name="request">The report job request containing report parameters.</param>
        /// <returns>The Hangfire job ID.</returns>
        public string EnqueueReportJob(ReportJobRequest request)
        {
            var jobId = BackgroundJob.Enqueue<ReportJob>(job => job.ProcessReportJob(request));
            _logger.LogInformation("Report job enqueued with ID: {JobId}", jobId);
            return jobId;
        }

        /// <summary>
        /// Schedules an email job to be processed at a future time.
        /// </summary>
        /// <param name="request">The email job request.</param>
        /// <param name="scheduledAt">The scheduled time for job execution.</param>
        /// <returns>The Hangfire job ID.</returns>
        public string ScheduleEmailJob(EmailJobRequest request, DateTime scheduledAt)
        {
            var jobId = BackgroundJob.Schedule<EmailJob>(job => job.ProcessEmailJob(request), scheduledAt);
            _logger.LogInformation("Email job scheduled with ID: {JobId} for {ScheduledAt}", jobId, scheduledAt);
            return jobId;
        }

        /// <summary>
        /// Schedules a report job to be processed at a future time.
        /// </summary>
        /// <param name="request">The report job request.</param>
        /// <param name="scheduledAt">The scheduled time for job execution.</param>
        /// <returns>The Hangfire job ID.</returns>
        public string ScheduleReportJob(ReportJobRequest request, DateTime scheduledAt)
        {
            var jobId = BackgroundJob.Schedule<ReportJob>(job => job.ProcessReportJob(request), scheduledAt);
            _logger.LogInformation("Report job scheduled with ID: {JobId} for {ScheduledAt}", jobId, scheduledAt);
            return jobId;
        }

        /// <summary>
        /// Configures recurring background jobs such as maintenance tasks.
        /// </summary>
        public void SetupRecurringJobs()
        {
            _logger.LogInformation("Setting up recurring jobs...");

            RecurringJob.AddOrUpdate<MaintenanceJob>(
                "cleanup-logs",
                job => job.CleanupLogs(),
                Cron.Daily(2, 0));

            RecurringJob.AddOrUpdate<MaintenanceJob>(
                "health-check",
                job => job.HealthCheck(),
                Cron.Minutely());

            _logger.LogInformation("Recurring jobs configured.");
        }

        #endregion
    }
}
