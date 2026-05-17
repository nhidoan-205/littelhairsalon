using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace LittleHairSalon.Services
{
    public class EmailService
    {
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            // 1. Điền Gmail mới của bạn vào đây
            string myEmail = "gmail-moi-cua-ban@gmail.com";

            // 2. Điền Mật khẩu ứng dụng (16 ký tự) vào đây
            string myAppPassword = "mat-khau-ung-dung-16-ki-tu";

            // 3. ĐÃ SỬA: Đổi tên hiển thị thành LittleHairSalon
            var fromAddress = new MailAddress(myEmail, "LittleHairSalon");
            var toAddress = new MailAddress(toEmail);

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, myAppPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true // Nên đổi thành true để sau này gửi thư có in đậm, in nghiêng, màu sắc cho đẹp
            })
            {
                await smtp.SendMailAsync(message);
            }
        }
    }
}