
using aunction.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

[ApiController]
public class HomeController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private object ImageField;
    private readonly IWebHostEnvironment _environment;

    public HomeController(IConfiguration config, IWebHostEnvironment environment)
    {
        _configuration = config;
        _environment = environment;
    }
    [HttpGet]
    [Route("GetAllDetails")]
    public List<auctionSystem> GetAllDetails()
    {
        List<auctionSystem> Lst = new List<auctionSystem>();

        using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))

        {
            con.Open();


            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Users", con))
            {
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        auctionSystem dto = new auctionSystem
                        {

                            Username = row["Username"].ToString(),
                            Password = row["Password"].ToString(),
                            Email = row["Email"].ToString(),
                            UserID= row["UserID"].ToString(),
                            flag = row["flag"].ToString(),
                            flag2 = row["flag2"].ToString(),
                            ImageField = row["ImageURL"] != DBNull.Value
                                ? ConvertToByteArray(Convert.ToString(row["ImageURL"]))
                                : null


                        };
                        Lst.Add(dto);
                    }
                }
            }

        }

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
    [Route("InsertLogin")]
    public async Task<IActionResult> InsertBlogAsync([FromForm] auctionSystem blog)
    {
        Console.WriteLine("Hello");
        try
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                con.Open();
                var imageURL = await SaveImageToServer(blog.ImageURL);

                using (SqlCommand cmd = new SqlCommand("INSERT INTO Users (Username, Password, Email,ImageURL) VALUES (@Username, @Password, @Email,@ImageURL)", con))
                {
                    cmd.Parameters.AddWithValue("@Username", blog.Username);
                    cmd.Parameters.AddWithValue("@Password", blog.Password);
                    cmd.Parameters.AddWithValue("@Email", blog.Email);
                    cmd.Parameters.AddWithValue("@ImageURL", imageURL);

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



