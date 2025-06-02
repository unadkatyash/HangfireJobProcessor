using Hangfire;
using HangfireJobProcessor.IService;
using HangfireJobProcessor.Jobs;
using HangfireJobProcessor.Models;

namespace HangfireJobProcessor.Service
{
    /// <summary>
    /// 
    /// </summary>
    public class JobService : IJobService
    {
        #region [ Private Members ]

        private readonly ILogger<JobService> _logger;

        #endregion

        #region [ Constructor ]

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        public JobService(ILogger<JobService> logger)
        {
            _logger = logger;
        }

        #endregion

        #region [ IJobService Methods ]

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string EnqueueEmailJob(EmailJobRequest request)
        {
            var jobId = BackgroundJob.Enqueue<EmailJob>(job => job.ProcessEmailJob(request));
            _logger.LogInformation("Email job enqueued with ID: {JobId}", jobId);
            return jobId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string EnqueueReportJob(ReportJobRequest request)
        {
            var jobId = BackgroundJob.Enqueue<ReportJob>(job => job.ProcessReportJob(request));
            _logger.LogInformation("Report job enqueued with ID: {JobId}", jobId);
            return jobId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="scheduledAt"></param>
        /// <returns></returns>
        public string ScheduleEmailJob(EmailJobRequest request, DateTime scheduledAt)
        {
            var jobId = BackgroundJob.Schedule<EmailJob>(job => job.ProcessEmailJob(request), scheduledAt);
            _logger.LogInformation("Email job scheduled with ID: {JobId} for {ScheduledAt}", jobId, scheduledAt);
            return jobId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="scheduledAt"></param>
        /// <returns></returns>
        public string ScheduleReportJob(ReportJobRequest request, DateTime scheduledAt)
        {
            var jobId = BackgroundJob.Schedule<ReportJob>(job => job.ProcessReportJob(request), scheduledAt);
            _logger.LogInformation("Report job scheduled with ID: {JobId} for {ScheduledAt}", jobId, scheduledAt);
            return jobId;
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetupRecurringJobs()
        {
            // Setup recurring jobs
            RecurringJob.AddOrUpdate<MaintenanceJob>("cleanup-logs", job => job.CleanupLogs(), Cron.Daily(2, 0));
            RecurringJob.AddOrUpdate<MaintenanceJob>("health-check", job => job.HealthCheck(), Cron.Minutely());
        }

        #endregion
    }
}
