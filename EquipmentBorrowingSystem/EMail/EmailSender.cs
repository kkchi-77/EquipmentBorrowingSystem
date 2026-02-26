using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using Azure.Core;

namespace EquipmentBorrowingSystem.EMail
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {

            var mail = "nukddl@gmail.com"; 
            var pw = "micg frlf yqvj upvh";
            //hbyl mhsm xazn aezp(旗宏的寄信密碼)
            var smtp_email = "";
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(mail, pw)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(mail),
                Subject = subject,
                Body = message, // 使用 HTML 標籤的郵件內容
                IsBodyHtml = true // 設置為 true，指示郵件內容是 HTML 格式的
            };
            mailMessage.To.Add(email);



            return client.SendMailAsync(mailMessage);
        }

        private static string GetDomain(string email)
        {
            // 使用正則表達式匹配 @ 字元。
            Match match = Regex.Match(email, @"@(.*)");

            // 如果找到 @ 字元，則返回 @ 後面的字串。
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            else
            {
                return null;
            }
        }
    }
}