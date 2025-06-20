using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopTool.App.Core.Models
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(string name, string email, string password, string role);
        Task<(bool Success, string Role)> LoginAsync(string email, string password);
        Task<bool> UploadPdfAsync(string filePath, IProgress<int> progress = null);


    }

}
