using System.Collections.Generic;

namespace ISASWPFInterfaceFW
{
    internal class ErrorDTO
    {
        public int Index { get; set; }
        public List<string> Fields { get; set; } = new List<string>();
        public string Message { get; set; }
    }
}
