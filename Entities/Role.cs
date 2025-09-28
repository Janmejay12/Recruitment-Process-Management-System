using System.ComponentModel.DataAnnotations;

namespace Recruitment_System.Entities
{
    public class Role
    {
        public int RoleId { get; set; }

        [Required]
        [StringLength(50)]
        public string RoleName { get; set; } = string.Empty;

        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
