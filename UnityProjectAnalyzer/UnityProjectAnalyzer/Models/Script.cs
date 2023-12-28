using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityProjectAnalyzer.Models
{
    public class Script
    {
        public String Guid { get; set; }
        public String Path { get; set; }

        public Script(String guid, String path)
        {
            this.Guid = guid;
            this.Path = path;
        }
    }
}
