using ADBFinal.DataAccessLayer.DatabaseConnect;
using ADBFinal.DataAccessLayer.Entity;
using ADBFinal.DataAccessLayer.HttpRequests;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ADBFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {


        [HttpGet("Get_All_Products")]
        public async Task<IActionResult> GetProducts()
        {
            var productCollection = DatabaseConnect.ProductCollection();
            var filter = Builders<Product>.Filter.Empty;
            var products = await productCollection.Find(filter).ToListAsync();

            return Ok(products);
        }

        /*[HttpGet("Search_by_Category_and_Name")]
        public async Task<IActionResult> Search([FromQuery]SearchRequest myJsonResponse)
        {
            var productCollection = DatabaseConnect.ProductCollection();
            if (myJsonResponse.ProductCategoryId == null && !(myJsonResponse.ProductName == null)) {
                var nameFilter = Builders<Product>.Filter.Regex(p => p.ProductName, new BsonRegularExpression(myJsonResponse.ProductName, "i"));
                var products = await productCollection.Find(nameFilter).ToListAsync();
                return Ok(products);
            } else if (!(myJsonResponse.ProductCategoryId == null) && myJsonResponse.ProductName == null)
            {
                var ProductCategoryIdInt = Convert.ToInt32(myJsonResponse.ProductCategoryId);
                var categoryFilter = Builders<Product>.Filter.Eq(p => p.ProductCategoryID, ProductCategoryIdInt);
                var products = await productCollection.Find(categoryFilter).ToListAsync();
                return Ok(products);
            } else if (!(myJsonResponse.ProductCategoryId == null) && !(myJsonResponse.ProductName == null)) {
                var nameFilter = Builders<Product>.Filter.Regex(p => p.ProductName, new BsonRegularExpression(myJsonResponse.ProductName, "i"));
                var ProductCategoryIdInt = Convert.ToInt32(myJsonResponse.ProductCategoryId);
                var categoryFilter = Builders<Product>.Filter.Eq(p => p.ProductCategoryID, ProductCategoryIdInt);
                var combinedFilter = Builders<Product>.Filter.And(categoryFilter, nameFilter);
                var products = await productCollection.Find(combinedFilter).ToListAsync();
                return Ok(products);
            } else { return BadRequest(); }
        }*/


        [HttpGet("Search_by_Category_and_Name")]
        public async Task<IActionResult> Search(string? ProductCategoryId, string? ProductName)
        {
            var productCollection = DatabaseConnect.ProductCollection();
            if (ProductCategoryId == null && !(ProductName == null))
            {
                var nameFilter = Builders<Product>.Filter.Regex(p => p.ProductName, new BsonRegularExpression(ProductName, "i"));
                var products = await productCollection.Find(nameFilter).ToListAsync();
                return Ok(products);
            }
            else if (!(ProductCategoryId == null) && ProductName == null)
            {
                var ProductCategoryIdInt = Convert.ToInt32(ProductCategoryId);
                var categoryFilter = Builders<Product>.Filter.Eq(p => p.ProductCategoryID, ProductCategoryIdInt);
                var products = await productCollection.Find(categoryFilter).ToListAsync();
                return Ok(products);
            }
            else if (!(ProductCategoryId == null) && !(ProductName == null))
            {
                var nameFilter = Builders<Product>.Filter.Regex(p => p.ProductName, new BsonRegularExpression(ProductName, "i"));
                var ProductCategoryIdInt = Convert.ToInt32(ProductCategoryId);
                var categoryFilter = Builders<Product>.Filter.Eq(p => p.ProductCategoryID, ProductCategoryIdInt);
                var combinedFilter = Builders<Product>.Filter.And(categoryFilter, nameFilter);
                var products = await productCollection.Find(combinedFilter).ToListAsync();
                return Ok(products);
            }
            else { return BadRequest(); }
        }


        [HttpPost("Add_Product")]
        public async Task<IActionResult> AddProduct(AddProductRequest myJsonResponse)
        {
            var productCollection = DatabaseConnect.ProductCollection();
            var filterCount = Builders<Product>.Filter.Exists(p => p.ProductId);
            var productCount = DatabaseConnect.ProductCollection().CountDocuments(filterCount);
            int productNumber = Convert.ToInt32(productCount + 1); 
            var product = new Product(productNumber, myJsonResponse.ProductName,myJsonResponse.ProductDescription,myJsonResponse.ProductCategoryId,myJsonResponse.ProductPrice);
            productCollection.InsertOne(product);

            return Ok("New Product Insterted");
        }

        [HttpGet("Get_Product")]
        public async Task<IActionResult> GetProduct(int ProductId)
        {
            var productCollection = DatabaseConnect.ProductCollection();
            var filter = Builders<Product>.Filter.Eq(p => p.ProductId, ProductId);
            var dbproduct = productCollection.Find(filter).FirstOrDefault();
            if (dbproduct == null)
            {
                return NotFound("Product Not Found");
            }

            return Ok(dbproduct);
        }

        [HttpGet("Delete_Product")]
        public async Task<IActionResult> DeleteProduct(int ProductId)
        {
            var productCollection = DatabaseConnect.ProductCollection();
            var filter = Builders<Product>.Filter.Eq(p => p.ProductId, ProductId);
            var dbproduct = productCollection.Find(filter).FirstOrDefault();
            if (!(dbproduct == null))
            {
                productCollection.DeleteOne(filter);
                return Ok("Product Deleted");
            }

            return NotFound("Product Not Found");
        }
    }
}
