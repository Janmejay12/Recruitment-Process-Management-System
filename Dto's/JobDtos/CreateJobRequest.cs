using System.ComponentModel.DataAnnotations;

namespace Recruitment_System.Dto_s.JobDtos
{
    public class CreateJobRequest
    {
        [Required(ErrorMessage = "Job title is required")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Job description is required")]
        [StringLength(2000, MinimumLength = 50, ErrorMessage = "Description must be between 50 and 2000 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is required")]
        [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Department is required")]
        [StringLength(50, ErrorMessage = "Department cannot exceed 50 characters")]
        public string Department { get; set; } = string.Empty;

        [Required(ErrorMessage = "Minimum experience is required")]
        [Range(0, 50, ErrorMessage = "Minimum experience must be between 0 and 50 years")]
        public int MinExperience { get; set; }

        [Required(ErrorMessage = "Job level is required")]
        [RegularExpression("^(Junior|Mid-Level|Senior|Lead|Principal)$", ErrorMessage = "Invalid job level")]
        public string Level { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Salary range cannot exceed 100 characters")]
        public string? SalaryRange { get; set; }

        [Required(ErrorMessage = "Employment type is required")]
        [RegularExpression("^(Full-time|Part-time|Contract)$", ErrorMessage = "Invalid employment type")]
        public string EmploymentType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Number of positions is required")]
        [Range(1, 100, ErrorMessage = "Number of positions must be between 1 and 100")]
        public int NumberOfPositions { get; set; } = 1;

        public DateTime? ApplicationDeadline { get; set; }
        // Optional skills to map at creation time
        public List<CreateJobSkillItem>? Skills { get; set; }

        public class CreateJobSkillItem
        {

            [Required]
            public int SkillId { get; set; }

            [Required]
            public bool IsMandatory { get; set; } = true;

            [Range(1, 3, ErrorMessage = "Priority must be 1 (High), 2 (Medium) or 3 (Low)")]
            public int Priority { get; set; } = 1;

            [StringLength(500)]
            public string? Notes { get; set; }

        }
    }
}
