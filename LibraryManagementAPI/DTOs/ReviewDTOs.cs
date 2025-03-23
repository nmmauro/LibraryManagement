namespace LibraryManagementAPI.DTOs
{
    public class ReviewDTO
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Username { get; set; }
    }

    public class ReviewCreateDTO
    {
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}