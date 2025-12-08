using System.ComponentModel.DataAnnotations;

namespace Recruitment_System.Dto_s.CandidateDtos
{
    public class ResumeUploadRequest
    {
        [Required]
        public IFormFile File { get; set; } = null!;
    }
}
