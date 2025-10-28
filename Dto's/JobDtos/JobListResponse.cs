namespace Recruitment_System.Dto_s.JobDtos
{
    public class JobListResponse
    {
        public int JobId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public string EmploymentType { get; set; } = string.Empty;
        public int NumberOfPositions { get; set; }
        public DateTime? ApplicationDeadline { get; set; }
        public string Status { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int SkillsCount { get; set; }
    }
}
