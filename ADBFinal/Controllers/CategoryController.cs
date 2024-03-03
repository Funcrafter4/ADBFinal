using ADBFinal.DataAccessLayer.DatabaseConnect;
using ADBFinal.DataAccessLayer.Entity;
using ADBFinal.DataAccessLayer.HttpRequests;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace ADBFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        [HttpGet("Get_All_Categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categoryCollection = DatabaseConnect.CategoryCollection();
            var filter = Builders<Category>.Filter.Empty;
            var categories = await categoryCollection.Find(filter).ToListAsync();

            return Ok(categories);
        }

        [HttpGet("Get_Category")]
        public async Task<IActionResult> GetCategory(int CategoryId)
        {
            var categoryCollection = DatabaseConnect.CategoryCollection();
            var filter = Builders<Category>.Filter.Eq(c => c.CategoryId, CategoryId);
            var dbcategory = categoryCollection.Find(filter).FirstOrDefault();
            if (dbcategory == null)
            {
                return BadRequest("Category Not Found");
            }

            return Ok(dbcategory);
        }

        [HttpPost("Add_Category")]
        public async Task<IActionResult> AddCategory(AddCategoryRequest myJsonResponse)
        {
            var categoryCollection = DatabaseConnect.CategoryCollection();
            var filterCount = Builders<Category>.Filter.Exists(c => c.CategoryId);
            var categoryCount = DatabaseConnect.CategoryCollection().CountDocuments(filterCount);
            int categoryNumber = Convert.ToInt32(categoryCount + 1);
            var category = new Category(categoryNumber, myJsonResponse.CategoryName, myJsonResponse.CategoryDescription);
            categoryCollection.InsertOne(category);

            return Ok("New Category Insterted");
        }


        [HttpDelete("Delete_Category")]
        public async Task<IActionResult> DeleteCategory(int CategoryId)
        {
            var categoryCollection = DatabaseConnect.CategoryCollection();
            var filter = Builders<Category>.Filter.Eq(c => c.CategoryId, CategoryId);
            var dbcategory = categoryCollection.Find(filter).FirstOrDefault();
            if (!(dbcategory == null))
            {
                categoryCollection.DeleteOne(filter);
                return Ok("Category Deleted");
            }

            return BadRequest("Category Not Found");
        }
    }
}
