namespace ADBFinal.DataAccessLayer.Entity
{
    public class Category
    {
        public Category(string categoryName, string categoryDescription)
        {
            CategoryName = categoryName;
            CategoryDescription = categoryDescription;

        }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string CategoryDescription { get; set; }

    }
}
