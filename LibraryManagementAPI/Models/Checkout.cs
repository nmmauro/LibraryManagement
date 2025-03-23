namespace LibraryManagementAPI.Models
{
    public class Checkout : EntityBase
    {
        public DateTime CheckoutDate { get; set; } = DateTime.UtcNow;
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        // Foreign keys
        public int BookId { get; set; }
        public string UserId { get; set; }

        // Navigation properties
        public Book Book { get; set; }
        public ApplicationUser User { get; set; }
    }
}