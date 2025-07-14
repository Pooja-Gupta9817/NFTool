using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopTool.App.Core.Interfaces
{
    public interface IAIAssistantService
    {
        Task<string> AskAsync(string question);
    }
}
