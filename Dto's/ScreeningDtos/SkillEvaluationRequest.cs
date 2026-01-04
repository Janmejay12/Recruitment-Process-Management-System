using System.ComponentModel.DataAnnotations;

namespace Recruitment_System.Dto_s.ScreeningDtos
{
    public class SkillEvaluationRequest
    {
        [Required]
        public int SkillId { get; set; }

        [Range(0, 50)]
        public int YearsExperience { get; set; }
    }
}
