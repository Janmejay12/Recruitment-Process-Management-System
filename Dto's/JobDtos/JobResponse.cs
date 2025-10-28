namespace Recruitment_System.Dto_s.JobDtos
{
    public class JobResponse
    {
        public int JobId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public int MinExperience { get; set; }
        public string Level { get; set; } = string.Empty;
        public string? SalaryRange { get; set; }
        public string EmploymentType { get; set; } = string.Empty;
        public int NumberOfPositions { get; set; }
        public DateTime? ApplicationDeadline { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ClosedReason { get; set; }
        public DateTime? ClosedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty; // Creator's name
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? OnHoldReason { get; set; }
        public DateTime? OnHoldAt { get; set; }
        public bool IsActive { get; set; }
        public List<JobSkillInfo> Skills { get; set; } = new List<JobSkillInfo>();
    }

    public class JobSkillInfo
    {
        public int SkillId { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool IsMandatory { get; set; }
        public int Priority { get; set; }  // 1=High, 2=Medium, 3=Low
        public string? Notes { get; set; }
    }
}
