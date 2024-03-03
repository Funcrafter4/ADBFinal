using ADBFinal.DataAccessLayer.DatabaseConnect;
using ADBFinal.DataAccessLayer.DTO;
using ADBFinal.DataAccessLayer.Entity;
using ADBFinal.DataAccessLayer.HttpRequests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

namespace ADBFinal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {

        [HttpPatch("Add_to_Users_Wishlist")]
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
            return BadRequest("User Doesn't Exist");  
        }

        [HttpPatch("Add_to_Users_History")]
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
            return BadRequest("User Doesn't Exist");
        }

        [HttpPatch("Add_to_Users_Cart")]
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
            return BadRequest("User Doesn't Exist");
        }

        [HttpDelete("Delete_from_Users_Wishlist")]
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
                return BadRequest("No such item in wishlist");
            }
            return BadRequest("User Doesn't Exist");
        }

        [HttpDelete("Delete_from_Users_Cart")]
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
                return BadRequest("No such item in Cart");
            }
            return BadRequest("User Doesn't Exist");
        }

        [HttpDelete("Delete_from_Users_History")]
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
                return BadRequest("No such item in History");
            }
            return BadRequest("User Doesn't Exist");
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
                return BadRequest("No Items in wishlist");
            }

            return BadRequest("User Doesn't Exist");
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
                return BadRequest("No Items in cart");
            }

            return BadRequest("User Doesn't Exist");
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
                return BadRequest("No Items in History");
            }

            return BadRequest("User Doesn't Exist");
        }

        [HttpGet("Get_Recomendations_based_on_History")]
        public async Task<ActionResult<List<Product>>> GetRecomendations(int UserId)
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

                    // Step 1: Get ProductCategoryId from user history
                    var productFilter = Builders<Product>.Filter.In(p => p.ProductId, userhistory);
                    var products = await productCollection.Find(productFilter).ToListAsync();
                    var categoryIds = products.Select(p => p.ProductCategoryID).Distinct().ToList();

                    if (categoryIds.IsNullOrEmpty())
                    {
                        return BadRequest("No Items in History");
                    }

                    // Step 2: Get 5 random products with the same category
                    var random = new Random();
                    var recommendedProducts = new List<Product>();

                    foreach (var categoryId in categoryIds)
                    {
                        var categoryFilter = Builders<Product>.Filter.Eq(p => p.ProductCategoryID, categoryId);
                        var categoryProducts = await productCollection.Find(categoryFilter).ToListAsync();

                        if (categoryProducts.Count > 0)
                        {
                            var randomProducts = categoryProducts.OrderBy(_ => random.Next()).Take(5);
                            recommendedProducts.AddRange(randomProducts);
                        }
                    }
                    var finalRecommendations = recommendedProducts.OrderBy(_ => random.Next()).Take(5).ToList();

                    return Ok(finalRecommendations);
                }

                return BadRequest("No Items in History");
            }

            return BadRequest("User Doesn't Exist");
        }

        [HttpGet("Get_Users_Likes")]
        public async Task<ActionResult<List<int>>> GetUsersLikes(int UserId)
        {
            var userCollection = DatabaseConnect.UserCollection();
            var filter = Builders<User>.Filter.Eq(u => u.UserId, UserId);
            var dbuser = await userCollection.Find(filter).FirstOrDefaultAsync();

            if (dbuser != null)
            {
                List<int> userwishlist = dbuser.UserWishlist;
                if (!userwishlist.IsNullOrEmpty())
                {
                    // Find other users with wishlists containing at least one common product ID
                    var filterOtherUsers = Builders<User>.Filter.And(
                        Builders<User>.Filter.Ne(u => u.UserId, UserId), // Exclude the current user
                        Builders<User>.Filter.AnyIn(u => u.UserWishlist, userwishlist) // Wishlist contains at least one common product ID
                    );

                    var otherUsers = await userCollection.Find(filterOtherUsers).ToListAsync();

                    // Extract the wishlists from other users
                    List<List<int>> otherWishlists = otherUsers.Select(u => u.UserWishlist).ToList();

                    // Exclude product IDs of the initial user from the combined product IDs of other users' wishlists
                    List<int> otherUserProductIds = otherWishlists.SelectMany(ids => ids).Except(userwishlist).ToList();

                    var productCollection = DatabaseConnect.ProductCollection();

                    var productFilter = Builders<Product>.Filter.In(p => p.ProductId, otherUserProductIds);
                    var products = await productCollection.Find(productFilter).ToListAsync();

                    var random = new Random();
                    var userlikesProducts = products.OrderBy(_ => random.Next()).Take(5).ToList();

                    if (!userlikesProducts.IsNullOrEmpty())
                    {
                        return Ok(userlikesProducts);
                    }
                    else
                    {
                        return BadRequest("No other products found");
                    }

                }
                else
                {
                    return BadRequest("No Items in Wishlist");
                }


            }

            return BadRequest("User Doesn't Exist");
        }

    }
}
