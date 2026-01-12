using System.ComponentModel.DataAnnotations;

namespace Recruitment_System.Dto_s.InterviewDtos
{
    public class AddPanelMemberRequest
    {
        [Required]
        public int InterviewerUserId { get; set; }
    }
}
