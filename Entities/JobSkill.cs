using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recruitment_System.Entities
{
    [Table("JobSkills")]
    public class JobSkill
    {
        [Key]
        public int JobSkillId { get; set; }

        [Required]
        public int JobId { get; set; }

        [Required]
        public int SkillId { get; set; }

        [Required]
        public bool IsMandatory { get; set; } = true;

        [Required]
        public int Priority { get; set; } = 1; // 1=High, 2=Medium, 3=Low

        [StringLength(500)]
        public string? Notes { get; set; }

        // Navigation Properties
        [ForeignKey("JobId")]
        public virtual Job Job { get; set; } = null!;

        [ForeignKey("SkillId")]
        public virtual Skill Skill { get; set; } = null!;
    }
}