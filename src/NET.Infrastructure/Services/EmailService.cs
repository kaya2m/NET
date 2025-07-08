using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NET.Application.Common.Interfaces;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace NET.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            await SendEmailAsync(new[] { to }, subject, body);
        }

        public async Task SendEmailAsync(string[] to, string subject, string body)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("SmtpSettings");
                var smtpServer = smtpSettings["Server"];
                var smtpPort = int.Parse(smtpSettings["Port"] ?? "587");
                var smtpUsername = smtpSettings["Username"];
                var smtpPassword = smtpSettings["Password"];
                var fromEmail = smtpSettings["FromEmail"];
                var fromName = smtpSettings["FromName"];

                using var client = new SmtpClient(smtpServer, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                foreach (var email in to)
                {
                    mailMessage.To.Add(email);
                }

                await client.SendMailAsync(mailMessage);

                _logger.LogInformation("Email sent successfully to {Recipients}", string.Join(", ", to));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Recipients}", string.Join(", ", to));
                throw;
            }
        }

        public async Task SendTemplateEmailAsync(string to, string templateName, object model)
        {
            // Template-based email implementation
            // This would typically use a template engine like Razor or HandleBars
            var subject = GetTemplateSubject(templateName);
            var body = await RenderTemplateAsync(templateName, model);

            await SendEmailAsync(to, subject, body);
        }

        public async Task SendWelcomeEmailAsync(string to, string userName, string temporaryPassword)
        {
            var subject = "Welcome to Building Management System";
            var body = $@"
                <h2>Welcome to Building Management System</h2>
                <p>Hello {userName},</p>
                <p>Your account has been created successfully.</p>
                <p><strong>Temporary Password:</strong> {temporaryPassword}</p>
                <p>Please log in and change your password immediately.</p>
                <p>Best regards,<br/>Building Management Team</p>
            ";

            await SendEmailAsync(to, subject, body);
        }

        public async Task SendPasswordResetEmailAsync(string to, string resetToken)
        {
            var subject = "Password Reset Request";
            var resetUrl = $"{_configuration["AppUrl"]}/reset-password?token={resetToken}";
            var body = $@"
                <h2>Password Reset Request</h2>
                <p>You have requested to reset your password.</p>
                <p>Click the link below to reset your password:</p>
                <p><a href=""{resetUrl}"">Reset Password</a></p>
                <p>If you didn't request this, please ignore this email.</p>
                <p>Best regards,<br/>Building Management Team</p>
            ";

            await SendEmailAsync(to, subject, body);
        }

        public async Task SendInvoiceEmailAsync(string to, string invoiceNumber, byte[] invoicePdf)
        {
            try
            {
                var subject = $"Invoice {invoiceNumber}";
                var body = $@"
                    <h2>Invoice {invoiceNumber}</h2>
                    <p>Please find your invoice attached.</p>
                    <p>Payment is due as indicated on the invoice.</p>
                    <p>Best regards,<br/>Building Management Team</p>
                ";

                var smtpSettings = _configuration.GetSection("SmtpSettings");
                var smtpServer = smtpSettings["Server"];
                var smtpPort = int.Parse(smtpSettings["Port"] ?? "587");
                var smtpUsername = smtpSettings["Username"];
                var smtpPassword = smtpSettings["Password"];
                var fromEmail = smtpSettings["FromEmail"];
                var fromName = smtpSettings["FromName"];

                using var client = new SmtpClient(smtpServer, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(to);

                // Add PDF attachment
                if (invoicePdf != null && invoicePdf.Length > 0)
                {
                    var attachment = new Attachment(new System.IO.MemoryStream(invoicePdf), $"Invoice_{invoiceNumber}.pdf", "application/pdf");
                    mailMessage.Attachments.Add(attachment);
                }

                await client.SendMailAsync(mailMessage);

                _logger.LogInformation("Invoice email sent successfully to {Email}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send invoice email to {Email}", to);
                throw;
            }
        }

        public async Task SendMaintenanceNotificationEmailAsync(string to, string requestTitle, string buildingName)
        {
            var subject = "Maintenance Request Update";
            var body = $@"
                <h2>Maintenance Request Update</h2>
                <p>There has been an update to your maintenance request:</p>
                <p><strong>Request:</strong> {requestTitle}</p>
                <p><strong>Building:</strong> {buildingName}</p>
                <p>Please log in to your account to view the details.</p>
                <p>Best regards,<br/>Building Management Team</p>
            ";

            await SendEmailAsync(to, subject, body);
        }

        private string GetTemplateSubject(string templateName)
        {
            // Return appropriate subject based on template name
            return templateName switch
            {
                "welcome" => "Welcome to Building Management System",
                "password-reset" => "Password Reset Request",
                "invoice" => "New Invoice",
                "maintenance" => "Maintenance Update",
                _ => "Building Management System Notification"
            };
        }

        private async Task<string> RenderTemplateAsync(string templateName, object model)
        {
            // This is a simple implementation
            // In a real application, you would use a proper template engine
            await Task.CompletedTask;
            return $"<p>Template: {templateName}</p><p>Model: {model}</p>";
        }
    }
}