using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTcp
{
    public interface ISimpleTcpClient
    {
        Task<int> SendAsync(string message);
        Task<int> SendObjectAsync<T>(T obj);
        Task<T?> ReceiveObjectAsync<T>();
        Task<byte[]> ReceiveAsync();
        void Close();
    }
}
