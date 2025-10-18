using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recruitment_System.Entities
{
    [Table("Jobs")]
    public class Job
    {
        [Key]
        public int JobId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Location { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Department { get; set; } = string.Empty;

        [Required]
        public int MinExperience { get; set; }

        [Required]
        [StringLength(20)]
        public string Level { get; set; } = string.Empty; // Junior, Mid-Level, Senior, Lead, Principal

        [StringLength(100)]
        public string? SalaryRange { get; set; }

        [Required]
        [StringLength(20)]
        public string EmploymentType { get; set; } = string.Empty; // Full-time, Part-time, Contract

        [Required]
        public int NumberOfPositions { get; set; } = 1;

        public DateTime? ApplicationDeadline { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Open"; // Open, OnHold, Closed

        [StringLength(500)]
        public string? ClosedReason { get; set; }

        public DateTime? ClosedAt { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [StringLength(500)]
        public string? OnHoldReason { get; set; }

        public DateTime? OnHoldAt { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation Properties
        [ForeignKey("CreatedBy")]
        public virtual User CreatedByUser { get; set; } = null!;

        public virtual ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
        public virtual ICollection<JobClosure> JobClosures { get; set; } = new List<JobClosure>();
    }
}