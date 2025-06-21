using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopTool.App.Core.Models
{
    public class UploadedFiles
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public long Size { get; set; }
        public string UploadedBy { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.Now;
    }

}
