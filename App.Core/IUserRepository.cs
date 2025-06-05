using DesktopTool.App.UI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopTool.App.Core
{
    public interface IUserRepository : IRepository<UI.Model.User>
    {
        Task<UI.Model.User> GetByEmailAsync(string email);
    }

}
