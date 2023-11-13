﻿using ADBFinal.DataAccessLayer.DatabaseConnect;
using ADBFinal.DataAccessLayer.DTO;
using ADBFinal.DataAccessLayer.Entity;
using ADBFinal.DataAccessLayer.HttpRequests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

namespace ADBFinal.Controllers
{
    public class UserController : Controller
    {

        [HttpPut("Add_to_Users_Wishlist")]
        public async Task<ActionResult<string>> AddWishlistItem(AddDeleteArrayRequest myJsonResponse)
        {
            var userCollection = DatabaseConnect.UserCollection();
            var filter = Builders<User>.Filter.Eq(u => u.UserId, myJsonResponse.UserId);
            var dbuser = userCollection.Find(filter).FirstOrDefault();
            if (dbuser != null)
            {
                var update = Builders<User>.Update.AddToSet(u => u.UserWishlist, myJsonResponse.ProductId);
                userCollection.UpdateOne(filter, update);
                return Ok("Product Added to Wishlist");
            }
                return NotFound("User Doesn't Exist");
        }

        [HttpPut("Add_to_Users_History")]
        public async Task<ActionResult<string>> AddItemToHistory(AddDeleteArrayRequest myJsonResponse)
        {
            var userCollection = DatabaseConnect.UserCollection();
            var filter = Builders<User>.Filter.Eq(u => u.UserId, myJsonResponse.UserId);
            var dbuser = userCollection.Find(filter).FirstOrDefault();
            if (dbuser != null)
            {
                var update = Builders<User>.Update.AddToSet(u => u.UserHistory, myJsonResponse.ProductId);
                userCollection.UpdateOne(filter, update);
                return Ok("Updated User History");
            }
            return NotFound("User Doesn't Exist");
        }

        [HttpPut("Add_to_Users_Cart")]
        public async Task<ActionResult<string>> AddCartItem(AddDeleteArrayRequest myJsonResponse)
        {
            var userCollection = DatabaseConnect.UserCollection();
            var filter = Builders<User>.Filter.Eq(u => u.UserId, myJsonResponse.UserId);
            var dbuser = userCollection.Find(filter).FirstOrDefault();
            if (dbuser != null)
            {
                var update = Builders<User>.Update.AddToSet(u => u.UserCart, myJsonResponse.ProductId);
                userCollection.UpdateOne(filter, update);
                return Ok("Product Added to Cart");
            }
            return NotFound("User Doesn't Exist");
        }

        [HttpPut("Delete_from_Users_Wishlist")]
        public async Task<ActionResult<string>> DeleteWishlistItem(AddDeleteArrayRequest myJsonResponse)
        {
            var userCollection = DatabaseConnect.UserCollection();
            var filter = Builders<User>.Filter.Eq(u => u.UserId, myJsonResponse.UserId);
            var dbuser = userCollection.Find(filter).FirstOrDefault();
            if (dbuser != null)
            {
                List<int> userwishlist = dbuser.UserWishlist;

                if (userwishlist.Contains(myJsonResponse.ProductId))
                {
                    var update = Builders<User>.Update.Pull(u => u.UserWishlist, myJsonResponse.ProductId);
                    userCollection.UpdateOne(filter, update);
                    return Ok("Product Deleted from Wishlist");
                }               
                return NotFound("No such item in wishlist");
            }
            return NotFound("User Doesn't Exist");
        }

        [HttpPut("Delete_from_Users_Cart")]
        public async Task<ActionResult<string>> DeleteCartItem(AddDeleteArrayRequest myJsonResponse)
        {
            var userCollection = DatabaseConnect.UserCollection();
            var filter = Builders<User>.Filter.Eq(u => u.UserId, myJsonResponse.UserId);
            var dbuser = userCollection.Find(filter).FirstOrDefault();
            if (dbuser != null)
            {
                List<int> usercart = dbuser.UserCart;

                if (usercart.Contains(myJsonResponse.ProductId))
                {
                    var update = Builders<User>.Update.Pull(u => u.UserCart, myJsonResponse.ProductId);
                    userCollection.UpdateOne(filter, update);
                    return Ok("Product Deleted from Cart");
                }
                return NotFound("No such item in Cart");
            }
            return NotFound("User Doesn't Exist");
        }

