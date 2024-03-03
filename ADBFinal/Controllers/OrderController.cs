using ADBFinal.DataAccessLayer.DatabaseConnect;
using ADBFinal.DataAccessLayer.DTO;
using ADBFinal.DataAccessLayer.Entity;
using ADBFinal.DataAccessLayer.HttpRequests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;

namespace ADBFinal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : Controller
    {
        static Random random = new Random();

        [HttpGet("Get_User_Orders")]
        public async Task<IActionResult> GetUserOrders(int UserId)
        {
            var orderCollection = DatabaseConnect.OrderCollection();
            var filter = Builders<Order>.Filter.Eq(u => u.UserId, UserId);
            var orders = await orderCollection.Find(filter).ToListAsync();
            
            return Ok(orders);
        }

        [HttpGet("Specified_Order")]
        public async Task<IActionResult> GetSpecifiedOrder(int OrderId)
        {
            var orderCollection = DatabaseConnect.OrderCollection();
            var filter = Builders<Order>.Filter.Eq(o => o.OrderId, OrderId);
            var dborder = await orderCollection.Find(filter).FirstOrDefaultAsync();
            if (dborder == null)
            {
                return NotFound("Order not found");
            }

            var productCollection = DatabaseConnect.ProductCollection();
            var productFilter = Builders<Product>.Filter.In(p => p.ProductId, dborder.OrderProducts);
            var products = await productCollection.Find(productFilter).ToListAsync();

            var orderInfo = new OrderInfoDTO
            {
                OrderId = dborder.OrderId,
                Status = dborder.Status,
                Timestamp = dborder.Timestamp,
                Longitude = dborder.Longitude,
                Latitude = dborder.Latitude,
                Products = products
            };

            return Ok(orderInfo);
        }

        [HttpPost("Create_Order")]
        public async Task<IActionResult> CreateOrder(int UserId)
        {
            var userCollection = DatabaseConnect.UserCollection();
            var filter = Builders<User>.Filter.Eq(u => u.UserId, UserId);
            var dbuser = await userCollection.Find(filter).FirstOrDefaultAsync();
            if (dbuser != null)
            {
                List<int> usercart = dbuser.UserCart;
                if (!usercart.IsNullOrEmpty())
                {
                    List<int> PreparedCart = new List<int>();
                    PreparedCart = usercart;
                    var orderCollection = DatabaseConnect.OrderCollection();
                    var filterCount = Builders<Order>.Filter.Exists(o => o.OrderId);
                    var orderCount = DatabaseConnect.OrderCollection().CountDocuments(filterCount);
                    int orderNumber = Convert.ToInt32(orderCount + 1);
                    string defaultstatus = "Packaging";
                    double Longitude = GetRandomValue(71.0, 72.0, 3);
                    double Latitude = GetRandomValue(51.0, 51.5, 3);
                    var order = new Order(orderNumber, UserId, defaultstatus, Longitude, Latitude, PreparedCart, DateTime.UtcNow);
                    orderCollection.InsertOne(order);

                    dbuser.UserCart.Clear();
                    await userCollection.ReplaceOneAsync(filter, dbuser);

                    return Ok("New Order Insterted");
                }
                return BadRequest("No Items in cart");
            }
            return BadRequest("User Doesn't Exist");
        }

        static double GetRandomValue(double from, double to, int fixedDigits)
        {
            double range = to - from;
            double randomValue = random.NextDouble() * range + from;
            return Math.Round(randomValue, fixedDigits);
        }


        [HttpDelete("Delete_Order")]
        public async Task<IActionResult> DeleteOrder(int OrderId)
        {
            var orderCollection = DatabaseConnect.OrderCollection();
            var filter = Builders<Order>.Filter.Exists(o => o.OrderId);
            var dborder = orderCollection.Find(filter).FirstOrDefault();
            if (!(dborder == null))
            {
                orderCollection.DeleteOne(filter);
                return Ok("Order Deleted");
            }

            return BadRequest("Order Not Found");
        }
    }
}
