using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using Scaffoldong.Data;
using Scaffoldong.EMail;
using Scaffoldong.Models;
using System.Diagnostics;
using System.Net.Mail;
using System.Text;
using System.Net;
using System.Net.Mail;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using static System.Net.Mime.MediaTypeNames;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Drawing;
using System.Runtime.InteropServices;
namespace Scaffoldong.Controllers
{
    public class SendEmail_OverdueNotificationController : Controller
    {
        private readonly EmployeeContext _context;
        private readonly IEmailSender emailSender;

        public SendEmail_OverdueNotificationController(IEmailSender emailSender, EmployeeContext context)
        {
            _context = context;
            this.emailSender = emailSender;
        }


        public async Task<IActionResult> SendEmail_OverdueNotification(string email, string fOrderGuid)
        {
            var applicationCompleted = _context.Application_Completed
                .FirstOrDefault(m => m.fOrderGuid == fOrderGuid);

            var applicationDetails_NoConsumable = _context.Application_Details
                .Where(m => m.fOrderGuid == fOrderGuid && m.Is_Consumable == "False")
                .ToList();

            var applicationDetails_YesConsumable = _context.Application_Details
            .Where(m => m.fOrderGuid == fOrderGuid && m.Is_Consumable == "True")
            .ToList();
            if (applicationCompleted == null )
            {
                if( applicationDetails_YesConsumable.Count == 0 && applicationDetails_NoConsumable.Count == 0)
                {
                    // 如果找不到相關的申請或設備，則返回 NotFound 或其他相應的結果
                    return NotFound();
                }
         
            }

            var counter = 1;
            var equipment_quaintity = 0;

            foreach (var item in applicationDetails_NoConsumable)
            {
                equipment_quaintity++;
            }
            foreach (var item in applicationDetails_YesConsumable)
            {
                equipment_quaintity += int.Parse(item.Consumable_Borrowing_Times);
            }

            var borrowedEquipmentList = string.Join("", applicationDetails_NoConsumable.Select(item =>
            $@"
            {counter++}、<br>
            設備名稱：{item.EName}<br>
            設備型號：{item.Emodel}<br>
            <div style='display: flex; justify-content: space-between;'>
                <span>設備編號：{item.EId}</span>
                <span style='margin-left: auto;'>數量：{item.Consumable_Borrowing_Times}</span>
            </div>
            <hr/>"
             ));




            borrowedEquipmentList += string.Join("", applicationDetails_YesConsumable.Select(item =>
                $@"
            {counter++}、<br>
            設備名稱：{item.EName}<br>
            <div style='display: flex; justify-content: space-between;'>
               <span>設備型號：{item.Emodel}</span>
                <span style='margin-left: auto;'>數量：{item.Consumable_Borrowing_Times}</span>
            </div>
            <hr/>
            "
            ));
            var companyLogoUrl = "https://www.ddc.tw/uploads/1/4/0/3/140336294/logo-03.png"; // 替換為您公司的標誌 URL
            //var companyLogoUrl = "https://lh3.googleusercontent.com/pw/AP1GczNG0CqddkwOYS7wN1RqUIJwRsrEPCgqXyWkHefNQTNqtFFeHp99vtQLWkJa8V6VDjV3gopjLuLzW4NVyY_HzR1ouE2dFxrqSjaPSUhduRL4mhPCphAuYZig5mJEGd9PAhu8lo_jkInvxaiXa9qPoZjC=w356-h77-s-no-gm?authuser=0"; // 替換為您公司的標誌 URL
            var companyLogoHtml = $"<img src=\"{companyLogoUrl}\" alt=\"Company Logo\" style=\"max-width: 100%; height: auto;\">";

            var subject = "設備逾期歸還通知";
            var message = "";
            if (applicationCompleted.Date_Of_Application == new DateTime(2000, 10, 10))
            {
                message = $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>設備借用驗證碼</title>
    <style>
html,body {{ padding: 0; margin:0}}
        /* 添加您喜歡的 CSS 樣式 */
        body {{
            font-family: Arial, sans-serif;
            line-height: 1.6;
            background-color: #ffffff; /* 背景色改為白色 */
            padding: 20px;
        }}
        .container {{
            max-width: 600px;
            margin: auto;
            background: #fff;
            padding: 20px;
            border-radius: 5px;
            box-shadow: 0 0 10px rgba(0,0,0,0.1);
        }}
        .logo {{
            text-align: center;
            margin-bottom: 20px;
        }}
        .message {{
           color: #000;
        }}

    </style>
  <link href=""https://fonts.googleapis.com/css2?family=Noto+Sans+TC:wght@100;200;300;400;500;600;700;800;900&display=swap"" rel=""stylesheet"">
</head>
<body>
     <div style=""font-family:Noto Sans TC,Arial,Helvetica,sans-serif; line-height: 1.5; font-weight: normal; font-size: 15px; color: #2F3044; min-height: 100%; margin:0; padding:0; width:100%; background-color:#edf2f7"">
            <br>
            <table align=""center"" cellpadding=""0"" cellspacing=""0"" style=""border-collapse:collapse;margin:0 auto; padding:0; max-width:600px; width: 100%; border: 0"">
                <tbody>
                    <tr>
                        <td align=""center"" style=""text-align:center; padding: 40px; vertical-align: center"">
                                <img alt=""Logo"" style=""max-height: 100px"" src=""  {companyLogoUrl}"">
              
                        </td>
                    </tr>
                    <tr>
                        <td align=""left"" style=""vertical-align: center"">
                            <div style=""text-align:left; margin: 0 20px; padding: 40px; background-color:#ffffff; border-radius: 6px"">
                            <div class=""message"">
                                  <p>親愛的使用者，</p>
                                  <p>您向數位內容設計研究中心申請了設備的借用。</p>
                                  <p>借用日期：{applicationCompleted.Borrow_Time}</p>
                                  <p>歸還日期：{applicationCompleted.Return_Time}</p>
                                  <p>您的借用日期已到期，提醒您請將設備歸還至數位內容設計研究中心！</p>
                                  <p>您借用的設備有(共{equipment_quaintity}項)：</p>
                                  <p>{borrowedEquipmentList}</p>
                            </div>
                                <div style=""padding-bottom: 12px; font-weight: bold;"">
                                    數位內容設計研究中心
                                </div>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style=""font-size: 13px; text-align:center; vertical-align: center; padding: 20px; color: #6d6e7c;"">
                            <p>
                                Copyright ©{@DateTime.Now.Year}
                                <a href=""https://www.ddc.tw/"" rel=""noopener"" target=""_blank"">數位內容設計研究中心</a>
                                &
                                <a href=""https://www.eduverse.tw"" rel=""noopener"" target=""_blank"">EduVerse教育元宇宙</a>
                            </p>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
</body>
</html>";
            }
            else
            {
                message = $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>設備借用驗證碼</title>
    <style>
html,body {{ padding: 0; margin:0}}
        /* 添加您喜歡的 CSS 樣式 */
        body {{
            font-family: Arial, sans-serif;
            line-height: 1.6;
            background-color: #ffffff; /* 背景色改為白色 */
            padding: 20px;
        }}
        .container {{
            max-width: 600px;
            margin: auto;
            background: #fff;
            padding: 20px;
            border-radius: 5px;
            box-shadow: 0 0 10px rgba(0,0,0,0.1);
        }}
        .logo {{
            text-align: center;
            margin-bottom: 20px;
        }}
        .message {{
           color: #000;
        }}

    </style>
  <link href=""https://fonts.googleapis.com/css2?family=Noto+Sans+TC:wght@100;200;300;400;500;600;700;800;900&display=swap"" rel=""stylesheet"">
</head>
<body>
     <div style=""font-family:Noto Sans TC,Arial,Helvetica,sans-serif; line-height: 1.5; font-weight: normal; font-size: 15px; color: #2F3044; min-height: 100%; margin:0; padding:0; width:100%; background-color:#edf2f7"">
            <br>
            <table align=""center"" cellpadding=""0"" cellspacing=""0"" style=""border-collapse:collapse;margin:0 auto; padding:0; max-width:600px; width: 100%; border: 0"">
                <tbody>
                    <tr>
                        <td align=""center"" style=""text-align:center; padding: 40px; vertical-align: center"">
                                <img alt=""Logo"" style=""max-height: 100px"" src=""  {companyLogoUrl}"">
              
                        </td>
                    </tr>
                    <tr>
                        <td align=""left"" style=""vertical-align: center"">
                            <div style=""text-align:left; margin: 0 20px; padding: 40px; background-color:#ffffff; border-radius: 6px"">
                            <div class=""message"">
                                  <p>親愛的使用者，</p>
                                  <p>您在申請日期：{applicationCompleted.Date_Of_Application} 向數位內容設計研究中心申請了設備的借用。</p>
                                  <p>借用日期：{applicationCompleted.Borrow_Time}</p>
                                  <p>歸還日期：{applicationCompleted.Return_Time}</p>
                                  <p>您的借用日期已到期，提醒您請將設備歸還至數位內容設計研究中心！</p>
                                  <p>您借用的設備有(共{equipment_quaintity}項)：</p>
                                  <p>{borrowedEquipmentList}</p>
                            </div>
                                <div style=""padding-bottom: 12px; font-weight: bold;"">
                                    數位內容設計研究中心
                                </div>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style=""font-size: 13px; text-align:center; vertical-align: center; padding: 20px; color: #6d6e7c;"">
                            <p>
                                Copyright ©{@DateTime.Now.Year}
                                <a href=""https://www.ddc.tw/"" rel=""noopener"" target=""_blank"">數位內容設計研究中心</a>
                                &
                                <a href=""https://www.eduverse.tw"" rel=""noopener"" target=""_blank"">EduVerse教育元宇宙</a>
                            </p>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
</body>
</html>";
            }

            await emailSender.SendEmailAsync(email, subject, message);

            // 設定狀態碼為 204 No Content
            return NoContent();
        }

