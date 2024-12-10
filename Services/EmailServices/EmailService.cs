using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;

namespace Services.EmailServices
{
    public class EmailService : IEmailSender
    {
      
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var senderEmail = "librarymanagementsystem570@gmail.com";
            var appPassword = "rrji hyun nvgm evxb";

            using (var message = new MailMessage())
            {
                message.From = new MailAddress(senderEmail);
                message.To.Add(email);
                message.Subject = subject;
                message.Body = $"<html><body>{htmlMessage}</body></html>";
                message.IsBodyHtml = true;

                using (var smtpClient = new SmtpClient("smtp.gmail.com"))
                {
                    smtpClient.Port = 587;
                    smtpClient.Credentials = new NetworkCredential(senderEmail, appPassword);
                    smtpClient.EnableSsl = true;

                    try
                    {
                        await smtpClient.SendMailAsync(message);
                        Console.WriteLine("Email sent successfullyز");
                    }
                    catch (SmtpException smtpEx)
                    {
                        Console.WriteLine($"{smtpEx.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{ex.Message}");
                    }
                }
            }
        }


    }
}
