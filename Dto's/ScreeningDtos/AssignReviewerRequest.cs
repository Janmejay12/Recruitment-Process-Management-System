using System.ComponentModel.DataAnnotations;

namespace Recruitment_System.Dto_s.ScreeningDtos
{
    public class AssignReviewerRequest
    {
        [Required]
        public int ReviewerUserId { get; set; }
    }
}
