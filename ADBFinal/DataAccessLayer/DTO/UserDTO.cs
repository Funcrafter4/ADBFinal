namespace ADBFinal.DataAccessLayer.DTO
{
    public class UserDTO
    {
        public UserDTO(int userId, string userName, string userEmail, List<int> userHistory, List<int> userWishlist, List<int> userCart)
        {
            UserId = userId;
            UserName = userName;
            UserEmail = userEmail;
            UserHistory = userHistory;
            UserWishlist = userWishlist;
            UserCart = userCart;
        }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public string UserEmail { get; set; }

        public List<int> UserHistory { get; set; } = new List<int>();

        public List<int> UserWishlist { get; set; } = new List<int>();

        public List<int> UserCart { get; set; } = new List<int>();
    }
}
