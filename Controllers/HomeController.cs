
using aunction.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

[ApiController]
public class HomeController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private object ImageField;

    public HomeController(IConfiguration config)
    {
        _configuration = config;
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


                        };
                        Lst.Add(dto);
                    }
                }
            }

        }

        return Lst;
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

                using (SqlCommand cmd = new SqlCommand("INSERT INTO Users (Username, Password, Email) VALUES (@Username, @Password, @Email)", con))
                {
                    cmd.Parameters.AddWithValue("@Username", blog.Username);
                    cmd.Parameters.AddWithValue("@Password", blog.Password);
                    cmd.Parameters.AddWithValue("@Email", blog.Email);

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