        public async Task sendEmail_Verification_Code(string email, string verification_code)
        {
            var subject = "設備借用驗證碼";
            var greeting = "親愛的使用者，";
            var message = $"您好！感謝您使用我們的服務。<br><br>請使用以下驗證碼完成設備借用驗證：<br><br><strong>{verification_code}</strong><br><br>如果您沒有執行此操作，請忽略此郵件。<br><br>祝您使用愉快！";

            // 添加品牌標誌
            var companyLogoUrl = "https://www.ddc.tw/uploads/1/4/0/3/140336294/logo-03.png"; // 替換為您公司的標誌 URL
            var companyLogoHtml = $"<img src=\"{companyLogoUrl}\" alt=\"Company Logo\" style=\"max-width: 100%; height: auto;\">";

            //            // 製作 HTML 格式的郵件內容
            //            var htmlMessage = $@"
            //<!DOCTYPE html>
            //<html lang=""en"">
            //<head>
            //    <meta charset=""UTF-8"">
            //    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
            //    <title>設備借用驗證碼</title>
            //    <style>
            //html,body {{ padding: 0; margin:0}}
            //        /* 添加您喜歡的 CSS 樣式 */
            //        body {{
            //            font-family: Arial, sans-serif;
            //            line-height: 1.6;
            //            background-color: #ffffff; /* 背景色改為白色 */
            //            padding: 20px;
            //        }}
            //        .container {{
            //            max-width: 600px;
            //            margin: auto;
            //            background: #fff;
            //            padding: 20px;
            //            border-radius: 5px;
            //            box-shadow: 0 0 10px rgba(0,0,0,0.1);
            //        }}
            //        .logo {{
            //            text-align: center;
            //            margin-bottom: 20px;
            //        }}
            //        .message {{
            //            color: #333;
            //        }}

            //    </style>
            //  <link href=""https://fonts.googleapis.com/css2?family=Noto+Sans+TC:wght@100;200;300;400;500;600;700;800;900&display=swap"" rel=""stylesheet"">
            //</head>
            //<body>
            //    <div class=""container"">
            //        <div class=""logo"">
            //            {companyLogoHtml}
            //        </div>
            //<div class=""message"">
            //    <p>{greeting}</p>
            //    <p>{message}</p>
            //</div>
            //    </div>
            //</body>
            //</html>";
            // 製作 HTML 格式的郵件內容
            var htmlMessage = $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>設備借用驗證碼</title>
    <style>
html,body {{ padding: 0; margin:0}}
        /* 添加您喜歡的 CSS 樣式 */
        body {{
            font-family: Arial, sans-serif;
            line-height: 1.6;
            background-color: #ffffff; /* 背景色改為白色 */
            padding: 20px;
        }}
        .container {{
            max-width: 600px;
            margin: auto;
            background: #fff;
            padding: 20px;
            border-radius: 5px;
            box-shadow: 0 0 10px rgba(0,0,0,0.1);
        }}
        .logo {{
            text-align: center;
            margin-bottom: 20px;
        }}
        .message {{
            color: #333;
        }}

    </style>
  <link href=""https://fonts.googleapis.com/css2?family=Noto+Sans+TC:wght@100;200;300;400;500;600;700;800;900&display=swap"" rel=""stylesheet"">
</head>
<body>
     <div style=""font-family:Noto Sans TC,Arial,Helvetica,sans-serif; line-height: 1.5; font-weight: normal; font-size: 15px; color: #2F3044; min-height: 100%; margin:0; padding:0; width:100%; background-color:#edf2f7"">
            <br>
            <table align=""center"" cellpadding=""0"" cellspacing=""0"" style=""border-collapse:collapse;margin:0 auto; padding:0; max-width:600px; width: 100%; border: 0"">
                <tbody>
                    <tr>
                        <td align=""center"" style=""text-align:center; padding: 40px; vertical-align: center"">
                                <img alt=""Logo"" style=""max-height: 100px"" src=""  {companyLogoUrl}"">
              
                        </td>
                    </tr>
                    <tr>
                        <td align=""left"" style=""vertical-align: center"">
                            <div style=""text-align:left; margin: 0 20px; padding: 40px; background-color:#ffffff; border-radius: 6px"">
                                        <div class=""message"">
                        <p>{greeting}</p>
                        <p>{message}</p>
                    </div>
                                <div style=""padding-bottom: 12px; font-weight: bold;"">
                                    數位內容設計研究中心
                                </div>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style=""font-size: 13px; text-align:center; vertical-align: center; padding: 20px; color: #6d6e7c;"">
                            <p>
                                Copyright ©{@DateTime.Now.Year}
                                <a href=""https://www.ddc.tw/"" rel=""noopener"" target=""_blank"">數位內容設計研究中心</a>
                                &
                                <a href=""https://www.eduverse.tw"" rel=""noopener"" target=""_blank"">EduVerse教育元宇宙</a>
                            </p>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
</body>
</html>";
            // 以 HTML 格式發送郵件
            await emailSender.SendEmailAsync(email, subject, htmlMessage);
        }



