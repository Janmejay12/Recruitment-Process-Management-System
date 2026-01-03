using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recruitment_System.Entities
{
    [Table("CandidateReviewComments")]
    public class CandidateReviewComment
    {
        [Key]
        public int CommentId { get; set; }

        [Required]
        public int ReviewId { get; set; }

        [Required]
        [StringLength(2000)]
        public string CommentText { get; set; } = string.Empty;

        [Required]
        public int CommentedByUserId { get; set; }

        [Required]
        [StringLength(20)]
        public string RoleAtTime { get; set; } = string.Empty;
        // Recruiter | Reviewer | Interviewer

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey(nameof(ReviewId))]
        public virtual CandidateJobReview Review { get; set; } = null!;

        [ForeignKey(nameof(CommentedByUserId))]
        public virtual User CommentedByUser { get; set; } = null!;
    }
}
