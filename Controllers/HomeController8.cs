using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class HomeController8 : ControllerBase
{
    private readonly IConfiguration _configuration;

    public HomeController8(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    [Route("GetCustomersDetails")]
    public async Task<IActionResult> GetCustomersAsync()
    {
        try
        {
            List<CustomerModel> customers = new List<CustomerModel>();

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await con.OpenAsync(); // Use async Open method

                using (SqlCommand cmd = new SqlCommand("SELECT * FROM customersDetails", con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        CustomerModel customer = new CustomerModel
                        {
                            FullName = Convert.ToString(reader["FullName"]),
                            Email = Convert.ToString(reader["Email"]),
                            Phone = Convert.ToString(reader["Phone"]),
                            Address = Convert.ToString(reader["Address"]),
                            City = Convert.ToString(reader["City"]),
                            ItemID = reader["ItemID"] != DBNull.Value ? Convert.ToInt32(reader["ItemID"]) : 0,
                            otp = reader["otp"] != DBNull.Value ? Convert.ToInt32(reader["otp"]) : 0,

                        };

                        customers.Add(customer);
                    }
                }
            }

            return Ok(customers);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }

    [HttpPost]
    [Route("InsertCustomer")]
    public async Task<IActionResult> InsertCustomerAsync([FromForm] CustomerModel customer)
    {
        try
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("INSERT INTO customersDetails (FullName, Email, Phone, Address, City, ItemID,otp) VALUES (@FullName, @Email, @Phone, @Address, @City, @ItemID,@otp); SELECT SCOPE_IDENTITY();", con))
                {
                    cmd.Parameters.AddWithValue("@FullName", customer.FullName);
                    cmd.Parameters.AddWithValue("@Email", customer.Email);
                    cmd.Parameters.AddWithValue("@Phone", customer.Phone);
                    cmd.Parameters.AddWithValue("@Address", customer.Address);
                    cmd.Parameters.AddWithValue("@City", customer.City);
                    cmd.Parameters.AddWithValue("@ItemID", customer.ItemID);
                    cmd.Parameters.AddWithValue("@otp", customer.otp);

                    // ExecuteScalarAsync is used for queries that return a single value
                    var insertedItemId = await cmd.ExecuteScalarAsync();

                    // If needed, you can return the inserted ItemID
                    return Ok($"Insert successful.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }
}

public class CustomerModel
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public int ItemID { get; set; }
    public int otp { get; set; }

    // Add other properties as needed
}
