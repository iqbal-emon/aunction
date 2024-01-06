using aunction.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;

[ApiController]
public class HomeController7 : ControllerBase
{
    private readonly IConfiguration _configuration;

    public HomeController7(IConfiguration config)
    {
        _configuration = config;
    }

    [HttpGet]
    [Route("Home/Win/{CustomerID}")]
    public List<bidStatus> GetAllDetails(string CustomerID)
    {
        List<bidStatus> Lst = new List<bidStatus>();

        using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            con.Open();
            using (SqlCommand command = new SqlCommand(
                "WITH RankedBids AS (" +
                "    SELECT " +
                "        b.*," +
                "        i.EndTime,i.Title," + // Include additional columns from the 'items' table
                "        RANK() OVER (PARTITION BY b.ItemID ORDER BY b.Amount DESC) AS BidRank " +
                "    FROM bids b" +
                "    JOIN items i ON b.ItemID = i.ItemID" + // Join with the 'items' table
                ")" +
                "SELECT * FROM RankedBids WHERE BidRank = 1 AND CustomerID = @CustomerID", con))
            {
                command.Parameters.AddWithValue("@CustomerID", CustomerID);

                using (SqlDataAdapter da = new SqlDataAdapter(command))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        bidStatus dto = new bidStatus
                        {
                            ItemID = Convert.ToInt32(row["ItemID"]),
                            CustomerID = Convert.ToString(row["CustomerID"]),
                            Amount = Convert.ToString(row["Amount"]),// Adjust the type accordingly
                            BidTime = row["BidTime"] != DBNull.Value ? Convert.ToDateTime(row["BidTime"]) : DateTime.MinValue,
                            EndTime = row["EndTime"] != DBNull.Value ? Convert.ToDateTime(row["EndTime"]) : DateTime.MinValue,
                            Title = Convert.ToString(row["Title"]),


                        };

                        Lst.Add(dto);
                    }
                }
            }
        }

        return Lst;
    }
}
