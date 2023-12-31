﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ADBFinal.DataAccessLayer.Entity
{
    public class User
    {
        public User(int userId,string userName, string userEmail, byte[] userPasswordHash, byte[] userPasswordSalt)
        {
            UserId = userId;
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

        public List<int> UserHistory {  get; set; } = new List<int>();

        public List<int> UserWishlist { get; set; } = new List<int>();

        public List<int> UserCart { get; set; } = new List<int>();

        public bool IsAdmin { get; set; }

    }
}
