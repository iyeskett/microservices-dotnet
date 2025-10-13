namespace GeekShopping.Web.Models
{
    public class CartDetail
    {
        public long CartHeaderId { get; set; }

        public CartHeader CartHeader { get; set; }

        public long ProductId { get; set; }

        public Product Product { get; set; }

        public int Count { get; set; }
    }
}