using aunction.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

[ApiController]
public class HomeController4 : ControllerBase
{
    private readonly IConfiguration _configuration;

    public HomeController4(IConfiguration config)
    {
        _configuration = config;
    }



    [HttpPost]
    [Route("Home/BidInsert")]
    public async Task<IActionResult> InsertProductAsync([FromForm] bidding product)
    {
        try
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("INSERT INTO Bids (itemID,CustomerID,Amount,BidTime) VALUES (@itemID,@CustomerID,@Amount,@BidTime)", con))
                {
                    cmd.Parameters.AddWithValue("@Amount", product.Amount);
                    cmd.Parameters.AddWithValue("@BidTime", product.BidTime);
                    cmd.Parameters.AddWithValue("@CustomerID", product.CustomerID);
                    cmd.Parameters.AddWithValue("@ItemID", product.ItemID);

                


                    int rowsAffected = cmd.ExecuteNonQuery();
                    Console.WriteLine($"Rows Affected: {rowsAffected}");
                }

                con.Close();
            }

            return Ok("Product inserted successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
        }
    }
    [HttpGet]
    [Route("Home/GetBids/{ItemID}")]
    public List<bidding> GetBidsForItem(int ItemID)
    {
        List<bidding> bidList = new List<bidding>();

        try
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Bids WHERE ItemID = @ItemID", con))
                {
                    cmd.Parameters.AddWithValue("@ItemID", ItemID);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            bidding bid = new bidding
                            {
                                BidID = reader["BidID"].ToString(),
                                ItemID = reader["ItemID"].ToString(),
                                CustomerID = reader["CustomerID"].ToString(),
                                Amount = reader["Amount"].ToString(),
                                BidTime = reader["BidTime"] as DateTime?




                            };
                            Console.WriteLine("CustomerID: " + bid.CustomerID);
                            bidList.Add(bid);
                        }
                    }
                }

                con.Close();
            }

            return bidList;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            // You might want to handle errors more gracefully
            return null;
        }
    }


}
