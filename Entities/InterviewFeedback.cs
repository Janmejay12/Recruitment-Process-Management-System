using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recruitment_System.Entities
{
    [Table("InterviewFeedbacks")]
    public class InterviewFeedback
    {
        [Key]
        public int InterviewFeedbackId { get; set; }

        [Required]
        public int InterviewRoundId { get; set; }

        [Required]
        public int InterviewerUserId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [StringLength(20)]
        public string Recommendation { get; set; } = "Hold";
        // Hire | Reject | Hold

        [StringLength(2000)]
        public string? Comments { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey(nameof(InterviewRoundId))]
        public virtual InterviewRound InterviewRound { get; set; } = null!;

        [ForeignKey(nameof(InterviewerUserId))]
        public virtual User Interviewer { get; set; } = null!;
    }
}
