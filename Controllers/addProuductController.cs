using aunction.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;

[ApiController]
public class AddProudctController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public AddProudctController(IConfiguration config, IWebHostEnvironment environment)
    {
        _configuration = config;
        _environment = environment;
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
                                ? ConvertToByteArray(Convert.ToString(row["ImageURL"]))
                                : null,
                            ImageField1 = row["ImageURL1"] != DBNull.Value
                                ? ConvertToByteArray(Convert.ToString(row["ImageURL1"]))
                                : null,
                            ImageField2 = row["ImageURL2"] != DBNull.Value
                                ? ConvertToByteArray(Convert.ToString(row["ImageURL2"]))
                                : null
                        };

                        Lst.Add(dto);
                    }
                }
            }
        }

        // Rest of the code...

        return Lst;
    }

    private byte[] ConvertToByteArray(string filePath)
    {
        if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
        {
            return System.IO.File.ReadAllBytes(filePath);
        }

        // Handle other cases or return null if the file doesn't exist
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

                // Save images to server and get file paths
                var imageURL = await SaveImageToServer(product.ImageURL);
                var imageURL1 = await SaveImageToServer(product.ImageURL1);
                var imageURL2 = await SaveImageToServer(product.ImageURL2);

                using (SqlCommand cmd = new SqlCommand("INSERT INTO Items (UserID, Title, Description, Category, ReservePrice, ImageURL, ImageURL1, ImageURL2, StartTime, EndTime) VALUES (@UserID, @Title, @Description, @Category, @ReservePrice, @ImageURL, @ImageURL1, @ImageURL2, @StartTime, @EndTime)", con))
                {
                    cmd.Parameters.AddWithValue("@Title", product.Title);
                    cmd.Parameters.AddWithValue("@Description", product.Description);
                    cmd.Parameters.AddWithValue("@Category", product.Category);
                    cmd.Parameters.AddWithValue("@ReservePrice", product.ReservePrice);
                    cmd.Parameters.AddWithValue("@StartTime", product.StartTime);
                    cmd.Parameters.AddWithValue("@EndTime", product.EndTime);
                    cmd.Parameters.AddWithValue("@UserID", product.UserID);

                    cmd.Parameters.AddWithValue("@ImageURL", imageURL);
                    cmd.Parameters.AddWithValue("@ImageURL1", imageURL1);
                    cmd.Parameters.AddWithValue("@ImageURL2", imageURL2);

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
    [HttpPut]
    [Route("Home/ProductUpdate/{ItemID}")]
    public async Task<IActionResult> UpdateProductAsync(int ItemID, [FromForm] addProudct product)
    {
        try
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                con.Open();

                // Check if the product with the specified ItemID exists


                // Save images to server and get file paths
                var imageURL = await SaveImageToServer(product.ImageURL);
                var imageURL1 = await SaveImageToServer(product.ImageURL1);
                var imageURL2 = await SaveImageToServer(product.ImageURL2);


                using (SqlCommand cmd = new SqlCommand("UPDATE Items SET Title = @Title, Description = @Description, Category = @Category, ReservePrice = @ReservePrice, StartTime = @StartTime, EndTime = @EndTime" +
                    "{0} WHERE ItemID = @ItemID", con))
                {
                    cmd.Parameters.AddWithValue("@Title", product.Title);
                    cmd.Parameters.AddWithValue("@Description", product.Description);
                    cmd.Parameters.AddWithValue("@Category", product.Category);
                    cmd.Parameters.AddWithValue("@ReservePrice", product.ReservePrice);
                    cmd.Parameters.AddWithValue("@StartTime", product.StartTime);
                    cmd.Parameters.AddWithValue("@EndTime", product.EndTime);
                    cmd.Parameters.AddWithValue("@ItemID", ItemID);

                    StringBuilder additionalConditions = new StringBuilder();

                    if (product.ImageURL != null)
                    {
                        additionalConditions.Append(", ImageURL = @ImageURL");
                        cmd.Parameters.AddWithValue("@ImageURL", imageURL);
                    }
                    if (product.ImageURL1 != null)
                    {
                        additionalConditions.Append(", ImageURL1 = @ImageURL1");
                        cmd.Parameters.AddWithValue("@ImageURL1", imageURL1);
                    }
                    if (product.ImageURL2 != null)
                    {
                        additionalConditions.Append(", ImageURL2 = @ImageURL2");
                        cmd.Parameters.AddWithValue("@ImageURL2", imageURL2);
                    }

                    cmd.CommandText = string.Format(cmd.CommandText, additionalConditions.ToString());

                    int rowsAffected = cmd.ExecuteNonQuery();
                    Console.WriteLine($"Rows Affected: {rowsAffected}");
                }



                con.Close();

            }

            return Ok($"Product with ItemID {ItemID} updated successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
        }
    }

    // Helper method to save image to server and return file path
    private async Task<string> SaveImageToServer(IFormFile imageFile)
    {
        if (imageFile != null && imageFile.Length > 0)
        {
            var uploadsFolderPath = Path.Combine(_environment.ContentRootPath, "Images", "Uploads");
            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            var imageName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
            var imagePath = Path.Combine(uploadsFolderPath, imageName);

            using (var stream = System.IO.File.Create(imagePath))
            {
                await imageFile.CopyToAsync(stream);
            }

            return imagePath; // You can store this path in the database
        }
        else
        {
            return null;
        }
    }
}
