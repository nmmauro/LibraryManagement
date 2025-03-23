using System;

namespace LibraryManagementAPI.Models
{
    public class Review : EntityBase
    {
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign keys
        public int BookId { get; set; }
        public string UserId { get; set; }

        // Navigation properties
        public Book Book { get; set; }
        public ApplicationUser User { get; set; }
    }
}