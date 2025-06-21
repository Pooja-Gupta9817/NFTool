using DesktopTool.App.Core.Interfaces;
using DesktopTool.App.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DesktopTool.App.Infrastructure.Repository
{
    public class UploadedFileRepository : IUploadedFileRepository
    {
        private readonly ApplicationDbContext _context;

        public UploadedFileRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(UploadedFiles file)
        {
            await _context.UploadedFiles.AddAsync(file);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
