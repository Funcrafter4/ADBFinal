using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ADBFinal.DataAccessLayer.Entity
{
    public class Category
    {
        public Category(string categoryName, string categoryDescription)
        {
            CategoryName = categoryName;
            CategoryDescription = categoryDescription;

        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string CategoryDescription { get; set; }

    }
}
