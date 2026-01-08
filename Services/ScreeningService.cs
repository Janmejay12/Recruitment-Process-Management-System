using Microsoft.EntityFrameworkCore;
using Recruitment_System.Data;
using Recruitment_System.Dto_s.ScreeningDtos;
using Recruitment_System.Entities;

namespace Recruitment_System.Services
{
    public class ScreeningService
    {
        private readonly ApplicationDbContext _db;

        public ScreeningService(ApplicationDbContext db)
        {
            _db = db;
        }

        
        public async Task<CandidateJobReview> CreateReviewAsync(int candidateId, int jobId)
        {
            if (await _db.CandidateJobReviews.AnyAsync(r => r.CandidateId == candidateId))
                throw new InvalidOperationException("Review already exists for this candidate.");

            var review = new CandidateJobReview
            {
                CandidateId = candidateId,
                JobId = jobId,
                CurrentStage = "Screening",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.CandidateJobReviews.Add(review);
            await _db.SaveChangesAsync();

            return review;
        }

        
        public async Task AssignReviewerAsync(int reviewId, int reviewerUserId)
        {
            var review = await LoadReview(reviewId);

            EnsureStage(review, "Screening");

            review.AssignedReviewerId = reviewerUserId;
            review.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
        }

        
        public async Task SaveSkillEvaluationAsync(
            int reviewId,
            int skillId,
            int yearsExperience,
            int verifiedByUserId)
        {
            var review = await LoadReview(reviewId);
            EnsureStage(review, "Screening");

            var existing = await _db.CandidateSkillEvaluations
                .FirstOrDefaultAsync(e => e.ReviewId == reviewId && e.SkillId == skillId);

            if (existing == null)
            {
                var evaluation = new CandidateSkillEvaluation
                {
                    ReviewId = reviewId,
                    SkillId = skillId,
                    YearsExperience = yearsExperience,
                    IsVerified = true,
                    VerifiedByUserId = verifiedByUserId
                };

                _db.CandidateSkillEvaluations.Add(evaluation);
            }
            else
            {
                existing.YearsExperience = yearsExperience;
                existing.VerifiedByUserId = verifiedByUserId;
            }

            review.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        
        public async Task AddCommentAsync(
            int reviewId,
            string commentText,
            int commentedByUserId,
            string roleAtTime)
        {
            if (string.IsNullOrWhiteSpace(commentText))
                throw new InvalidOperationException("Comment cannot be empty.");

            var review = await LoadReview(reviewId);

            var comment = new CandidateReviewComment
            {
                ReviewId = reviewId,
                CommentText = commentText,
                CommentedByUserId = commentedByUserId,
                RoleAtTime = roleAtTime,
                CreatedAt = DateTime.UtcNow
            };

            _db.CandidateReviewComments.Add(comment);
            await _db.SaveChangesAsync();
        }

        
        public async Task ShortlistAsync(int reviewId, int interviewerUserId)
        {
            var review = await LoadReview(reviewId);
            EnsureStage(review, "Screening");

            review.CurrentStage = "Interview";
            review.AssignedInterviewerId = interviewerUserId;
            review.UpdatedAt = DateTime.UtcNow;

            var candidate = await _db.Candidates.FindAsync(review.CandidateId);
            if (candidate != null)
            {
                candidate.ProfileStatus = "Shortlisted";
                candidate.UpdatedAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
        }

       
        public async Task RejectAsync(int reviewId)
        {
            var review = await LoadReview(reviewId);

            if (review.CurrentStage == "Rejected")
                throw new InvalidOperationException("Candidate already rejected.");

            review.CurrentStage = "Rejected";
            review.UpdatedAt = DateTime.UtcNow;

            var candidate = await _db.Candidates.FindAsync(review.CandidateId);
            if (candidate != null)
            {
                candidate.ProfileStatus = "Rejected";
                candidate.UpdatedAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
        }

      
        public async Task<bool> HasPreviousReviewHistoryAsync(string candidateEmail)
        {
            return await _db.Candidates
                .Where(c => c.Email == candidateEmail)
                .Join(_db.CandidateJobReviews,
                      c => c.CandidateId,
                      r => r.CandidateId,
                      (c, r) => r)
                .AnyAsync(r =>
                    r.CurrentStage == "Interview" ||
                    r.CurrentStage == "Rejected" ||
                    r.CurrentStage == "Shortlisted");
        }

        
        private async Task<CandidateJobReview> LoadReview(int reviewId)
        {
            var review = await _db.CandidateJobReviews
                .Include(r => r.SkillEvaluations)
                .Include(r => r.Comments)
                .FirstOrDefaultAsync(r => r.ReviewId == reviewId);

            if (review == null)
                throw new InvalidOperationException("Review not found.");

            return review;
        }

        private static void EnsureStage(CandidateJobReview review, string requiredStage)
        {
            if (review.CurrentStage != requiredStage)
                throw new InvalidOperationException(
                    $"Action not allowed in '{review.CurrentStage}' stage.");
        }

        public async Task<List<ReviewPipelineItemDto>> GetReviewsByJobAsync(int jobId)
        {
            var reviews = await _db.CandidateJobReviews
                .Include(r => r.Candidate)
                .Include(r => r.AssignedReviewer)
                .Include(r => r.AssignedInterviewer)
                .Where(r => r.JobId == jobId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return reviews.Select(r => new ReviewPipelineItemDto
            {
                ReviewId = r.ReviewId,
                CandidateId = r.CandidateId,
                CandidateName = r.Candidate.FullName,
                CandidateEmail = r.Candidate.Email,
                CurrentStage = r.CurrentStage,
                ReviewerName = r.AssignedReviewer != null ? r.AssignedReviewer.FullName : null,
                InterviewerName = r.AssignedInterviewer != null ? r.AssignedInterviewer.FullName : null,
                CreatedAt = r.CreatedAt
            }).ToList();
        }


        public async Task<ReviewDetailsDto> GetReviewDetailsAsync(int reviewId)
        {
            var review = await _db.CandidateJobReviews
                .Include(r => r.Candidate)
                    .ThenInclude(c => c.CandidateSkills)
                        .ThenInclude(cs => cs.Skill)
                .Include(r => r.Job)
                    .ThenInclude(j => j.JobSkills)
                        .ThenInclude(js => js.Skill)
                .Include(r => r.SkillEvaluations)
                    .ThenInclude(se => se.Skill)
                .Include(r => r.SkillEvaluations)
                    .ThenInclude(se => se.VerifiedByUser)
                .Include(r => r.Comments)
                    .ThenInclude(c => c.CommentedByUser)
                .Include(r => r.AssignedReviewer)
                .Include(r => r.AssignedInterviewer)
                .FirstOrDefaultAsync(r => r.ReviewId == reviewId);

            if (review == null)
                throw new InvalidOperationException("Review not found.");

            // History check (same email, other reviews)
            var hasHistory = await _db.Candidates
                .Where(c => c.Email == review.Candidate.Email && c.CandidateId != review.CandidateId)
                .Join(_db.CandidateJobReviews,
                      c => c.CandidateId,
                      r => r.CandidateId,
                      (c, r) => r)
                .AnyAsync(r =>
                    r.CurrentStage == "Interview" ||
                    r.CurrentStage == "Rejected" ||
                    r.CurrentStage == "Shortlisted");

            return new ReviewDetailsDto
            {
                ReviewId = review.ReviewId,
                CurrentStage = review.CurrentStage,

                Candidate = new ReviewCandidateDto
                {
                    CandidateId = review.Candidate.CandidateId,
                    FullName = review.Candidate.FullName,
                    Email = review.Candidate.Email,
                    Phone = review.Candidate.Phone,
                    CvPath = review.Candidate.CvPath
                },

                Job = new ReviewJobDto
                {
                    JobId = review.Job.JobId,
                    Title = review.Job.Title
                },

                JobRequiredSkills = review.Job.JobSkills.Select(js => new ReviewJobSkillDto
                {
                    SkillId = js.SkillId,
                    SkillName = js.Skill.SkillName,
                    IsMandatory = js.IsMandatory,
                    Priority = js.Priority
                }).ToList(),

                ResumeSkills = review.Candidate.CandidateSkills.Select(cs => new ReviewResumeSkillDto
                {
                    SkillId = cs.SkillId,
                    SkillName = cs.Skill.SkillName,
                    YearsExperience = cs.YearsExperience
                }).ToList(),

                VerifiedSkills = review.SkillEvaluations.Select(se => new ReviewVerifiedSkillDto
                {
                    SkillId = se.SkillId,
                    SkillName = se.Skill.SkillName,
                    YearsExperience = se.YearsExperience,
                    VerifiedBy = se.VerifiedByUser.FullName
                }).ToList(),

                Comments = review.Comments
                    .OrderByDescending(c => c.CreatedAt)
                    .Select(c => new ReviewCommentDto
                    {
                        CommentText = c.CommentText,
                        CommentedBy = c.CommentedByUser.FullName,
                        RoleAtTime = c.RoleAtTime,
                        CreatedAt = c.CreatedAt
                    }).ToList(),

                AssignedReviewerName = review.AssignedReviewer?.FullName,
                AssignedInterviewerName = review.AssignedInterviewer?.FullName,

                HasPreviousHistory = hasHistory
            };
        }

    }
}
