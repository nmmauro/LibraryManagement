namespace LibraryManagementAPI.DTOs
{
    public class CheckoutDTO
    {
        public int Id { get; set; }
        public DateTime CheckoutDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Username { get; set; }
    }
}