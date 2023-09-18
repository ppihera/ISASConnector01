using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISASWPFInterfaceFW
{
    internal class ErrorDTO
    {
        public List<string> Fields { get; set; } = new List<string>();
        public string Message { get; set; }
    }
}
