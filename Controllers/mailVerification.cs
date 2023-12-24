// Install the necessary NuGet packages: Microsoft.Extensions.DependencyInjection, System.Net.Mail, System.Net.Http.Json

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration; // Add this for configuration
using System;
using System.Net;
using System.Net.Mail;

[ApiController]
[Route("api")]
public class MailVerificationController : ControllerBase
{
    private readonly SmtpClient _smtpClient;
    private readonly IConfiguration _configuration;

    public MailVerificationController(SmtpClient smtpClient, IConfiguration configuration)
    {
        _smtpClient = smtpClient;
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
        var fromAddress = new MailAddress(_configuration["Smtp:FromEmail"], "Your Name");
        var toAddress = new MailAddress(toEmail);

        _smtpClient.Host = _configuration["Smtp:Host"];
        _smtpClient.Port = int.Parse(_configuration["Smtp:Port"]);
        _smtpClient.EnableSsl = true;
        _smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        _smtpClient.UseDefaultCredentials = false;
        _smtpClient.Credentials = new NetworkCredential(_configuration["Smtp:Username"], _configuration["Smtp:Password"]);

        var message = new MailMessage(fromAddress, toAddress)
        {
            Subject = "Email Verification",
            Body = $"Your verification code is: {verificationCode}"
        };

        _smtpClient.Send(message);
    }
}

public class EmailRequest
{
    public string Email { get; set; }
}
