using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ADBFinal.DataAccessLayer.Entity
{
    public class Product
    {
        public Product(int productId,string productName, string productImage, string productDescription, int productCategoryID, int productPrice)
        {
            ProductId = productId;
            ProductName = productName;
            ProductImage = productImage;
            ProductDescription = productDescription;
            ProductCategoryID = productCategoryID;
            ProductPrice = productPrice;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductImage {  get; set; }

        public string ProductDescription {  get; set; }

        public int ProductCategoryID { get; set; }

        public int ProductPrice { get; set; }


    }
}
