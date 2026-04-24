using DocumentService.Domain.Entities;
using DocumentService.Application.Interfaces;
using DocumentService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DocumentService.Infrastructure.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly AppDbContext _context;
    public DocumentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Document?> GetByIdAsync(Guid id)
    {
        return await _context.Documents.FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<List<Document>> GetByApplicantUserIdAsync(Guid applicantUserId)
    {
        return await _context.Documents
            .Where(d => d.ApplicantUserId == applicantUserId)
            .ToListAsync();
    }

    public async Task AddDocumentAsync(Document document)
    {
        await _context.Documents.AddAsync(document);
        await _context.SaveChangesAsync();   
    }

    public async Task AddStoredFileAsync(StoredFile file)
    {
        await _context.StoredFiles.AddAsync(file);
        await _context.SaveChangesAsync();
    }

    public async Task<StoredFile?> GetStoredFileByIdAsync(Guid id)
    {
        return await _context.StoredFiles.FirstOrDefaultAsync(f => f.Id == id);
    }
}