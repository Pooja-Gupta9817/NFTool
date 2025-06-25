using DesktopTool.App.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopTool.App.Core
{
    public class UserContext : IUserContext
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }

}
