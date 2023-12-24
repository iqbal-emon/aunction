using aunction.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;

[ApiController]
public class blockAdminProduct : ControllerBase
{
    private readonly IConfiguration _configuration;

    public blockAdminProduct(IConfiguration config)
    {
        _configuration = config;
    }

    [HttpGet]
    [Route("blockSellerAdd")]
    public IActionResult BlockSeller(string userID)
    {
        try
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                con.Open();

                string updateQuery = "UPDATE Users SET flag2 = '1' WHERE UserID = @UserID";

                using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                {
                    cmd.Parameters.AddWithValue("@UserID", userID);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    Console.WriteLine($"Rows Affected: {rowsAffected}");
                }

                con.Close();
            }

            return Ok("User flagged successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }

    [HttpGet]
    [Route("UnblockSellerAdd/{userID}")]
    public IActionResult UnblockSeller(string userID)
    {
        try
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                con.Open();

                string updateQuery = "UPDATE Users SET flag2 =NULL WHERE UserID = @UserID";

                using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                {
                    cmd.Parameters.AddWithValue("@UserID", userID);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    Console.WriteLine($"Rows Affected: {rowsAffected}");
                }

                con.Close();
            }

            return Ok("User unblocked successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }
}
