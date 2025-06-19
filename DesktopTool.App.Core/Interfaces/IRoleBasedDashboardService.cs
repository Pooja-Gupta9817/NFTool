using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace DesktopTool.App.Core.Interfaces
{
    public interface IRoleBasedDashboardService
    {
        void GetDashboardForRole(string role);
    }
}
