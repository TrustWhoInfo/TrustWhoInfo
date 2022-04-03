namespace backend
{
    public class Article
    {
        public int Id { get; set; }
        public int? MediaId { get; set; }
        public int? AuthorId { get; set; }
        public string Url { get; set; }
        public string? Name { get; set; }
        public string? Subject { get; set; }
        public string? Lang { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}