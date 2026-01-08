namespace Recruitment_System.Dto_s.ScreeningDtos
{
    public class ReviewPipelineItemDto
    {
        public int ReviewId { get; set; }
        public int CandidateId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public string CandidateEmail { get; set; } = string.Empty;

        public string CurrentStage { get; set; } = string.Empty;

        public string? ReviewerName { get; set; }
        public string? InterviewerName { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
