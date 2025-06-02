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
        #region Services & Constructor

        private readonly IJobService _jobService;
        private readonly ILogger<JobsController> _logger;

        public JobsController(IJobService jobService, ILogger<JobsController> logger)
        {
            _jobService = jobService;
            _logger = logger;
        }

        #endregion

        #region Job Endpoints

        /// <summary>
        /// Enqueues an email job to be processed immediately.
        /// </summary>
        /// <param name="request">Email job request containing email details.</param>
        /// <returns>Returns job ID and status message.</returns>
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

        /// <summary>
        /// Schedules an email job for future execution.
        /// </summary>
        /// <param name="request">Email job request containing email details.</param>
        /// <param name="scheduledAt">Date and time to schedule the job.</param>
        /// <returns>Returns job ID and scheduling status.</returns>
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

        /// <summary>
        /// Enqueues a report generation job to be processed immediately.
        /// </summary>
        /// <param name="request">Report job request containing report parameters.</param>
        /// <returns>Returns job ID and status message.</returns>
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

        /// <summary>
        /// Schedules a report generation job for future execution.
        /// </summary>
        /// <param name="request">Report job request containing report parameters.</param>
        /// <param name="scheduledAt">Date and time to schedule the job.</param>
        /// <returns>Returns job ID and scheduling status.</returns>
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

        /// <summary>
        /// Gets the status of a specific job by job ID.
        /// </summary>
        /// <param name="jobId">The ID of the job to check.</param>
        /// <returns>Returns job status and state details.</returns>
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

        /// <summary>
        /// Cancels a job by deleting it from the Hangfire queue.
        /// </summary>
        /// <param name="jobId">The ID of the job to cancel.</param>
        /// <returns>Returns job cancellation status.</returns>
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

        #endregion
    }
}

