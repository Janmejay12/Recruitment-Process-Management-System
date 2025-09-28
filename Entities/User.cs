using System.ComponentModel.DataAnnotations;

namespace Recruitment_System.Entities
{
    public class User
    {
        public int UserId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [StringLength(20)]
        public string Status { get; set; } = "Active";

        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    }
}
