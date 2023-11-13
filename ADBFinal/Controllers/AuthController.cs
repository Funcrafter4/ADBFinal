using Microsoft.AspNetCore.Mvc;
using ADBFinal.DataAccessLayer.Entity;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MongoDB.Driver;
using ADBFinal.DataAccessLayer.DatabaseConnect;
using ADBFinal.DataAccessLayer.DTO;
using ADBFinal.DataAccessLayer.HttpRequests;
namespace ADBFinal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;
        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        [HttpPost("Register")]
        public Task<ActionResult<User>> Register(RegisterRequest myJsonResponse)
        {

            var userCollection = DatabaseConnect.UserCollection();

            var filter = Builders<User>.Filter.Eq(u => u.UserEmail, myJsonResponse.UserEmail);
            var dbuser = userCollection.Find(filter).FirstOrDefault();
           if (dbuser == null)
            {
                var filterCount = Builders<User>.Filter.Exists(u => u.UserId);
                var userCount = DatabaseConnect.UserCollection().CountDocuments(filterCount);
                int userNumber = Convert.ToInt32(userCount + 1);
                CreatePasswordHash(myJsonResponse.UserPassword, out byte[] UserPasswordHash, out byte[] UserPasswordSalt);
                dbuser = new User(userNumber, myJsonResponse.UserName, myJsonResponse.UserEmail, UserPasswordHash, UserPasswordSalt);
                userCollection.InsertOne(dbuser);
                return Task.FromResult<ActionResult<User>>(Ok("New User Inserted"));
            } 
            return Task.FromResult<ActionResult<User>>(NotFound("User Exists"));
        }

        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login(LoginRequest myJsonRespone)
        {
            var userCollection = DatabaseConnect.UserCollection();
            var filter = Builders<User>.Filter.Eq(u => u.UserEmail, myJsonRespone.UserEmail);
            var dbuser = userCollection.Find(filter).FirstOrDefault();

            if (dbuser == null)
            {
                return NotFound("User not found");
            }

            if (!VerifyPasswordHash(myJsonRespone.UserPassword, dbuser.UserPasswordHash, dbuser.UserPasswordSalt))
            {
                return BadRequest("Wrong Password");
            }

            string token = CreateToken(dbuser);

            return Ok(token);

        }

        [HttpGet("GetId")]
        public async Task<ActionResult<int>> GetId(string JWT)
        {
            string jwt = JWT;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(jwt);
            var tokenS = jsonToken as JwtSecurityToken;
            var jti = tokenS.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            int userid = int.Parse(jti);

            return Ok(userid);
        }


        [HttpGet("GetUser")]

        public async Task<ActionResult<string>> GetUser(int UserId)
        {
            var userCollection = DatabaseConnect.UserCollection();
            var filter = Builders<User>.Filter.Eq(u => u.UserId, UserId);
            var dbuser = userCollection.Find(filter).FirstOrDefault();
            if (dbuser != null)
            {
                var userDTO = new UserDTO(dbuser.UserId, dbuser.UserName, dbuser.UserEmail, dbuser.UserHistory, dbuser.UserWishlist, dbuser.UserCart); 
                
                return Ok(userDTO);
            }
            return NotFound("User Doesn't Exist");
        }

        [HttpPost("DeleteUser")]
        public async Task<ActionResult<string>> DeleteUser(int UserId)
        {
            var userCollection = DatabaseConnect.UserCollection();

            var filter = Builders<User>.Filter.Eq(u => u.UserId, UserId);
            var dbuser = userCollection.Find(filter).FirstOrDefault();
            if (dbuser != null)
            {
                userCollection.DeleteOne(filter);
                return Ok("User Deleted");
            }


            return NotFound("User Doesn't Exist");
        }

        [HttpPut("Admin_Create_Admin")]
        public async Task<ActionResult<string>> AdminCreateAdmin(CreateAdminRequest myJsonResponse)
        {
            var userCollection = DatabaseConnect.UserCollection();

            var isadminfilter = Builders<User>.Filter.Eq(u => u.UserId, myJsonResponse.AdminId);
            var dbadmin = userCollection.Find(isadminfilter).FirstOrDefault();
            if (dbadmin.IsAdmin)
            {
                var filter = Builders<User>.Filter.Eq(u => u.UserId, myJsonResponse.NewAdminId);
                var dbuser = userCollection.Find(filter).FirstOrDefault();
                if(dbuser != null)
                {
                    var update = Builders<User>.Update.Set(u => u.IsAdmin, true);
                    var updateResult = await userCollection.UpdateOneAsync(filter, update);
                    if (updateResult.ModifiedCount > 0)
                    {
                        return Ok("User promoted to admin successfully");
                    }
                    else
                    {
                        return BadRequest("Failed to update user");
                    }
                }
                return NotFound("User to promote doesn't exist");

            }
            return Unauthorized("Only admins can create admins");
        }


        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
    {
        new Claim(ClaimTypes.Email, user.UserEmail),
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
    };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            // Use HMACSHA1 instead of HmacSha512Signature
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }


        private void CreatePasswordHash(string UserPassword, out byte[] UserPasswordHash, out byte[] UserPasswordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                UserPasswordSalt = hmac.Key;
                UserPasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(UserPassword));
            }
        }

        private bool VerifyPasswordHash(string UserPassword, byte[] UserPasswordHash, byte[] UserPasswordSalt)
        {
            using (var hmac = new HMACSHA512(UserPasswordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(UserPassword));
                return computedHash.SequenceEqual(UserPasswordHash);
            }
        }



    }
}
