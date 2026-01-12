using System.ComponentModel.DataAnnotations;

namespace Recruitment_System.Dto_s.InterviewDtos
{
    public class CreateRoundRequest
    {
        [Required]
        public int RoundNumber { get; set; }

        [Required]
        [StringLength(20)]
        public string RoundType { get; set; } = "Technical"; // Technical | HR
    }
}
