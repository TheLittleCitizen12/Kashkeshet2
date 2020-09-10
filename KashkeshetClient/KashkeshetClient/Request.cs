using System;
using System.Collections.Generic;
using System.Text;

namespace KashkeshetClient
{
    [Serializable]
    public class Request
    {
        public string Name { get; set; }
        

        public string Text { get; set; }
        public string Type { get; set; }
        public string Dst { get; set; }
    }
}
