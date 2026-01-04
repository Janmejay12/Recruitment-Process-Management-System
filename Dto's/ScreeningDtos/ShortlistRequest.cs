using System.ComponentModel.DataAnnotations;

namespace Recruitment_System.Dto_s.ScreeningDtos
{
    public class ShortlistRequest
    {
        [Required]
        public int InterviewerUserId { get; set; }
    }
}
