namespace NET.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendEmailAsync(string[] to, string subject, string body);
        Task SendTemplateEmailAsync(string to, string templateName, object model);
        Task SendWelcomeEmailAsync(string to, string userName, string temporaryPassword);
        Task SendPasswordResetEmailAsync(string to, string resetToken);
        Task SendInvoiceEmailAsync(string to, string invoiceNumber, byte[] invoicePdf);
        Task SendMaintenanceNotificationEmailAsync(string to, string requestTitle, string buildingName);
    }
}