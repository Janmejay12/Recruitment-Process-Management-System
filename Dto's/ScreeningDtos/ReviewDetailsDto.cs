    namespace Recruitment_System.Dto_s.ScreeningDtos
{
    public class ReviewDetailsDto
    {
        public int ReviewId { get; set; }
        public string CurrentStage { get; set; } = "";

        public ReviewCandidateDto Candidate { get; set; } = null!;
        public ReviewJobDto Job { get; set; } = null!;

        public List<ReviewJobSkillDto> JobRequiredSkills { get; set; } = new();
        public List<ReviewResumeSkillDto> ResumeSkills { get; set; } = new();
        public List<ReviewVerifiedSkillDto> VerifiedSkills { get; set; } = new();

        public List<ReviewCommentDto> Comments { get; set; } = new();

        public string? AssignedReviewerName { get; set; }
        public string? AssignedInterviewerName { get; set; }

        public bool HasPreviousHistory { get; set; }
    }

    public class ReviewCandidateDto
    {
        public int CandidateId { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Phone { get; set; }
        public string? CvPath { get; set; }
    }

    public class ReviewJobDto
    {
        public int JobId { get; set; }
        public string Title { get; set; } = "";
    }

    public class ReviewJobSkillDto
    {
        public int SkillId { get; set; }
        public string SkillName { get; set; } = "";
        public bool IsMandatory { get; set; }
        public int Priority { get; set; }
    }

    public class ReviewResumeSkillDto
    {
        public int SkillId { get; set; }
        public string SkillName { get; set; } = "";
        public int YearsExperience { get; set; }
    }

    public class ReviewVerifiedSkillDto
    {
        public int SkillId { get; set; }
        public string SkillName { get; set; } = "";
        public int YearsExperience { get; set; }
        public string VerifiedBy { get; set; } = "";
    }

    public class ReviewCommentDto
    {
        public string CommentText { get; set; } = "";
        public string CommentedBy { get; set; } = "";
        public string RoleAtTime { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }

}
