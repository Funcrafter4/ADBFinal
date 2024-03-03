using ADBFinal.DataAccessLayer.Entity;

namespace ADBFinal.DataAccessLayer.DTO
{
    public class OrderInfoDTO
    {
        public int OrderId { get; set; }
        public string Status { get; set; }
        public DateTime Timestamp { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public List<Product> Products { get; set; } // Assuming Product is a class representing product information
    }
}

