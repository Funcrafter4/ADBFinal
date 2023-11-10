using Microsoft.AspNetCore.Mvc;
using ADBFinal.DataAccessLayer.Entity;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MongoDB.Driver;
using ADBFinal.DataAccessLayer.DatabaseConnect;
using ADBFinal.DataAccessLayer.DTO;

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
        public async Task<ActionResult<User>> Register(String UserEmail, String UserPassword, String UserName)
        {
            var userCollection = DatabaseConnect.UserCollection();

            var filter = Builders<User>.Filter.Eq(u => u.UserEmail, UserEmail);
            var dbuser = userCollection.Find(filter).FirstOrDefault();
           if (dbuser == null)
            {
                var filterCount = Builders<User>.Filter.Exists(u => u.UserId);
                var userCount = DatabaseConnect.UserCollection().CountDocuments(filterCount);
                int userNumber = Convert.ToInt32(userCount + 1);
                CreatePasswordHash(UserPassword, out byte[] UserPasswordHash, out byte[] UserPasswordSalt);
                dbuser = new User(userNumber, UserName, UserEmail, UserPasswordHash, UserPasswordSalt);
                userCollection.InsertOne(dbuser);
                return Ok("New User Inserted");
            } 
            return BadRequest("User Exists");
        }

        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login(String UserEmail, String UserPassword)
        {
            var userCollection = DatabaseConnect.UserCollection();
            var filter = Builders<User>.Filter.Eq(u => u.UserEmail, UserEmail);
            var dbuser = userCollection.Find(filter).FirstOrDefault();

            if (dbuser == null)
            {
                return BadRequest("User not found");
            }

            if (!VerifyPasswordHash(UserPassword, dbuser.UserPasswordHash, dbuser.UserPasswordSalt))
            {
                return BadRequest("Wrong Password");
            }

            string token = CreateToken(dbuser);

            return Ok(token);

        }

        [HttpGet("GetId")]
        public async Task<ActionResult<int>> GetId(String JWT)
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
            var userDTO = new UserDTO(dbuser.UserId, dbuser.UserName, dbuser.UserEmail, dbuser.UserHistory, dbuser.UserWishlist, dbuser.UserCart);

            return Ok(userDTO);
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


            return BadRequest("User Doesn't Exist");
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
