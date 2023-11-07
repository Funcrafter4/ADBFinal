using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ADBFinal.DataAccessLayer.Entity
{
    public class User
    {
        public User(string userName, string userEmail, byte[] userPasswordHash, byte[] userPasswordSalt)
        {
            UserName = userName;
            UserEmail = userEmail;
            UserPasswordHash = userPasswordHash;
            UserPasswordSalt = userPasswordSalt;
        }


        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id{ get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public string UserEmail {  get; set; }

        public byte[] UserPasswordHash { get; set; }

        public byte[] UserPasswordSalt { get; set; }

    }
}
