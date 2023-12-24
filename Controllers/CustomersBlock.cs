using aunction.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;

[ApiController]
public class customerBlock : ControllerBase
{
    private readonly IConfiguration _configuration;

    public customerBlock(IConfiguration config)
    {
        _configuration = config;
    }

    [HttpGet]
    [Route("customerBlock")]
    public IActionResult customerBlock1(string CustomerID)
    {
        try
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                con.Open();

                string updateQuery = "UPDATE Customers SET flag = '1' WHERE CustomerID = @CustomerID";

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
    [Route("customerUnblock/{CustomerID}")]
    public IActionResult customerUnblock(string CustomerID)
    {
        try
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                con.Open();

                string updateQuery = "UPDATE Customers SET flag =NULL WHERE CustomerID = @CustomerID";

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
