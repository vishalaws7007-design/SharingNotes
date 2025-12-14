using System.Net;
using System.Net.Mail;

namespace SharingNotes.Service
{
    public class EmailService
    {
        private readonly SmtpClient _client;

        public EmailService()
        {
            _client = new SmtpClient("sandbox.smtp.mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("b06ffb6c24e66a", "375382894bd6d0"),
                EnableSsl = true
            };
        }

        public void SendEmail(string toEmail, string subject, string body)
        {
            var from = "vishal39715@gmail.com";

            _client.Send(from, toEmail, subject, body);
        }

        public void SendPasswordResetEmail(string toEmail, string resetLink)
        {
            string subject = "Password Reset Request";

            string body = $@"
<!DOCTYPE html>
<html>
<head>
  <meta charset=""UTF-8"">
  <title>Password Reset</title>
</head>
<body style=""margin:0; padding:0; background-color:#f4f6f8; font-family: Arial, sans-serif;"">

  <div style=""width:100%; padding:30px 0;"">
    <div style=""
         max-width:600px;
         margin:auto;
         background-color:#ffffff;
         border-radius:8px;
         box-shadow:0 4px 10px rgba(0,0,0,0.08);
         overflow:hidden;"">

      <!-- Header -->
      <div style=""background-color:#0d6efd; color:#ffffff; padding:20px; text-align:center;"">
        <h2 style=""margin:0; font-size:22px;"">Password Reset Request</h2>
      </div>

      <!-- Body -->
      <div style=""padding:30px; color:#333333; font-size:14px; line-height:1.6;"">
        <p>Hello,</p>

        <p>
          We received a request to reset your password. Click the button below to create a new one.
        </p>

        <p style=""text-align:center; margin:25px 0;"">
          <a href=""{resetLink}""
             style=""
               display:inline-block;
               padding:12px 24px;
               background-color:#0d6efd;
               color:#ffffff;
               text-decoration:none;
               font-size:15px;
               font-weight:bold;
               border-radius:5px;"">
            Reset Password
          </a>
        </p>

        <p style=""color:#6c757d; font-size:13px;"">
          If the button doesn’t work, copy and paste this link into your browser:
        </p>

        <div style=""
             background-color:#f8f9fa;
             padding:12px;
             border-radius:5px;
             word-break:break-all;
             font-size:13px;"">
          <a href=""{resetLink}"" style=""color:#0d6efd;"">
            {resetLink}
          </a>
        </div>

        <p style=""margin-top:15px; color:#6c757d; font-size:13px;"">
          ⏰ This link will expire in <strong>1 hour</strong>.
        </p>

        <p style=""color:#6c757d; font-size:13px;"">
          If you did not request this password reset, you can safely ignore this email.
        </p>
      </div>

      <!-- Footer -->
      <div style=""
           background-color:#f8f9fa;
           padding:15px;
           text-align:center;
           font-size:12px;
           color:#777777;"">
        <p style=""margin:0;"">
          Thanks,<br />
          <strong>Your App Team</strong>
        </p>
      </div>

    </div>
  </div>

</body>
</html>";
            

            SendEmail(toEmail, subject, body);
        }

    }
}
