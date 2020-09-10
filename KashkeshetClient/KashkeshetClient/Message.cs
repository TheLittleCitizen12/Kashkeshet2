using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace KashkeshetClient
{
    [Serializable]
    public class Message: Request
    {

        public Message()
        {
            Type = "message";

        }

    }
}
