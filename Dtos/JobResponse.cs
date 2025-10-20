using System.ComponentModel.DataAnnotations;

namespace Recruitment_System.Dtos
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
        public List<JobSkillResponse> JobSkills { get; set; } = new List<JobSkillResponse>();
    }
}
