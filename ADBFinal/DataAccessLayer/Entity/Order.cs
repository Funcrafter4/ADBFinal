using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ADBFinal.DataAccessLayer.Entity
{
    public class Order
    {
        public Order(int orderId, int userId, string status, double longitude, double latitude, List<int> orderProducts, DateTime timestamp)
        {
            UserId = userId;
            OrderId = orderId;
            Status = status;
            Longitude = longitude;
            Latitude = latitude;
            OrderProducts = orderProducts;
            Timestamp = timestamp;
        }


        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        public int OrderId { get; set; }

        public int UserId { get; set; }

        public string Status { get; set; }

        public DateTime Timestamp { get; set; }

        public List<int> OrderProducts { get; set; } = new List<int>();

        public double Longitude {  get; set; }

        public double Latitude {  get; set; }


    }
}
