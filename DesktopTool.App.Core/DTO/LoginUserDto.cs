using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopTool.App.Core
{
    public class LoginUserDto
    {
        public string Token { get; set; }           // Access token
        public string RefreshToken { get; set; }    // Refresh token
        public UserInfoDto User { get; set; }
    }
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class UserInfoDto
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
    }
}
