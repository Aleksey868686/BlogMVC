namespace Blog.Models
{
    public class PostTag
    {
        public Guid Id { get; set; }
        public Guid TagId { get; set; }
        public Guid PostId { get; set; }

        // Navigation properties
        public Post Post { get; set; }
        public Tag Tag { get; set; }
    }
}
