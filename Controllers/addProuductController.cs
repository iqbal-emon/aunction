
using aunction.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

[ApiController]
public class AddProudctController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private object ImageField;

    public AddProudctController(IConfiguration config)
    {
        _configuration = config;
    }
   
    [HttpPost]
    [Route("InsertProduct")]
    public async Task<IActionResult> InsertBlogAsync([FromForm] addProudct blog)
    {
        Console.WriteLine("Hello");
        try
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("INSERT INTO Items (UserID,Title, Description,Category,ReservePrice, ImageURL,StartTime,EndTime) VALUES (@UserID,@Title, @Description,@Category,@ReservePrice, CAST(@ImageURL AS VARBINARY(MAX)),@StartTime,@EndTime)", con))
                {
                    cmd.Parameters.AddWithValue("@Title", blog.Title);
                   
                    cmd.Parameters.AddWithValue("@Description", blog.Description);
                    cmd.Parameters.AddWithValue("@Category", blog.Category);
                    cmd.Parameters.AddWithValue("@ReservePrice", blog.ReservePrice);
                    cmd.Parameters.AddWithValue("@StartTime", blog.StartTime);
                    Console.WriteLine(blog.StartTime);
                    cmd.Parameters.AddWithValue("@EndTime", blog.EndTime);
                    //cmd.Parameters.AddWithValue("@userIdID", blog.userIdID);
                    Console.WriteLine(blog.EndTime);
                    cmd.Parameters.AddWithValue("@UserID", blog.UserID);

                    if (blog.ImageURL != null && blog.ImageURL.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await blog.ImageURL.CopyToAsync(memoryStream);
                            byte[] imageBytes = memoryStream.ToArray();
                            cmd.Parameters.AddWithValue("@ImageURL", imageBytes);
                        }
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@ImageURL", DBNull.Value); // Adjust the parameter name
                    }

                    int rowsAffected = cmd.ExecuteNonQuery();
                    Console.WriteLine($"Rows Affected: {rowsAffected}");
                }

                con.Close();
            }

            return Ok("Blog inserted successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
        }
    }

}