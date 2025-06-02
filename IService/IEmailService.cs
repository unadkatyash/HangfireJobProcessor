namespace HangfireJobProcessor.IService
{
    public interface IEmailService
    {
        #region Email Methods

        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name="to">Recipient email address.</param>
        /// <param name="subject">Subject of the email.</param>
        /// <param name="body">Body content of the email.</param>
        /// <param name="cc">Optional list of CC recipients.</param>
        /// <param name="bcc">Optional list of BCC recipients.</param>
        /// <param name="isHtml">Indicates whether the body content is HTML formatted.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SendEmailAsync(string to, string subject, string body, List<string>? cc = null, List<string>? bcc = null, bool isHtml = true);

        #endregion
    }
}
