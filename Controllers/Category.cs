using aunction.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

[ApiController]
public class Category : ControllerBase
{
    private readonly IConfiguration _configuration;

    public Category(IConfiguration config)
    {
        _configuration = config;
    }

    [HttpGet]
    [Route("getCategory")]
    public List<categoryGot> getCategory()
    {
        List<categoryGot> Lst = new List<categoryGot>();

        using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            con.Open();

            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Category", con))
            {
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        categoryGot dto = new categoryGot
                        {
                            CategoryID = row["CategoryID"].ToString(),
                            CategoryName = row["CategoryName"].ToString(),
                        };
                        Lst.Add(dto);
                    }
                }
            }
        }

        return Lst;
    }

    [HttpPost]
    [Route("insertCategory")]
    public IActionResult InsertCategory([FromForm] categoryGot category)
    {
        try
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("INSERT INTO Category (CategoryName) VALUES (@CategoryName)", con))
                {
                    cmd.Parameters.AddWithValue("@CategoryName", category.CategoryName);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return Ok("Category inserted successfully.");
                    }
                    else
                    {
                        return BadRequest("Failed to insert category.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
