using DocumentService.Application.Interfaces;
using DocumentService.Domain.Entities;
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
        return await _context.Documents.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Document>> GetByApplicantUserIdAsync(Guid applicantUserId)
    {
        return await _context.Documents
            .Where(x => x.ApplicantUserId == applicantUserId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<StoredFile?> GetStoredFileByIdAsync(Guid id)
    {
        return await _context.StoredFiles.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddStoredFileAsync(StoredFile file)
    {
        await _context.StoredFiles.AddAsync(file);
        await _context.SaveChangesAsync();
    }

    public async Task AddDocumentAsync(Document document)
    {
        await _context.Documents.AddAsync(document);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateDocumentAsync(Document document)
    {
        _context.Documents.Update(document);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteDocumentAsync(Document document)
    {
        _context.Documents.Remove(document);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteStoredFileAsync(StoredFile file)
    {
        _context.StoredFiles.Remove(file);
        await _context.SaveChangesAsync();
    }
}
