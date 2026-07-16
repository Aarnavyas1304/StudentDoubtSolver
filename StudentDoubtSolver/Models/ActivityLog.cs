public class ActivityLog
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime ActivityDate { get; set; }
    public string ActivityType { get; set; } // e.g., "Question", "Answer"
}