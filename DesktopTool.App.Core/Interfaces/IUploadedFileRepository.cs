using DesktopTool.App.Core.Models;
using System.Threading.Tasks;

namespace DesktopTool.App.Core.Interfaces
{
    public interface IUploadedFileRepository
    {
        Task AddAsync(UploadedFiles file);
        Task SaveChangesAsync();
    }
}
