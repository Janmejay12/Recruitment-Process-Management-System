using Recruitment_System.Dto_s.CandidateDtos;
using System.ComponentModel.DataAnnotations;

namespace Recruitment_System.Dto_s
{
    public class CandidateRegisterRequest
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, MinimumLength = 2)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number")]
        public string? Phone { get; set; }

        public string? CvPath { get; set; }

        public List<CandidateSkillRequest> Skills { get; set; } = new();
    }
    
}
