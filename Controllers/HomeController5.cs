using aunction.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;

public class HomeController5 : ControllerBase
{
    private readonly IConfiguration _configuration;

    public HomeController5(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost]
    [Route("Home/UpdateProduct/{ItemID}")]
    public async Task<IActionResult> UpdateProduct(int ItemID, [FromForm] addProudct product)
    {
        try
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await con.OpenAsync();

                string updateQuery = "UPDATE items SET Title = @Title, Description = @Description, " +
                                     "Category = @Category, ReservePrice = @ReservePrice, " +
                                     "ImageURL = COALESCE(CAST(@ImageURL AS VARBINARY(MAX)), ImageURL), " +
                                     "StartTime = @StartTime, EndTime = @EndTime " +
                                     "WHERE itemID = @ItemID";

                using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                {
                    cmd.Parameters.AddWithValue("@ItemID", product.ItemID);
                    cmd.Parameters.AddWithValue("@Title", product.Title);
                    cmd.Parameters.AddWithValue("@Description", product.Description);
                    cmd.Parameters.AddWithValue("@Category", product.Category);
                    cmd.Parameters.AddWithValue("@ReservePrice", product.ReservePrice);
                    cmd.Parameters.AddWithValue("@StartTime", product.StartTime);
                    cmd.Parameters.AddWithValue("@EndTime", product.EndTime);

                    if (product.ImageURL != null && product.ImageURL.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await product.ImageURL.CopyToAsync(memoryStream);
                            byte[] imageBytes = memoryStream.ToArray();
                            cmd.Parameters.AddWithValue("@ImageURL", imageBytes);
                        }
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@ImageURL", DBNull.Value);
                    }

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    Console.WriteLine($"Rows Affected: {rowsAffected}");
                }

                await con.CloseAsync();
            }

            return Ok("Product updated successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
        }
    }
}

