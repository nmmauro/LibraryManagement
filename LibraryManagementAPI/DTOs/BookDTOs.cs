namespace LibraryManagementAPI.DTOs
{
    public class BookDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string CoverImage { get; set; }
        public string Publisher { get; set; }
        public DateTime PublicationDate { get; set; }
        public string Category { get; set; }
        public string ISBN { get; set; }
        public int PageCount { get; set; }
        public bool IsAvailable { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }

    public class BookCreateDTO
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
    }

    public class BookUpdateDTO
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
    }

    public class BookDetailDTO : BookDTO
    {
        public List<ReviewDTO> Reviews { get; set; } = new List<ReviewDTO>();
        public CheckoutDTO CurrentCheckout { get; set; }
    }
}