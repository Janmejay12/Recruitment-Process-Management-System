using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recruitment_System.Entities
{
    [Table("InterviewPanelMembers")]
    public class InterviewPanelMember
    {
        [Key]
        public int InterviewPanelMemberId { get; set; }

        [Required]
        public int InterviewRoundId { get; set; }

        [Required]
        public int InterviewerUserId { get; set; }

        // Navigation
        [ForeignKey(nameof(InterviewRoundId))]
        public virtual InterviewRound InterviewRound { get; set; } = null!;

        [ForeignKey(nameof(InterviewerUserId))]
        public virtual User Interviewer { get; set; } = null!;
    }
}
