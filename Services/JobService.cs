using Microsoft.EntityFrameworkCore;
using Recruitment_System.Data;
using Recruitment_System.Dto_s.JobDtos;
using Recruitment_System.Entities;

namespace Recruitment_System.Services
{
    public class JobService
    {
        private readonly ApplicationDbContext _db;

        public JobService(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<JobResponse> CreateJobAsync(CreateJobRequest request, int createdByUserId)
        {
            using var tx = await _db.Database.BeginTransactionAsync();

            var job = new Job
            {
                Title = request.Title,
                Description = request.Description,
                Location = request.Location,
                Department = request.Department,
                MinExperience = request.MinExperience,
                Level = request.Level,
                SalaryRange = request.SalaryRange,
                EmploymentType = request.EmploymentType,
                NumberOfPositions = request.NumberOfPositions,
                ApplicationDeadline = request.ApplicationDeadline,
                Status = "Open",
                CreatedBy = createdByUserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _db.Jobs.Add(job);
            await _db.SaveChangesAsync();

            if(request.Skills != null && request.Skills.Count > 0)
            {
                //ensure distinct skills
                var distinctSkillIds = request.Skills.Select(s => s.SkillId).Distinct().ToList();
                if(distinctSkillIds.Count != request.Skills.Count)
                {
                    throw new InvalidOperationException("Duplicate skills are not allowed for a job.");
                }

                //load vaalid skills
                var activeSkillIds = await _db.Skills
                    .Where(s => distinctSkillIds.Contains(s.SkillId) && s.IsActive)
                    .Select(s => s.SkillId)
                    .ToListAsync();

                // verify all exist and active
                var missing = distinctSkillIds.Except(activeSkillIds).ToList();
                if (missing.Count > 0)
                    throw new InvalidOperationException($"One or more skills are invalid or inactive: {string.Join(", ", missing)}");

                var jobSkills = request.Skills.Select(s => new JobSkill
                {
                    JobId = job.JobId,
                    SkillId = s.SkillId,
                    IsMandatory = s.IsMandatory,
                    Priority = s.Priority,
                    Notes = s.Notes
                });

                _db.JobSkills.AddRange(jobSkills);
                await _db.SaveChangesAsync();
            }

            await tx.CommitAsync();

            var result = await GetJobByIdAsync(job.JobId);
            if (result == null) throw new InvalidOperationException("Failed to load created job.");
            return result;
        }
        public async Task<JobResponse> GetJobByIdAsync(int jobId)
        {
            var job = await _db.Jobs
                .Include(j => j.CreatedByUser)
                .Include(j => j.JobSkills)
                .ThenInclude(j => j.Skill)
                .FirstOrDefaultAsync(j => j.JobId == jobId && j.IsActive);

            if (job == null) return null;

            return new JobResponse
            {
                JobId = job.JobId,
                Title = job.Title,
                Description = job.Description,
                Location = job.Location,
                Department = job.Department,
                MinExperience = job.MinExperience,
                Level = job.Level,
                SalaryRange = job.SalaryRange,
                EmploymentType = job.EmploymentType,
                NumberOfPositions = job.NumberOfPositions,
                ApplicationDeadline = job.ApplicationDeadline,
                Status = job.Status,
                ClosedReason = job.ClosedReason,
                ClosedAt = job.ClosedAt,
                CreatedBy = job.CreatedByUser.FullName,
                CreatedAt = job.CreatedAt,
                UpdatedAt = job.UpdatedAt,
                OnHoldReason = job.OnHoldReason,
                OnHoldAt = job.OnHoldAt,
                IsActive = job.IsActive,
                Skills = job.JobSkills.Select(js => new JobSkillInfo
                {
                    SkillId = js.SkillId,
                    SkillName = js.Skill.SkillName,
                    Category = js.Skill.Category,
                    IsMandatory = js.IsMandatory,
                    Priority = js.Priority,
                    Notes = js.Notes
                }).ToList()
            };
        }


        public async Task<List<JobListResponse>> GetJobAsync()
        {
            var query = _db.Jobs
                .Include(j => j.CreatedByUser)
                 .Include(j => j.JobSkills)
                .Where(j => j.IsActive);

            return query.Select(j => new JobListResponse
            {
                JobId = j.JobId,
                Title = j.Title,
                Location = j.Location,
                Department = j.Department,
                Level = j.Level,
                EmploymentType = j.EmploymentType,
                NumberOfPositions = j.NumberOfPositions,
                ApplicationDeadline = j.ApplicationDeadline,
                Status = j.Status,
                CreatedBy = j.CreatedByUser.FullName,
                CreatedAt = j.CreatedAt,
                UpdatedAt = j.UpdatedAt,
                SkillsCount = j.JobSkills.Count
            }).ToList();
        }

        public async Task<JobResponse> UpdateJobAsync(int jobId, UpdateJobRequest request)
        {
            var job = await _db.Jobs
                .Include(j => j.CreatedByUser)
                .FirstOrDefaultAsync(j => j.JobId == jobId && j.IsActive);

            if (job == null) return null;
            if (job.Status == "Closed")
                throw new InvalidOperationException("Cannot update a closed job");

            if (request.Title != null) job.Title = request.Title;
            if (request.Description != null) job.Description = request.Description;
            if (request.Location != null) job.Location = request.Location;
            if (request.Department != null) job.Department = request.Department;
            if (request.MinExperience.HasValue) job.MinExperience = request.MinExperience.Value;
            if (request.Level != null) job.Level = request.Level;
            if (request.SalaryRange != null) job.SalaryRange = request.SalaryRange;
            if (request.EmploymentType != null) job.EmploymentType = request.EmploymentType;
            if (request.NumberOfPositions.HasValue) job.NumberOfPositions = request.NumberOfPositions.Value;
            if (request.ApplicationDeadline.HasValue) job.ApplicationDeadline = request.ApplicationDeadline;

            job.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return await GetJobByIdAsync(jobId);

        }


        public async Task<bool> DeleteJobAsync(int jobId)
        {
            var job = await _db.Jobs
                .FirstOrDefaultAsync(j => j.JobId == jobId && j.IsActive);
            if (job == null) return false;
            if (job.Status == "Closed")
                throw new InvalidOperationException("Cannot delete a closed job");

            job.IsActive = false;
            job.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }

    }
}
