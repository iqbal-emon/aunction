using System.ComponentModel.DataAnnotations;

namespace aunction.Model
{
    public class auctionSystem
    {
        public string Username { get; set; }
        public string Password { get; set; }
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

    }

}
