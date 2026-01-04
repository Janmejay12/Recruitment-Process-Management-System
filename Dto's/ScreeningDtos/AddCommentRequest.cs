using System.ComponentModel.DataAnnotations;

namespace Recruitment_System.Dto_s.ScreeningDtos
{
    public class AddCommentRequest
    {
        [Required]
        [StringLength(2000)]
        public string CommentText { get; set; } = string.Empty;
    }
}
