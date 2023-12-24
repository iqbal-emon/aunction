using aunction.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;

[ApiController]
public class customerNotBid : ControllerBase
{
    private readonly IConfiguration _configuration;

    public customerNotBid(IConfiguration config)
    {
        _configuration = config;
    }

    [HttpGet]
    [Route("customerNotBid")]
    public IActionResult CustomerNotBid(string CustomerID)
    {
        try
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                con.Open();

                string updateQuery = "UPDATE Customers SET flag2 = '1' WHERE CustomerID = @CustomerID";

                using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", CustomerID);

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
    [Route("customerNotBidUnblock/{CustomerID}")]
    public IActionResult customerNotBidUnblock(string CustomerID)
    {
        try
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                con.Open();

                string updateQuery = "UPDATE Customers SET flag2 =NULL WHERE CustomerID = @CustomerID";

                using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", CustomerID);

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
