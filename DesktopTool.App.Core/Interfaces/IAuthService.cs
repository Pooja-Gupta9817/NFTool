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
        Task<bool> LoginAsync(string name, string password);
    }

}
