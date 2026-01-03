using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recruitment_System.Entities
{
    [Table("CandidateJobReviews")]
    public class CandidateJobReview
    {
        [Key]
        public int ReviewId { get; set; }

        [Required]
        public int CandidateId { get; set; }

        [Required]
        public int JobId { get; set; }

        [Required]
        [StringLength(20)]
        public string CurrentStage { get; set; } = "Screening";
        // Screening | Shortlisted | Interview | Rejected

        public int? AssignedReviewerId { get; set; }
        public int? AssignedInterviewerId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey(nameof(CandidateId))]
        public virtual Candidate Candidate { get; set; } = null!;

        [ForeignKey(nameof(JobId))]
        public virtual Job Job { get; set; } = null!;

        [ForeignKey(nameof(AssignedReviewerId))]
        public virtual User? AssignedReviewer { get; set; }

        [ForeignKey(nameof(AssignedInterviewerId))]
        public virtual User? AssignedInterviewer { get; set; }

        public virtual ICollection<CandidateReviewComment> Comments { get; set; }
            = new List<CandidateReviewComment>();

        public virtual ICollection<CandidateSkillEvaluation> SkillEvaluations { get; set; }
            = new List<CandidateSkillEvaluation>();
    }
}
