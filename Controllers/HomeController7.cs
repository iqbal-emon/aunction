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

            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 * FROM bids " +
                                       "INNER JOIN Customers ON Customers.CustomerID = Bids.CustomerID " +
                                       "WHERE bids.CustomerID = @CustomerID " +
                                       "ORDER BY Bids.Amount DESC", con))
            {
                cmd.Parameters.AddWithValue("@CustomerID", CustomerID);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        bidStatus dto = new bidStatus
                        {
                            ItemID = Convert.ToInt32(row["ItemID"]),
                            CustomerID = Convert.ToString(row["CustomerID"]),
                            Amount = Convert.ToString(row["Amount"])
                        };

                        Lst.Add(dto);
                    }
                }
            }
        }

        return Lst;
    }
}
