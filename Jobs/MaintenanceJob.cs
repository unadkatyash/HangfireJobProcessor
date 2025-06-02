namespace HangfireJobProcessor.Jobs
{
    public class MaintenanceJob
    {
        private readonly ILogger<MaintenanceJob> _logger;

        public MaintenanceJob(ILogger<MaintenanceJob> logger)
        {
            _logger = logger;
        }

        public async Task CleanupLogs()
        {
            _logger.LogInformation("Starting log cleanup maintenance task");
            await Task.Delay(1000); // Simulate cleanup work
            _logger.LogInformation("Log cleanup completed");
        }

        public async Task HealthCheck()
        {
            _logger.LogInformation("Performing health check");
            await Task.Delay(100); // Simulate health check
            _logger.LogInformation("Health check completed - System is healthy");
        }
    }
}
