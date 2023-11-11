using ADBFinal.DataAccessLayer.Entity;
using MongoDB.Driver;

namespace ADBFinal.DataAccessLayer.DatabaseConnect
{
    public class DatabaseConnect
    {

        public static IMongoCollection<User> UserCollection()
        {
            var connector = new MongoDBHelper("mongodb+srv://Admin:ytP5DHSV5N4T1Kcw@adbfinal.wv3trmy.mongodb.net/", "MegaMart");
            var userCollection = connector.GetCollection<User>("Users");
            return userCollection;
        }

        public static IMongoCollection<Product> ProductCollection()
        {
            var connector = new MongoDBHelper("mongodb+srv://Admin:ytP5DHSV5N4T1Kcw@adbfinal.wv3trmy.mongodb.net/", "MegaMart");
            var productCollection = connector.GetCollection<Product>("Products");
            return productCollection;
        }

        public static IMongoCollection<Category> CategoryCollection()
        {
            var connector = new MongoDBHelper("mongodb+srv://Admin:ytP5DHSV5N4T1Kcw@adbfinal.wv3trmy.mongodb.net/", "MegaMart");
            var categoryCollection = connector.GetCollection<Category>("Categories");
            return categoryCollection;
        }
    }
}
