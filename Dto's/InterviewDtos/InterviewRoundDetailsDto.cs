namespace Recruitment_System.Dto_s.InterviewDtos
{
    public class InterviewRoundDetailsDto
    {
        public int InterviewRoundId { get; set; }
        public int RoundNumber { get; set; }
        public string RoundType { get; set; } = "";
        public string Status { get; set; } = "";

        public DateTime? ScheduledAt { get; set; }
        public string Mode { get; set; } = "";
        public string? MeetingLinkOrLocation { get; set; }

        public List<string> PanelMembers { get; set; } = new();
        public List<InterviewFeedbackDto> Feedbacks { get; set; } = new();
    }
}
