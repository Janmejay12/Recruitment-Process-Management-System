using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recruitment_System.Entities
{
    [Table("CandidateSkills")]
    public class CandidateSkill
    {
        [Key]
        public int CandidateSkillId { get; set; }

        [Required]
        public int CandidateId { get; set; }

        [Required]
        public int SkillId { get; set; }

        [Range(0, 50)]
        public int YearsExperience { get; set; }

        [StringLength(20)]
        public string? ProficiencyLevel { get; set; } // Beginner, Intermediate, Advanced, Expert

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("CandidateId")]
        public virtual Candidate Candidate { get; set; } = null!;

        [ForeignKey("SkillId")]
        public virtual Skill Skill { get; set; } = null!;
    }
}
