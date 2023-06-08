using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace Common.Helper;

public static class MailHelper
{
    public static async Task<bool> SendEmail(string subject, string body, string from, string to)
    {
        try
        {
            // create email message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(from));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = body };
            //email.Body = new TextPart(TextFormat.Html) { Text = "<h1>Example HTML Message Body</h1>" };

            // send email
            using var smtp = new SmtpClient();
            //await smtp.ConnectAsync("smtp.gmail.com", 587);
            //await smtp.ConnectAsync("txpro9.fcomet.com", 465);
            await smtp.ConnectAsync("smtp.fastmail.com", 465);
            await smtp.AuthenticateAsync(@"info@pokernet.vip", @"jcf6wpntrucuj2e7");
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}