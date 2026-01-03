namespace Recruitment_System.Dto_s.CandidateDtos
{
    public class CandidateResponse
    {
        public int CandidateId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? CvPath { get; set; }
        public string ProfileStatus { get; set; } = "Applied";
        public bool IsActive { get; set; }
        public int JobId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<CandidateSkillResponse> Skills { get; set; } = new List<CandidateSkillResponse>();
    }

    public class CandidateSkillResponse
    {
        public int SkillId { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int YearsExperience { get; set; }
        public string? ProficiencyLevel { get; set; }
       
    }
}
