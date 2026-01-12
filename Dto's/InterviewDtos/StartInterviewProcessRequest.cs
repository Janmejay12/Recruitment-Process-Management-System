using System.ComponentModel.DataAnnotations;

namespace Recruitment_System.Dto_s.InterviewDtos
{
    public class StartInterviewProcessRequest
    {
        [Range(1, 10)]
        public int TotalRounds { get; set; } = 2;
    }
}
