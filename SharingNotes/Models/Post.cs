public class Post
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Title { get; set; }
    public string? Description { get; set; }
    // Counters
    public string Createdon { get; set; } = DateTime.Now.ToString("dd-MMM-yyyy:hh:mm");
    public string Modifedon { get; set; } = DateTime.Now.ToString("dd-MMM-yyyy:hh:mm");
    public string? userName  { get; set; }
    public int ViewCount { get; set; } = 0;
    public int LikeCount { get; set; } = 0;
    // Comments stored as JSON or plain text
    public string? Comments { get; set; }=string.Empty;
}
