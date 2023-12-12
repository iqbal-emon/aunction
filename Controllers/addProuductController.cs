using aunction.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;

[ApiController]
public class AddProudctController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AddProudctController(IConfiguration config)
    {
        _configuration = config;
    }



    [HttpGet]
    [Route("Home/GetAllDetails/{UserID}")]
    public List<addProudct> GetAllDetails(int UserID)
    {
        List<addProudct> Lst = new List<addProudct>();

        using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            con.Open();

            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Items WHERE UserID = @UserID", con))
            {
                cmd.Parameters.AddWithValue("@UserID", UserID);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        addProudct dto = new addProudct
                        {
                            ItemID1 = Convert.ToString(row["ItemID"]),
                            UserID1 = Convert.ToString(row["UserID"]),
                            Title = Convert.ToString(row["Title"]),
                            Description = Convert.ToString(row["Description"]),
                            Category = Convert.ToString(row["Category"]),
                            ReservePrice = Convert.ToDecimal(row["ReservePrice"]),
                            StartTime = Convert.ToDateTime(row["StartTime"]),
                            EndTime = Convert.ToDateTime(row["EndTime"]),
                            ImageField = row["ImageURL"] != DBNull.Value
                                ? ConvertToByteArray(row["ImageURL"])
                                : null
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



    [HttpPost]
    [Route("Home/ProductInsert")]
    public async Task<IActionResult> InsertProductAsync([FromForm] addProudct product)
    {
        try
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("INSERT INTO Items (UserID,Title, Description,Category,ReservePrice, ImageURL,StartTime,EndTime) VALUES (@UserID,@Title, @Description,@Category,@ReservePrice, CAST(@ImageURL AS VARBINARY(MAX)),@StartTime,@EndTime)", con))
                {
                    cmd.Parameters.AddWithValue("@Title", product.Title);
                    cmd.Parameters.AddWithValue("@Description", product.Description);
                    cmd.Parameters.AddWithValue("@Category", product.Category);
                    cmd.Parameters.AddWithValue("@ReservePrice", product.ReservePrice);
                    cmd.Parameters.AddWithValue("@StartTime", product.StartTime);
                    cmd.Parameters.AddWithValue("@EndTime", product.EndTime);
                    cmd.Parameters.AddWithValue("@UserID", product.UserID);

                    if (product.ImageURL != null && product.ImageURL.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await product.ImageURL.CopyToAsync(memoryStream);
                            byte[] imageBytes = memoryStream.ToArray();
                            cmd.Parameters.AddWithValue("@ImageURL", imageBytes);
                            Console.WriteLine(product.ImageURL);
                        }
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@ImageURL", DBNull.Value); // Adjust the parameter name
                        Console.WriteLine(product.ImageURL);

                    }

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


}
