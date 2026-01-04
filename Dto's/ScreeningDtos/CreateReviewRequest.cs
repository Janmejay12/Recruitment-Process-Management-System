using System.ComponentModel.DataAnnotations;

namespace Recruitment_System.Dto_s.ScreeningDtos
{
    public class CreateReviewRequest
    {
        [Required]
        public int CandidateId { get; set; }

        [Required]
        public int JobId { get; set; }
    }
}
