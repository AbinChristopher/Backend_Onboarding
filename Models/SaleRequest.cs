namespace app.Server.Models
{
    public class SaleRequest
    {
        public DateTime DateSold { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public int StoreId { get; set; }
    }
}