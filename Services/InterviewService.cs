using Microsoft.EntityFrameworkCore;
using Recruitment_System.Data;
using Recruitment_System.Dto_s.InterviewDtos;
using Recruitment_System.Entities;

namespace Recruitment_System.Services
{
    public class InterviewService
    {
        private readonly ApplicationDbContext _db;

        public InterviewService(ApplicationDbContext db)
        {
            _db = db;
        }

        
        public async Task<InterviewProcess> StartInterviewProcessAsync(int reviewId, int totalRounds)
        {
            var review = await _db.CandidateJobReviews.FindAsync(reviewId);
            if (review == null)
                throw new InvalidOperationException("Review not found.");

            if (review.CurrentStage != "Interview")
                throw new InvalidOperationException("Candidate is not in Interview stage.");

            if (await _db.InterviewProcesses.AnyAsync(p => p.ReviewId == reviewId))
                throw new InvalidOperationException("Interview process already exists.");

            var process = new InterviewProcess
            {
                ReviewId = reviewId,
                TotalRounds = totalRounds,
                CurrentRound = 1,
                Status = "InProgress",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.InterviewProcesses.Add(process);
            await _db.SaveChangesAsync();

            return process;
        }

       
        
        public async Task<InterviewRound> CreateRoundAsync(int processId, int roundNumber, string roundType)
        {
            var process = await _db.InterviewProcesses.FindAsync(processId);
            if (process == null)
                throw new InvalidOperationException("Interview process not found.");

            if (process.Status != "InProgress")
                throw new InvalidOperationException("Cannot add rounds to a finished process.");

            if (roundNumber < 1 || roundNumber > process.TotalRounds)
                throw new InvalidOperationException("Invalid round number.");

            if (await _db.InterviewRounds.AnyAsync(r => r.InterviewProcessId == processId && r.RoundNumber == roundNumber))
                throw new InvalidOperationException("This round already exists.");

            var round = new InterviewRound
            {
                InterviewProcessId = processId,
                RoundNumber = roundNumber,
                RoundType = roundType,
                Status = "Scheduled",
                CreatedAt = DateTime.UtcNow
            };

            _db.InterviewRounds.Add(round);
            await _db.SaveChangesAsync();

            return round;
        }

       
        public async Task ScheduleRoundAsync(int roundId, DateTime scheduledAt, string mode, string? linkOrLocation)
        {
            var round = await _db.InterviewRounds.FindAsync(roundId);
            if (round == null)
                throw new InvalidOperationException("Round not found.");

            round.ScheduledAt = scheduledAt;
            round.Mode = mode;
            round.MeetingLinkOrLocation = linkOrLocation;

            await _db.SaveChangesAsync();
        }

       
        public async Task AddPanelMemberAsync(int roundId, int interviewerUserId)
        {
            if (await _db.InterviewPanelMembers.AnyAsync(p =>
                p.InterviewRoundId == roundId && p.InterviewerUserId == interviewerUserId))
                throw new InvalidOperationException("Interviewer already in panel.");

            var panel = new InterviewPanelMember
            {
                InterviewRoundId = roundId,
                InterviewerUserId = interviewerUserId
            };

            _db.InterviewPanelMembers.Add(panel);
            await _db.SaveChangesAsync();
        }

        
        public async Task SubmitFeedbackAsync(int roundId, int interviewerUserId, int rating, string recommendation, string? comments)
        {
            var round = await _db.InterviewRounds
                .Include(r => r.InterviewProcess)
                .FirstOrDefaultAsync(r => r.InterviewRoundId == roundId);

            if (round == null)
                throw new InvalidOperationException("Round not found.");

            if (round.Status != "Scheduled")
                throw new InvalidOperationException("Round is not active.");

            if (await _db.InterviewFeedbacks.AnyAsync(f =>
                f.InterviewRoundId == roundId && f.InterviewerUserId == interviewerUserId))
                throw new InvalidOperationException("You already submitted feedback.");

            var feedback = new InterviewFeedback
            {
                InterviewRoundId = roundId,
                InterviewerUserId = interviewerUserId,
                Rating = rating,
                Recommendation = recommendation,
                Comments = comments,
                CreatedAt = DateTime.UtcNow
            };

            _db.InterviewFeedbacks.Add(feedback);
            await _db.SaveChangesAsync();
        }

       
        public async Task CompleteRoundAsync(int roundId)
        {
            var round = await _db.InterviewRounds
                .Include(r => r.InterviewProcess)
                .Include(r => r.Feedbacks)
                .FirstOrDefaultAsync(r => r.InterviewRoundId == roundId);

            if (round == null)
                throw new InvalidOperationException("Round not found.");

            if (round.Status != "Scheduled")
                throw new InvalidOperationException("Round already completed or cancelled.");

            if (!round.Feedbacks.Any())
                throw new InvalidOperationException("Cannot complete round without feedback.");

            // If ANY feedback says Reject ? FINAL REJECT
            if (round.Feedbacks.Any(f => f.Recommendation == "Reject"))
            {
                round.Status = "Completed";
                round.InterviewProcess.Status = "Rejected";
                round.InterviewProcess.UpdatedAt = DateTime.UtcNow;

                await RejectCandidateFinal(round.InterviewProcess.ReviewId);
                await _db.SaveChangesAsync();
                return;
            }

            // Otherwise, round passed
            round.Status = "Completed";

            var process = round.InterviewProcess;

            if (process.CurrentRound >= process.TotalRounds)
            {
                // FINAL SELECT
                process.Status = "Selected";
                process.UpdatedAt = DateTime.UtcNow;

                await SelectCandidateFinal(process.ReviewId);
            }
            else
            {
                process.CurrentRound++;
                process.UpdatedAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
        }

        
        private async Task RejectCandidateFinal(int reviewId)
        {
            var review = await _db.CandidateJobReviews.FindAsync(reviewId);
            if (review != null)
            {
                review.CurrentStage = "Rejected";
            }

            var candidate = await _db.Candidates.FindAsync(review!.CandidateId);
            if (candidate != null)
            {
                candidate.ProfileStatus = "Rejected";
            }
        }

       
        private async Task SelectCandidateFinal(int reviewId)
        {
            var review = await _db.CandidateJobReviews.FindAsync(reviewId);
            if (review != null)
            {
                review.CurrentStage = "Selected";
            }

            var candidate = await _db.Candidates.FindAsync(review!.CandidateId);
            if (candidate != null)
            {
                candidate.ProfileStatus = "Selected";
            }
        }

        public async Task<InterviewProcessDetailsDto> GetInterviewProcessDetailsAsync(int processId)
        {
            var process = await _db.InterviewProcesses
                .Include(p => p.Review)
                    .ThenInclude(r => r.Candidate)
                .Include(p => p.Review)
                    .ThenInclude(r => r.Job)
                .Include(p => p.Rounds)
                    .ThenInclude(r => r.PanelMembers)
                        .ThenInclude(pm => pm.Interviewer)
                .Include(p => p.Rounds)
                    .ThenInclude(r => r.Feedbacks)
                        .ThenInclude(f => f.Interviewer)
                .FirstOrDefaultAsync(p => p.InterviewProcessId == processId);

            if (process == null)
                throw new InvalidOperationException("Interview process not found.");

            return new InterviewProcessDetailsDto
            {
                InterviewProcessId = process.InterviewProcessId,
                Status = process.Status,
                TotalRounds = process.TotalRounds,
                CurrentRound = process.CurrentRound,

                CandidateName = process.Review.Candidate.FullName,
                JobTitle = process.Review.Job.Title,

                Rounds = process.Rounds
                    .OrderBy(r => r.RoundNumber)
                    .Select(r => new InterviewRoundDetailsDto
                    {
                        InterviewRoundId = r.InterviewRoundId,
                        RoundNumber = r.RoundNumber,
                        RoundType = r.RoundType,
                        Status = r.Status,
                        ScheduledAt = r.ScheduledAt,
                        Mode = r.Mode,
                        MeetingLinkOrLocation = r.MeetingLinkOrLocation,

                        PanelMembers = r.PanelMembers
                            .Select(p => p.Interviewer.FullName)
                            .ToList(),

                        Feedbacks = r.Feedbacks
                            .Select(f => new InterviewFeedbackDto
                            {
                                InterviewerName = f.Interviewer.FullName,
                                Rating = f.Rating,
                                Recommendation = f.Recommendation,
                                Comments = f.Comments,
                                CreatedAt = f.CreatedAt
                            })
                            .ToList()
                    })
                    .ToList()
            };
        }

    }
}
