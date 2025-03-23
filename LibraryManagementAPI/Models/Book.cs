using System;
using System.Collections.Generic;

namespace LibraryManagementAPI.Models
{
    public class Book : EntityBase
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string CoverImage { get; set; }
        public string Publisher { get; set; }
        public DateTime PublicationDate { get; set; }
        public string Category { get; set; }
        public string ISBN { get; set; }
        public int PageCount { get; set; }
        public bool IsAvailable { get; set; } = true;

        // Navigation properties
        public ICollection<Review> Reviews { get; set; }
        public Checkout CurrentCheckout { get; set; }
    }
}