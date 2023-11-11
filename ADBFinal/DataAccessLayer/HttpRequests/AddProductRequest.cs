namespace ADBFinal.DataAccessLayer.HttpRequests
{
    public class AddProductRequest
    {
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string ProductDescription { get; set; }
        public int ProductCategoryId { get; set; }
        public int ProductPrice { get; set; }
    }
}
