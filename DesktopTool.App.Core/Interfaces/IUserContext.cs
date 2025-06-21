using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopTool.App.Core.Interfaces
{
    public interface IUserContext
    {
        string Name { get; set; }
        string Email { get; set; }
        string Role { get; set; }
    }

}
