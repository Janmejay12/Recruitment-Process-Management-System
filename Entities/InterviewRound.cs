using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recruitment_System.Entities
{
    [Table("InterviewRounds")]
    public class InterviewRound
    {
        [Key]
        public int InterviewRoundId { get; set; }

        [Required]
        public int InterviewProcessId { get; set; }

        [Required]
        public int RoundNumber { get; set; } // 1, 2, 3...

        [Required]
        [StringLength(20)]
        public string RoundType { get; set; } = "Technical";
        // Technical | HR

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Scheduled";
        // Scheduled | Completed | Cancelled

        public DateTime? ScheduledAt { get; set; }

        [StringLength(20)]
        public string Mode { get; set; } = "Online"; // Online | Offline

        [StringLength(500)]
        public string? MeetingLinkOrLocation { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey(nameof(InterviewProcessId))]
        public virtual InterviewProcess InterviewProcess { get; set; } = null!;

        public virtual ICollection<InterviewPanelMember> PanelMembers { get; set; } = new List<InterviewPanelMember>();
        public virtual ICollection<InterviewFeedback> Feedbacks { get; set; } = new List<InterviewFeedback>();
    }
}
