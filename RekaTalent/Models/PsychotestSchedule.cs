using RekaTalent.Models;

public class PsychotestSchedule
{
    public int Id { get; set; }
    public DateTime ScheduledDate { get; set; }
    public int PsychotestId { get; set; }
    public Psychotest Psychotest { get; set; }
}