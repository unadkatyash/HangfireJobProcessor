using HangfireJobProcessor.Models;

namespace HangfireJobProcessor.IService
{
    public interface IJobService
    {
        #region Job Scheduling Methods

        /// <summary>
        /// Enqueues a background job to send an email immediately.
        /// </summary>
        /// <param name="request">Email job request details.</param>
        /// <returns>Job ID of the enqueued email job.</returns>
        string EnqueueEmailJob(EmailJobRequest request);

        /// <summary>
        /// Enqueues a background job to generate a report immediately.
        /// </summary>
        /// <param name="request">Report job request details.</param>
        /// <returns>Job ID of the enqueued report job.</returns>
        string EnqueueReportJob(ReportJobRequest request);

        /// <summary>
        /// Schedules an email job to run at a specified time.
        /// </summary>
        /// <param name="request">Email job request details.</param>
        /// <param name="scheduledAt">Date and time when the job should be executed.</param>
        /// <returns>Job ID of the scheduled email job.</returns>
        string ScheduleEmailJob(EmailJobRequest request, DateTime scheduledAt);

        /// <summary>
        /// Schedules a report generation job to run at a specified time.
        /// </summary>
        /// <param name="request">Report job request details.</param>
        /// <param name="scheduledAt">Date and time when the job should be executed.</param>
        /// <returns>Job ID of the scheduled report job.</returns>
        string ScheduleReportJob(ReportJobRequest request, DateTime scheduledAt);

        /// <summary>
        /// Sets up recurring jobs in Hangfire.
        /// </summary>
        void SetupRecurringJobs();

        #endregion
    }
}
