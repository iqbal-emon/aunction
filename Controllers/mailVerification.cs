// Other using statements...

using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;

[ApiController]
[Route("api")]
public class MailVerificationController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public MailVerificationController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("sendVerificationEmail")]
    public IActionResult SendVerificationEmail([FromBody] EmailRequest request)
    {
        try
        {
            // Validate the email (add more validation as needed)
            if (!IsValidEmail(request.Email))
            {
                return BadRequest(new { message = "Invalid email address" });
            }

            // Generate a verification code (you may want to use a more secure method)
            string verificationCode = Guid.NewGuid().ToString();

            // Send the verification email
            SendEmail(request.Email, verificationCode);

            // You may want to store the verification code in your database for later validation

            return Ok(new { message = "Verification email sent successfully" });
        }
        catch (Exception ex)
        {
            // Handle exceptions appropriately (log, return error response, etc.)
            return StatusCode(500, new { message = "An error occurred while sending the verification email." });
        }
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var mailAddress = new MailAddress(email);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private void SendEmail(string toEmail, string verificationCode)
    {
        using (var smtpClient = new SmtpClient())
        {
            var fromAddress = new MailAddress(_configuration["Smtp:ihemon281@gmail.com"], "Your Name");
            var toAddress = new MailAddress(toEmail);

            smtpClient.Host = _configuration["Smtp:smtp.elasticemail.com"];
            smtpClient.Port = int.Parse(_configuration["Smtp:2525"]);
            smtpClient.EnableSsl = true;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(_configuration["Smtp:ihemon281@gmail.com"], _configuration["Smtp:784B7800431B361EFFC1DB9FC276F5B9F1F2"]);

            var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = "Email Verification",
                Body = $"Your verification code is: {verificationCode}"
            };

            smtpClient.Send(message);
        }
    }
}

public class EmailRequest
{
    public string Email { get; set; }
}
