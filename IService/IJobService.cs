using HangfireJobProcessor.Models;

namespace HangfireJobProcessor.IService
{
    public interface IJobService
    {
        string EnqueueEmailJob(EmailJobRequest request);
        string EnqueueReportJob(ReportJobRequest request);
        string ScheduleEmailJob(EmailJobRequest request, DateTime scheduledAt);
        string ScheduleReportJob(ReportJobRequest request, DateTime scheduledAt);
        void SetupRecurringJobs();
    }
}
