
using aunction.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

[ApiController]
public class HomeController1 : ControllerBase
{
    private readonly IConfiguration _configuration;
    private object ImageField;

    public HomeController1(IConfiguration config)
    {
        _configuration = config;
    }
    [HttpGet]
    [Route("GetAllDetails1")]
    public List<signUp> GetAllDetails()
    {
        List<signUp> Lst = new List<signUp>();

        using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))

        {
            con.Open();

            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Customers", con))
            {
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        signUp dto = new signUp
                        {

                            Username = row["Username"].ToString(),
                            Password = row["Password"].ToString(),
                            Email = row["Email"].ToString(),
                            CustomerID = row["CustomerID"].ToString(),


                        };
                        Lst.Add(dto);
                    }
                }
            }

        }

        return Lst;
    }




    [HttpPost]
    [Route("InsertLogin1")]
    public async Task<IActionResult> InsertBlogAsync([FromForm] auctionSystem blog)
    {
        Console.WriteLine("Hello");
        try
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("INSERT INTO Customers (Username, Password, Email) VALUES (@Username, @Password, @Email)", con))
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



