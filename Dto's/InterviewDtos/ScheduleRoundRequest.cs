using System.ComponentModel.DataAnnotations;

namespace Recruitment_System.Dto_s.InterviewDtos
{
    public class ScheduleRoundRequest
    {
        [Required]
        public DateTime ScheduledAt { get; set; }

        [Required]
        [StringLength(20)]
        public string Mode { get; set; } = "Online"; // Online | Offline

        [StringLength(500)]
        public string? MeetingLinkOrLocation { get; set; }
    }
}
