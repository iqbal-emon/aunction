namespace aunction.Model
{
    public class bidStatus
    {
        public int ItemID { get; set; }


        public int UserID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal ReservePrice { get; set; }
        public IFormFile ImageURL { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public byte[]? ImageField { get; set; }
        public string? ItemID1 { get; set; }
        public string? UserID1 { get; set; }
        public string? ItemID2 { get; set; }
        internal string? BidID;

    
        public string CustomerID { get; set; }
        public string Amount { get; set; }

        public DateTime? BidTime { get; set; }
        public string Email { get; set; }

    }
}
