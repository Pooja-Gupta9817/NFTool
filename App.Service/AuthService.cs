using DesktopTool.App.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopTool.App.Service
{
    public class AuthService : IAuthService
    {
        public async Task<bool> LoginAsync(string username, string password)
        {
            // Replace this with actual Azure authentication logic
            await Task.Delay(500);
            return username == "admin" && password == "1234";
        }
    }
}
