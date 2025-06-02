namespace HangfireJobProcessor.IService
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body, List<string>? cc = null, List<string>? bcc = null, bool isHtml = true);
    }
}
