using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recruitment_System.Entities
{
    [Table("InterviewProcesses")]
    public class InterviewProcess
    {
        [Key]
        public int InterviewProcessId { get; set; }

        [Required]
        public int ReviewId { get; set; } // FK to CandidateJobReview

        [Required]
        public int TotalRounds { get; set; }

        [Required]
        public int CurrentRound { get; set; } = 1;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "NotStarted";
        // NotStarted | InProgress | Completed | Rejected | Selected

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey(nameof(ReviewId))]
        public virtual CandidateJobReview Review { get; set; } = null!;

        public virtual ICollection<InterviewRound> Rounds { get; set; } = new List<InterviewRound>();
    }
}
