using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace LibraryManagementAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Custom properties if needed
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Checkout> Checkouts { get; set; }
    }
}