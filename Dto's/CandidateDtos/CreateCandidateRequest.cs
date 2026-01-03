using System.ComponentModel.DataAnnotations;

namespace Recruitment_System.Dto_s.CandidateDtos
{
    public class CreateCandidateRequest
    {
        [Required]
        public int JobId { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string? Phone { get; set; }

        public string? CvPath { get; set; }

        [StringLength(20)]
        public string ProfileStatus { get; set; } = "Applied";

        public List<CandidateSkillRequest>? Skills { get; set; }
    }
}
