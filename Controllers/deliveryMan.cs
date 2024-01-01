using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class DeliveryManController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public DeliveryManController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    [Route("GetID/{otpValue}")]
    public async Task<IActionResult> GetCustomersAsync(int otpValue)
    {
        try
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await con.OpenAsync(); // Use async Open method

                // Use a parameterized query to prevent SQL injection
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM customersDetails WHERE otp = @otpValue", con))
                {
                    cmd.Parameters.AddWithValue("@otpValue", otpValue);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            // OTP matched
                            return Ok("OTP matched");
                        }
                    }
                }
            }

            // OTP not matched
            return Ok("OTP not matched");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }
}