        [HttpPut("Delete_from_Users_History")]
        public async Task<ActionResult<string>> DeleteHistoryItem(AddDeleteArrayRequest myJsonResponse)
        {
            var userCollection = DatabaseConnect.UserCollection();
            var filter = Builders<User>.Filter.Eq(u => u.UserId, myJsonResponse.UserId);
            var dbuser = userCollection.Find(filter).FirstOrDefault();
            if (dbuser != null)
            {
                List<int> userhistory = dbuser.UserHistory;

                if (userhistory.Contains(myJsonResponse.ProductId))
                {
                    var update = Builders<User>.Update.Pull(u => u.UserHistory, myJsonResponse.ProductId);
                    userCollection.UpdateOne(filter, update);
                    return Ok("Product Deleted from History");
                }
                return NotFound("No such item in History");
            }
            return NotFound("User Doesn't Exist");
        }

        [HttpGet("Get_Products_from_Wishlist")]
        public async Task<ActionResult<List<Product>>> GetWishlist(int UserId)
        {
            var userCollection = DatabaseConnect.UserCollection();
            var filter = Builders<User>.Filter.Eq(u => u.UserId, UserId);
            var dbuser = await userCollection.Find(filter).FirstOrDefaultAsync();

            if (dbuser != null)
            {
                List<int> userwishlist = dbuser.UserWishlist;
                if (!userwishlist.IsNullOrEmpty())
                {
                    var productCollection = DatabaseConnect.ProductCollection();
                    var productFilter = Builders<Product>.Filter.In(p => p.ProductId, userwishlist);
                    var products = await productCollection.Find(productFilter).ToListAsync();
                    return Ok(products);
                }
                return NotFound("No Items in wishlist");
            }

            return NotFound("User Doesn't Exist");
        }

        [HttpGet("Get_Products_from_Cart")]
        public async Task<ActionResult<List<Product>>> GetCart(int UserId)
        {
            var userCollection = DatabaseConnect.UserCollection();
            var filter = Builders<User>.Filter.Eq(u => u.UserId, UserId);
            var dbuser = await userCollection.Find(filter).FirstOrDefaultAsync();

            if (dbuser != null)
            {
                List<int> usercart = dbuser.UserCart;
                if (!usercart.IsNullOrEmpty())
                {
                    var productCollection = DatabaseConnect.ProductCollection();
                    var productFilter = Builders<Product>.Filter.In(p => p.ProductId, usercart);
                    var products = await productCollection.Find(productFilter).ToListAsync();
                    return Ok(products);
                }
                return NotFound("No Items in cart");
            }

            return NotFound("User Doesn't Exist");
        }

        [HttpGet("Get_Products_from_History")]
        public async Task<ActionResult<List<Product>>> GetHistory(int UserId)
        {
            var userCollection = DatabaseConnect.UserCollection();
            var filter = Builders<User>.Filter.Eq(u => u.UserId, UserId);
            var dbuser = await userCollection.Find(filter).FirstOrDefaultAsync();

            if (dbuser != null)
            {
                List<int> userhistory = dbuser.UserHistory;
                if (!userhistory.IsNullOrEmpty())
                {
                    var productCollection = DatabaseConnect.ProductCollection();
                    var productFilter = Builders<Product>.Filter.In(p => p.ProductId, userhistory);
                    var products = await productCollection.Find(productFilter).ToListAsync();
                    return Ok(products);
                }
                return NotFound("No Items in History");
            }

            return NotFound("User Doesn't Exist");
        }
    }
}
