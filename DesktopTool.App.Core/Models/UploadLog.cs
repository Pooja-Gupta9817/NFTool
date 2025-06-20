using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopTool.App.Core.Models
{
    public class UploadLog
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string UploadedBy { get; set; }
        public DateTime UploadedAt { get; set; }
    }

}
