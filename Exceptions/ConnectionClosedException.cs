using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTcp.Exceptions
{
    public class ConnectionClosedException : Exception
    {
        public ConnectionClosedException()
            : base("Connection was closed by the remote host.") { }
    }
}
