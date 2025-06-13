using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopTool.App.Core
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(string username, string password);
    }
}
