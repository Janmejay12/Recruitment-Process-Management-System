using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recruitment_System.Entities
{
    [Table("CandidateSkillEvaluations")]
    public class CandidateSkillEvaluation
    {
        [Key]
        public int EvaluationId { get; set; }

        [Required]
        public int ReviewId { get; set; }

        [Required]
        public int SkillId { get; set; }

        [Range(0, 50)]
        public int YearsExperience { get; set; }

        public bool IsVerified { get; set; } = true;

        [Required]
        public int VerifiedByUserId { get; set; }

        // Navigation
        [ForeignKey(nameof(ReviewId))]
        public virtual CandidateJobReview Review { get; set; } = null!;

        [ForeignKey(nameof(SkillId))]
        public virtual Skill Skill { get; set; } = null!;

        [ForeignKey(nameof(VerifiedByUserId))]
        public virtual User VerifiedByUser { get; set; } = null!;
    }
}
