using Hangfire;
using HangfireJobProcessor.IService;
using HangfireJobProcessor.Models;
using Microsoft.AspNetCore.Mvc;

namespace HangfireJobProcessor.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly IJobService _jobService;
        private readonly ILogger<JobsController> _logger;

        public JobsController(IJobService jobService, ILogger<JobsController> logger)
        {
            _jobService = jobService;
            _logger = logger;
        }

        [HttpPost("email")]
        public IActionResult EnqueueEmail([FromBody] EmailJobRequest request)
        {
            try
            {
                var jobId = _jobService.EnqueueEmailJob(request);
                return Ok(new JobResponse
                {
                    JobId = jobId,
                    Status = "Enqueued",
                    Message = "Email job has been queued for processing"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to enqueue email job");
                return BadRequest(new JobResponse
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }

        [HttpPost("email/schedule")]
        public IActionResult ScheduleEmail([FromBody] EmailJobRequest request, [FromQuery] DateTime scheduledAt)
        {
            try
            {
                var jobId = _jobService.ScheduleEmailJob(request, scheduledAt);
                return Ok(new JobResponse
                {
                    JobId = jobId,
                    Status = "Scheduled",
                    Message = $"Email job scheduled for {scheduledAt}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to schedule email job");
                return BadRequest(new JobResponse
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }

        [HttpPost("report")]
        public IActionResult EnqueueReport([FromBody] ReportJobRequest request)
        {
            try
            {
                var jobId = _jobService.EnqueueReportJob(request);
                return Ok(new JobResponse
                {
                    JobId = jobId,
                    Status = "Enqueued",
                    Message = "Report job has been queued for processing"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to enqueue report job");
                return BadRequest(new JobResponse
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }

        [HttpPost("report/schedule")]
        public IActionResult ScheduleReport([FromBody] ReportJobRequest request, [FromQuery] DateTime scheduledAt)
        {
            try
            {
                var jobId = _jobService.ScheduleReportJob(request, scheduledAt);
                return Ok(new JobResponse
                {
                    JobId = jobId,
                    Status = "Scheduled",
                    Message = $"Report job scheduled for {scheduledAt}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to schedule report job");
                return BadRequest(new JobResponse
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }

        [HttpGet("status/{jobId}")]
        public IActionResult GetJobStatus(string jobId)
        {
            try
            {
                var jobDetails = JobStorage.Current.GetConnection().GetJobData(jobId);
                if (jobDetails == null)
                {
                    return NotFound(new JobResponse
                    {
                        JobId = jobId,
                        Status = "NotFound",
                        Message = "Job not found"
                    });
                }

                return Ok(new JobResponse
                {
                    JobId = jobId,
                    Status = jobDetails.State,
                    Message = $"Job is currently {jobDetails.State}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get job status for {JobId}", jobId);
                return BadRequest(new JobResponse
                {
                    JobId = jobId,
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }

        [HttpDelete("{jobId}")]
        public IActionResult CancelJob(string jobId)
        {
            try
            {
                var result = BackgroundJob.Delete(jobId);
                if (result)
                {
                    return Ok(new JobResponse
                    {
                        JobId = jobId,
                        Status = "Cancelled",
                        Message = "Job has been cancelled"
                    });
                }
                else
                {
                    return BadRequest(new JobResponse
                    {
                        JobId = jobId,
                        Status = "Error",
                        Message = "Failed to cancel job"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cancel job {JobId}", jobId);
                return BadRequest(new JobResponse
                {
                    JobId = jobId,
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }

        [HttpGet("hangfire-dashboard")]
        [ProducesResponseType(StatusCodes.Status302Found)]
        public IActionResult GetHangfireDashboardRedirect()
        {
            return Redirect("/hangfire");
        }
    }
}
