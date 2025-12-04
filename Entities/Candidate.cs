using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recruitment_System.Entities
{
    [Table("Candidates")]
    public class Candidate
    {
        [Key]
        public int CandidateId { get; set; }

       
        public int? UserId { get; set; }  

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        public int? CreatedBy { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual User? CreatedByUser { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(500)]
        public string? CvPath { get; set; }

        [StringLength(20)]
        public string ProfileStatus { get; set; } = "Applied"; // Applied, Shortlisted, OnHold, etc.

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public virtual ICollection<CandidateSkill> CandidateSkills { get; set; } = new List<CandidateSkill>();
        public virtual ICollection<JobClosure> JobClosures { get; set; } = new List<JobClosure>();
    }
}
