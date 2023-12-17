using aunction.Model; // Make sure to include the correct namespace for IFormFile
using Microsoft.AspNetCore.Http; // Add this namespace for IFormFile
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

[ApiController]
public class HomeController6 : ControllerBase
{
    private readonly IConfiguration _configuration;

    public HomeController6(IConfiguration config)
    {
        _configuration = config;
    }

    [HttpGet]
    [Route("Home/ItemJoin/{ItemID}")]
    public List<bidStatus> GetAllDetails(int ItemID)
    {
        List<bidStatus> Lst = new List<bidStatus>();

        using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            con.Open();

            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 * FROM Items " +
                                                "INNER JOIN Bids ON Items.ItemID = Bids.ItemID " +
                                                "INNER JOIN Customers ON Bids.CustomerID = Customers.CustomerID " +
                                                "WHERE Items.ItemID = @ItemID " +
                                                "ORDER BY Bids.Amount DESC", con))
            {
                cmd.Parameters.AddWithValue("@ItemID", ItemID);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        bidStatus dto = new bidStatus
                        {
                            ItemID = Convert.ToInt32(row["ItemID"]),
                            UserID = Convert.ToInt32(row["UserID"]),
                            Title = Convert.ToString(row["Title"]),
                            Description = Convert.ToString(row["Description"]),
                            Category = Convert.ToString(row["Category"]),
                            ReservePrice = Convert.ToDecimal(row["ReservePrice"]),
                            StartTime = row["StartTime"] != DBNull.Value ? Convert.ToDateTime(row["StartTime"]) : DateTime.MinValue,
                            EndTime = row["EndTime"] != DBNull.Value ? Convert.ToDateTime(row["EndTime"]) : DateTime.MinValue,
                            ImageURL = null, // Assuming ImageURL is not fetched from the database
                            ImageField = row["ImageURL"] != DBNull.Value ? ConvertToByteArray(row["ImageURL"]) : null,
                            ItemID1 = Convert.ToString(row["ItemID"]),
                            UserID1 = Convert.ToString(row["UserID"]),
                            ItemID2 = Convert.ToString(row["ItemID"]),
                            BidID = Convert.ToString(row["BidID"]), // Assuming BidID is the correct column name
                            CustomerID = Convert.ToString(row["CustomerID"]),
                            Amount = Convert.ToString(row["Amount"]),
                            Email = Convert.ToString(row["Email"]),

                            BidTime = row["BidTime"] != DBNull.Value ? Convert.ToDateTime(row["BidTime"]) : (DateTime?)null,
                        };

                        Lst.Add(dto);
                    }
                }
            }
        }

        return Lst;
    }

    private byte[] ConvertToByteArray(object value)
    {
        if (value is byte[] byteArray)
        {
            return byteArray;
        }

        // Handle other conversions if necessary
        return null;
    }
}
