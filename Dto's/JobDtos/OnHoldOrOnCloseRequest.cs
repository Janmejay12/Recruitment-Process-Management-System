using System.ComponentModel.DataAnnotations;

namespace Recruitment_System.Dto_s.JobDtos
{
    public class OnHoldOrOnCloseRequest
    {
        [Required]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;
    }
}
