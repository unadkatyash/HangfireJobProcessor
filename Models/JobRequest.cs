namespace HangfireJobProcessor.Models
{
    #region Job Request Models

    /// <summary>
    /// Represents a general job request with parameters for enqueuing or scheduling.
    /// </summary>
    public class JobRequest
    {
        /// <summary>
        /// Gets or sets the type of the job (e.g. email, report).
        /// </summary>
        public string JobType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the parameters for the job.
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; } = new();

        /// <summary>
        /// Gets or sets the queue name where the job should be enqueued.
        /// Default is "default".
        /// </summary>
        public string? Queue { get; set; } = "default";

        /// <summary>
        /// Gets or sets the scheduled time for the job. Null means immediate enqueue.
        /// </summary>
        public DateTime? ScheduledAt { get; set; }

        /// <summary>
        /// Gets or sets the priority of the job. Default is 0.
        /// </summary>
        public int Priority { get; set; } = 0;
    }

    /// <summary>
    /// Represents a request to enqueue or schedule an email job.
    /// </summary>
    public class EmailJobRequest
    {
        /// <summary>
        /// Gets or sets the recipient email address.
        /// </summary>
        public string To { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the email subject.
        /// </summary>
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the email body content.
        /// </summary>
        public string Body { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets optional CC recipients.
        /// </summary>
        public List<string>? Cc { get; set; }

        /// <summary>
        /// Gets or sets optional BCC recipients.
        /// </summary>
        public List<string>? Bcc { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the email body is HTML. Default is true.
        /// </summary>
        public bool IsHtml { get; set; } = true;
    }

    /// <summary>
    /// Represents a request to enqueue or schedule a report generation job.
    /// </summary>
    public class ReportJobRequest
    {
        /// <summary>
        /// Gets or sets the type of the report to generate.
        /// </summary>
        public string ReportType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets parameters for report generation.
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; } = new();

        /// <summary>
        /// Gets or sets the output format of the report. Default is "PDF".
        /// </summary>
        public string OutputFormat { get; set; } = "PDF";

        /// <summary>
        /// Gets or sets the optional recipient email address to send the report to.
        /// </summary>
        public string? EmailTo { get; set; }
    }

    #endregion

    #region Job Response Model

    /// <summary>
    /// Represents the response returned after a job is enqueued, scheduled, or queried.
    /// </summary>
    public class JobResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier of the job.
        /// </summary>
        public string JobId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the status of the job (e.g. Enqueued, Scheduled, Completed).
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets any message related to the job status or result.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the UTC date and time when the job was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    #endregion
}
