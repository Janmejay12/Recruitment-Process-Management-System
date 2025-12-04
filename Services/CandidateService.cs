using Microsoft.EntityFrameworkCore;
using Recruitment_System.Data;
using Recruitment_System.Dto_s.CandidateDtos;
using Recruitment_System.Entities;

namespace Recruitment_System.Services
{
    public class CandidateService
    {
        private readonly ApplicationDbContext _db;

        public CandidateService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<CandidateResponse> CreateCandidateManualAsync(CreateCandidateRequest request, int createdByUserId)
        {
            using var tx = await _db.Database.BeginTransactionAsync();

            if (await _db.Candidates.AnyAsync(c => c.Email == request.Email))
                throw new InvalidOperationException("A candidate with this email already exists.");

            var candidate = new Candidate
            {
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                CvPath = request.CvPath,
                ProfileStatus = request.ProfileStatus ?? "Applied",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true,
                UserId = null, // Manual candidates don't have User accounts initially
                CreatedBy = createdByUserId // Track who created this candidate profile
            };

            _db.Candidates.Add(candidate);
            await _db.SaveChangesAsync();

            if (request.Skills != null && request.Skills.Any())
            {
                // Check for duplicate skills
                var distinctSkillIds = request.Skills.Select(s => s.SkillId).Distinct().ToList();
                if (distinctSkillIds.Count != request.Skills.Count)
                    throw new InvalidOperationException("Duplicate skills are not allowed for a candidate.");

                // Validate skills exist and are active
                var validSkillIds = await _db.Skills
                    .Where(s => distinctSkillIds.Contains(s.SkillId) && s.IsActive)
                    .Select(s => s.SkillId)
                    .ToListAsync();

                var invalidSkillIds = distinctSkillIds.Except(validSkillIds).ToList();
                if (invalidSkillIds.Any())
                    throw new InvalidOperationException($"Invalid or inactive skill IDs: {string.Join(", ", invalidSkillIds)}");

                var candidateSkills = request.Skills.Select(s => new CandidateSkill
                {
                    CandidateId = candidate.CandidateId,
                    SkillId = s.SkillId,
                    YearsExperience = s.YearsExperience,
                    ProficiencyLevel = s.ProficiencyLevel ?? "Beginner",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }).ToList();

                _db.CandidateSkills.AddRange(candidateSkills);
                await _db.SaveChangesAsync();
            }

            await tx.CommitAsync();

            // Return response with proper skill details loaded from DB
            return await GetCandidateByIdAsync(candidate.CandidateId) 
                ?? throw new InvalidOperationException("Failed to load created candidate.");
        }

        public async Task<CandidateResponse?> GetCandidateByIdAsync(int candidateId)
        {
            var candidate = await _db.Candidates
                .Include(c => c.CandidateSkills)
                .ThenInclude(cs => cs.Skill)
                .FirstOrDefaultAsync(c => c.CandidateId == candidateId && c.IsActive);

            if (candidate == null) return null;

            return new CandidateResponse
            {
                CandidateId = candidate.CandidateId,
                FullName = candidate.FullName,
                Email = candidate.Email,
                Phone = candidate.Phone,
                CvPath = candidate.CvPath,
                ProfileStatus = candidate.ProfileStatus,
                CreatedAt = candidate.CreatedAt,
                UpdatedAt = candidate.UpdatedAt,
                IsActive = candidate.IsActive,
                Skills = candidate.CandidateSkills.Select(cs => new CandidateSkillResponse
                {
                    SkillId = cs.SkillId,
                    SkillName = cs.Skill.SkillName,
                    Category = cs.Skill.Category,
                    YearsExperience = cs.YearsExperience,
                    ProficiencyLevel = cs.ProficiencyLevel
                }).ToList()
            };
        }
    }
}
