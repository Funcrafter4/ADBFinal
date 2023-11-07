using Microsoft.AspNetCore.Mvc;
using ADBFinal.DataAccessLayer.Entity;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MongoDB.Driver;
using ADBFinal.DataAccessLayer.DatabaseConnect;

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
        
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(String UserEmail, String UserPassword, String UserName)
        {
            var userCollection = DatabaseConnect.UserCollection();

            var filter = Builders<User>.Filter.Eq(u => u.UserEmail, UserEmail);
            var dbuser = userCollection.Find(filter).FirstOrDefault();
           if (dbuser == null)
            {
                CreatePasswordHash(UserPassword, out byte[] UserPasswordHash, out byte[] UserPasswordSalt);
                dbuser = new User(UserName, UserEmail, UserPasswordHash, UserPasswordSalt);
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
