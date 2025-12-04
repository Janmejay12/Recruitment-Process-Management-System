using System.ComponentModel.DataAnnotations;

namespace Recruitment_System.Dto_s.CandidateDtos
{
    public class CandidateSkillRequest
    {
        [Required(ErrorMessage = "Skill ID is required")]
        public int SkillId { get; set; }

        [Range(0, 50, ErrorMessage = "Years of experience must be between 0 and 50")]
        public int YearsExperience { get; set; }

        [StringLength(20, ErrorMessage = "Proficiency level cannot exceed 20 characters")]
        public string? ProficiencyLevel { get; set; }
    }
}
