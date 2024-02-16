using ADBFinal.DataAccessLayer.DatabaseConnect;
using ADBFinal.DataAccessLayer.DTO;
using ADBFinal.DataAccessLayer.Entity;
using ADBFinal.DataAccessLayer.HttpRequests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace ADBFinal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : Controller
    {
        [HttpPost("AddUser")]
        public Task<ActionResult<User>> AddUser(RegisterRequest myJsonResponse)
        {
            bool validpass = false;
            bool validemail = false;
            var userCollection = DatabaseConnect.UserCollection();

            var filter = Builders<User>.Filter.Eq(u => u.UserEmail, myJsonResponse.UserEmail);
            var dbuser = userCollection.Find(filter).FirstOrDefault();
            if (dbuser == null)
            {
                var filterCount = Builders<User>.Filter.Exists(u => u.UserId);
                var userCount = DatabaseConnect.UserCollection().CountDocuments(filterCount);
                int userNumber = Convert.ToInt32(userCount + 1);
                if (IsPasswordValid(myJsonResponse.UserPassword))
                {
                    validpass = true;

                }
                else
                {
                    return Task.FromResult<ActionResult<User>>(BadRequest("Password must contain 8 symbols"));
                }
                if (IsValidEmail(myJsonResponse.UserEmail))
                {
                    validemail = true;
                }
                else
                {
                    return Task.FromResult<ActionResult<User>>(BadRequest("Write valid email"));
                }
                if (validemail && validpass)
                {
                    CreatePasswordHash(myJsonResponse.UserPassword, out byte[] UserPasswordHash, out byte[] UserPasswordSalt);
                    dbuser = new User(userNumber, myJsonResponse.UserName, myJsonResponse.UserEmail, UserPasswordHash, UserPasswordSalt);
                    userCollection.InsertOne(dbuser);
                    return Task.FromResult<ActionResult<User>>(Ok("New User Inserted"));
                }
                else
                {
                    return Task.FromResult<ActionResult<User>>(BadRequest("Password or Email is not valid"));
                }
            }
            return Task.FromResult<ActionResult<User>>(BadRequest("User Exists"));
        }

        private void CreatePasswordHash(string UserPassword, out byte[] UserPasswordHash, out byte[] UserPasswordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                UserPasswordSalt = hmac.Key;
                UserPasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(UserPassword));
            }
        }

        static bool IsPasswordValid(string password)
        {
            // Check if the password has at least 8 characters
            return password.Length >= 8;
        }

        static bool IsValidEmail(string email)
        {
            // Define a regular expression for a simple email validation
            string pattern = @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";

            Regex regex = new Regex(pattern);

            return regex.IsMatch(email);
        }


        [HttpGet("Get_All_Users")]
        public async Task<IActionResult> GetUsers()
        {
            var userCollection = DatabaseConnect.UserCollection();
            var filter = Builders<User>.Filter.Empty;
            var users = await userCollection.Find(filter).ToListAsync();

            return Ok(users);
        }

        [HttpDelete("Delete_User")]
        public async Task<IActionResult> DeleteUser(int UserId)
        {
            var userCollection = DatabaseConnect.UserCollection();
            var filter = Builders<User>.Filter.Eq(u => u.UserId, UserId);
            var dbproduct = userCollection.Find(filter).FirstOrDefault();
            if (!(dbproduct == null))
            {
                userCollection.DeleteOne(filter);
                return Ok("User Deleted");
            }

            return BadRequest("User Not Found");
        }

        [HttpPatch("Patch_User")]
        public async Task<IActionResult> PatchUser(int UserId, [FromBody] UserUpdateModel updatedUser)
        {
            var userCollection = DatabaseConnect.UserCollection();
            var filter = Builders<User>.Filter.Eq(u => u.UserId, UserId);
            var dbUser = await userCollection.Find(filter).FirstOrDefaultAsync();

            if (dbUser == null)
            {
                return BadRequest("User Not Found");
            }

            var update = Builders<User>.Update
                .Set(u => u.UserName, updatedUser.UserName)
                .Set(u => u.UserEmail, updatedUser.Email);

            var updateResult = await userCollection.UpdateOneAsync(filter, update);

            if (updateResult.ModifiedCount > 0)
            {
                return Ok("User Updated");
            }

            return BadRequest("Failed to update user");
        }

        public class UserUpdateModel
        {
            public string UserName { get; set; }
            public string Email { get; set; }
        }

        [HttpPut("Add_Product_To_Wishlist")]
        public async Task<IActionResult> AddProductToWishlist(int productId)
        {
            var userCollection = DatabaseConnect.UserCollection();
            var filter = Builders<User>.Filter.Empty;
            var update = Builders<User>.Update.Push(u => u.UserWishlist, productId);

            var updateResult = await userCollection.UpdateManyAsync(filter, update);

            if (updateResult.ModifiedCount > 0)
            {
                return Ok($"Product with ID {productId} added to wishlist of all users");
            }

            return BadRequest("Failed to add product to wishlist");
        }
    }
}
