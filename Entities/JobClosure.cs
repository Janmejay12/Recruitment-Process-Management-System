using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recruitment_System.Entities
{
    [Table("JobClosures")]
    public class JobClosure
    {
        [Key]
        public int ClosureId { get; set; }

        [Required]
        public int JobId { get; set; }

        [Required]
        [StringLength(50)]
        public string ClosureType { get; set; } = string.Empty; // Selected, Cancelled, BudgetCut, NoSuitableCandidates

        public int? SelectedCandidateId { get; set; }

        public DateTime? SelectionDate { get; set; }

        [Required]
        [StringLength(500)]
        public string ClosureReason { get; set; } = string.Empty;

        [Required]
        public int ClosedBy { get; set; }

        public DateTime ClosedAt { get; set; } = DateTime.UtcNow;

        [StringLength(1000)]
        public string? Notes { get; set; }

        // Navigation Properties
        [ForeignKey("JobId")]
        public virtual Job Job { get; set; } = null!;

        [ForeignKey("SelectedCandidateId")]
        public virtual Candidate? SelectedCandidate { get; set; }

        [ForeignKey("ClosedBy")]
        public virtual User ClosedByUser { get; set; } = null!;
    }
}