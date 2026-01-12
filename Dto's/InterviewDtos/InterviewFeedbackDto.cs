namespace Recruitment_System.Dto_s.InterviewDtos
{
    public class InterviewFeedbackDto
    {
        public string InterviewerName { get; set; } = "";
        public int Rating { get; set; }
        public string Recommendation { get; set; } = "";
        public string? Comments { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
