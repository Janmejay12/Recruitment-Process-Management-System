using System.ComponentModel.DataAnnotations;

namespace Recruitment_System.Dto_s.JobDtos
{
    public class CloseWithCandidateRequest
    {
        [Required]
        public int CandidateId { get; set; }

        [Required]
        public DateTime SelectionDate { get; set; }

        [Required]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Notes { get; set; }
    }
}
