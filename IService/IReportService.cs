namespace HangfireJobProcessor.IService
{
    public interface IReportService
    {
        #region Report Generation

        /// <summary>
        /// Generates a report asynchronously based on the report type and parameters.
        /// </summary>
        /// <param name="reportType">The type or name of the report to generate.</param>
        /// <param name="parameters">A dictionary containing report parameters.</param>
        /// <param name="outputFormat">The output format of the report (default is "PDF").</param>
        /// <returns>A byte array representing the generated report file.</returns>
        Task<byte[]> GenerateReportAsync(string reportType, Dictionary<string, object> parameters, string outputFormat = "PDF");

        #endregion
    }
}
