using System;

namespace RekaTalent.Models
{
    public class InterviewSchedule
    {
        public int Id { get; set; }
        public DateTime ScheduledDate { get; set; }

        public int InterviewId { get; set; }
        public Interview Interview { get; set; }
    }
}