        public string Email_Verification_Code()
        {
            // 建立一個隨機數產生器
            Random random = new Random();
            // 建立一個字元陣列，存放所有可能的字元
            char[] englishwords_random = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0".ToCharArray();
            char[] number_random = "123456789".ToCharArray();
            // 建立一個 StringBuilder 物件，用來儲存隨機字串
            StringBuilder sb = new StringBuilder();
            // 產生指定長度的隨機字串
            for (int i = 0; i < 6; i++)
            {
                // 生成介於 1 和 2 之間的隨機整數 
                //用來判斷要選英文字還是數字 
                //randomNumber= 1 為數字
                //randomNumber = 2 為英文
                int randomNumber = random.Next(1, 3);
                if (randomNumber == 1)
                {
                    // 在number_random中產生一個隨機索引
                    int number_random_Index = random.Next(number_random.Length);
                    // 將隨機字元新增到 StringBuilder 物件中
                    sb.Append(number_random[number_random_Index]);
                }
                else
                {
                    // 在englishwords_random中產生一個隨機索引
                    int tenglishwords_random_Index = random.Next(englishwords_random.Length);
                    // 將隨機字元新增到 StringBuilder 物件中
                    sb.Append(englishwords_random[tenglishwords_random_Index]);
                }
            }
            // 傳回隨機字串
            return sb.ToString();
        }
    }
}

