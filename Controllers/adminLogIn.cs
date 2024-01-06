
using aunction.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

[ApiController]
public class adminLogIn : ControllerBase
{
    private readonly IConfiguration _configuration;
    private object ImageField;

    public adminLogIn(IConfiguration config)
    {
        _configuration = config;
    }
    [HttpGet]
    [Route("admin/login")]
    public List<admin> adminLoginInfo()
    {
        List<admin> Lst = new List<admin>();

        using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))

        {
            con.Open();

            using (SqlCommand cmd = new SqlCommand("SELECT * FROM admins", con))
            {
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        admin dto = new admin
                        {

                            Username = row["Username"].ToString(),
                            Password = row["Password"].ToString(),
                            AdminID = row["AdminID"].ToString()

                        };
                        Lst.Add(dto);
                    }
                }
            }

        }

        return Lst;
    }

}