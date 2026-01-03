using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using Microsoft.EntityFrameworkCore;
using Recruitment_System.Data;
using Recruitment_System.Dto_s.CandidateDtos;
using Recruitment_System.Entities;
using System.Text.RegularExpressions;


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
                JobId = request.JobId,
                IsActive = true,
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

        public async Task<CandidateResponse> UploadResumeAsync(IFormFile file, int uploadedByUserId, int jobId)
        {
            if (file == null || file.Length == 0)
                throw new InvalidOperationException("Invalid file");

            using var tx = await _db.Database.BeginTransactionAsync();

            try
            {
                // 1. Save file
                string uploadsFolder = Path.Combine("wwwroot", "uploads", "cvs");
                Directory.CreateDirectory(uploadsFolder);

                string fileName = $"{Guid.NewGuid()}_{file.FileName}";
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // 2. Extract text using iText7
                string text = ExtractPdfText(filePath);

                if (string.IsNullOrWhiteSpace(text))
                    throw new InvalidOperationException("Unable to extract text from the resume.");

                // 3. Extract email, phone, name
                string email = Regex.Match(text, @"[A-Za-z0-9\._%+-]+@[A-Za-z0-9\.-]+\.[A-Za-z]{2,}").Value;
                string phone = Regex.Match(text, @"\+?\d{10,13}").Value;

                string name = text.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                                  .FirstOrDefault()?.Trim() ?? "Unknown";

                // 4. Create candidate
                var candidate = new Candidate
                {
                    FullName = name,
                    Email = email,
                    Phone = phone,
                    CvPath = "/uploads/cvs/" + fileName,
                    ProfileStatus = "Imported",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    JobId = jobId,
                    IsActive = true,
                    CreatedBy = uploadedByUserId
                };

                _db.Candidates.Add(candidate);
                await _db.SaveChangesAsync();

                // 5. Auto-detect skills from resume text
                var allSkills = await _db.Skills.Where(s => s.IsActive).ToListAsync();
                var matchedSkills = new List<CandidateSkill>();
                var matchedSkillResponses = new List<CandidateSkillResponse>();

                foreach (var skill in allSkills)
                {
                    if (text.Contains(skill.SkillName, StringComparison.OrdinalIgnoreCase))
                    {
                        matchedSkills.Add(new CandidateSkill
                        {
                            CandidateId = candidate.CandidateId,
                            SkillId = skill.SkillId,
                            YearsExperience = 0,
                            ProficiencyLevel = "Beginner"
                        });

                        matchedSkillResponses.Add(new CandidateSkillResponse
                        {
                            SkillId = skill.SkillId,
                            SkillName = skill.SkillName,
                            Category = skill.Category,
                            YearsExperience = 0,
                            ProficiencyLevel = "Beginner"
                        });
                    }
                }

                _db.CandidateSkills.AddRange(matchedSkills);
                await _db.SaveChangesAsync();

                await tx.CommitAsync();

                // 6. Final response
                return new CandidateResponse
                {
                    CandidateId = candidate.CandidateId,
                    FullName = candidate.FullName,
                    Email = candidate.Email,
                    Phone = candidate.Phone,
                    CvPath = candidate.CvPath,
                    Skills = matchedSkillResponses
                };
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        private string ExtractPdfText(string filePath)
        {
            using var reader = new PdfReader(filePath);
            using var pdfDoc = new PdfDocument(reader);

            string text = "";

            for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
            {
                var page = pdfDoc.GetPage(i);
                text += PdfTextExtractor.GetTextFromPage(page);
                text += "\n";
            }

            return text;
        }

        public async Task<List<CandidateResponse>> GetAllCandidatesAsync()
        {
            var candidates = await _db.Candidates
                        .Include(c => c.Job)
                .Include(c => c.CandidateSkills)
                    .ThenInclude(cs => cs.Skill)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return candidates.Select(c => new CandidateResponse
            {
                CandidateId = c.CandidateId,
                FullName = c.FullName,
                Email = c.Email,
                Phone = c.Phone,
                CvPath = c.CvPath,
                ProfileStatus = c.ProfileStatus,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                JobId = c.JobId,
                JobTitle = c.Job.Title,

                Skills = c.CandidateSkills.Select(cs => new CandidateSkillResponse
                {
                    SkillId = cs.SkillId,
                    SkillName = cs.Skill.SkillName,
                    Category = cs.Skill.Category,
                    YearsExperience = cs.YearsExperience,
                    ProficiencyLevel = cs.ProficiencyLevel
                }).ToList()
            }).ToList();
        }

    }
}
