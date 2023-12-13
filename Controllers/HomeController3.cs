using aunction.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

[ApiController]
public class HomeController3 : ControllerBase
{
    private readonly IConfiguration _configuration;

    public HomeController3(IConfiguration config)
    {
        _configuration = config;
    }

    [HttpGet]
    [Route("Home/Item/{ItemID}")]
    public List<addProudct> GetAllDetails(int ItemID)
    {
        List<addProudct> Lst = new List<addProudct>();

        using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            con.Open();

            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Items WHERE ItemID = @ItemID", con))
            {
                cmd.Parameters.AddWithValue("@ItemID", ItemID);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        addProudct dto = new addProudct
                        {
                            ItemID2 = Convert.ToString(row["ItemID"]),
                            UserID1 = Convert.ToString(row["UserID"]),
                            Title = Convert.ToString(row["Title"]),
                            Description = Convert.ToString(row["Description"]),
                            Category = Convert.ToString(row["Category"]),
                            ReservePrice = Convert.ToDecimal(row["ReservePrice"]),
                            StartTime = row["StartTime"] != DBNull.Value ? Convert.ToDateTime(row["StartTime"]) : DateTime.MinValue,
                            EndTime = row["EndTime"] != DBNull.Value ? Convert.ToDateTime(row["EndTime"]) : DateTime.MinValue,
                            ImageField = row["ImageURL"] != DBNull.Value ? ConvertToByteArray(row["ImageURL"]) : null
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
}
