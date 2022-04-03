namespace backend
{
    public class Review
    {
        public int Id { get; set; }
        public int? AnalyticId { get; set; }
        public int? ArticleId { get; set; }
        public int? Level { get; set; }
        public string? Url { get; set; }
        public string? Comment { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}