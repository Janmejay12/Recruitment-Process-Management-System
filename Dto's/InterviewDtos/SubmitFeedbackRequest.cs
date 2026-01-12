using System.ComponentModel.DataAnnotations;

namespace Recruitment_System.Dto_s.InterviewDtos
{
    public class SubmitFeedbackRequest
    {
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [StringLength(20)]
        public string Recommendation { get; set; } = "Hold"; // Hire | Reject | Hold

        [StringLength(2000)]
        public string? Comments { get; set; }
    }
}
