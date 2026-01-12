namespace Recruitment_System.Dto_s.InterviewDtos
{
    public class InterviewProcessDetailsDto
    {
        public int InterviewProcessId { get; set; }
        public string Status { get; set; } = "";
        public int TotalRounds { get; set; }
        public int CurrentRound { get; set; }

        public string CandidateName { get; set; } = "";
        public string JobTitle { get; set; } = "";

        public List<InterviewRoundDetailsDto> Rounds { get; set; } = new();
    }
}
