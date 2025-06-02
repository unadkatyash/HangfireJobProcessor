namespace HangfireJobProcessor.Models
{
    public class JobRequest
    {
        public string JobType { get; set; } = string.Empty;
        public Dictionary<string, object> Parameters { get; set; } = new();
        public string? Queue { get; set; } = "default";
        public DateTime? ScheduledAt { get; set; }
        public int Priority { get; set; } = 0;
    }

    public class EmailJobRequest
    {
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public List<string>? Cc { get; set; }
        public List<string>? Bcc { get; set; }
        public bool IsHtml { get; set; } = true;
    }

    public class ReportJobRequest
    {
        public string ReportType { get; set; } = string.Empty;
        public Dictionary<string, object> Parameters { get; set; } = new();
        public string OutputFormat { get; set; } = "PDF";
        public string? EmailTo { get; set; }
    }

    public class JobResponse
    {
        public string JobId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
