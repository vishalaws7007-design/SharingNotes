public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; } 
    // Counters
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    // Comments stored as JSON or plain text
    public string Comments { get; set; }
}
