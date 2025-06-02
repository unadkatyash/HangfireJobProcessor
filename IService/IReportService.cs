namespace HangfireJobProcessor.IService
{
    public interface IReportService
    {
        Task<byte[]> GenerateReportAsync(string reportType, Dictionary<string, object> parameters, string outputFormat = "PDF");
    }
}
