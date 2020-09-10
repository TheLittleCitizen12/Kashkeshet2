using System;
using System.Collections.Generic;
using System.Text;

namespace KashkeshetClient
{
    [Serializable]
    public class PrivateMessage: Request
    {
        public PrivateMessage()
        {
            Type = "privateMessage";
        }
    }
}
