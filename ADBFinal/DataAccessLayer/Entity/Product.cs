namespace ADBFinal.DataAccessLayer.Entity
{
    public class Product
    {
        public Product(string productName, string productDescription, int productCategoryID)
        {
            ProductName = productName;
            ProductDescription = productDescription;
            ProductCategoryID = productCategoryID;
        }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductImage {  get; set; }

        public string ProductDescription {  get; set; }

        public int ProductCategoryID { get; set; }


    }
}
