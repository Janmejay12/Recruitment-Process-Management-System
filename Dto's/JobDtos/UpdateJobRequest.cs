using System.ComponentModel.DataAnnotations;

namespace Recruitment_System.Dto_s.JobDtos
{
    public class UpdateJobRequest
    {
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 200 characters")]
        public string? Title { get; set; }

        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Description must be between 50 and 2000 characters")]
        public string? Description { get; set; }

        [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters")]
        public string? Location { get; set; }

        [StringLength(50, ErrorMessage = "Department cannot exceed 50 characters")]
        public string? Department { get; set; }

        [Range(0, 50, ErrorMessage = "Minimum experience must be between 0 and 50 years")]
        public int? MinExperience { get; set; }

        [RegularExpression("^(Junior|Mid-Level|Senior|Lead|Principal)$", ErrorMessage = "Invalid job level")]
        public string? Level { get; set; }

        [StringLength(100, ErrorMessage = "Salary range cannot exceed 100 characters")]
        public string? SalaryRange { get; set; }

        [RegularExpression("^(Full-time|Part-time|Contract)$", ErrorMessage = "Invalid employment type")]
        public string? EmploymentType { get; set; }

        [Range(1, 100, ErrorMessage = "Number of positions must be between 1 and 100")]
        public int? NumberOfPositions { get; set; }

        public DateTime? ApplicationDeadline { get; set; }
    }
}
