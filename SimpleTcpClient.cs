using SimpleTcp.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SimpleTcp
{
    public class SimpleTcpClient : ISimpleTcpClient
    {
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

        public Socket Client { get; set; } = null!;
        public int MaxSize { get; set; } = 1024 * 1024;

        public SimpleTcpClient(Socket client)
        {
            Client = client;
        }

        public SimpleTcpClient(Socket client, int maxSize) : this(client)
        {
            MaxSize = maxSize;
        }

        public async Task<int> SendAsync(string message)
        {
            byte[] vPacket = BuildVirtualPacket(message);

            int totalSentByteCount = 0;
            while (totalSentByteCount < vPacket.Length)
            {
                int sentByteCount = await Client.SendAsync(
                    vPacket.AsMemory(totalSentByteCount, vPacket.Length - totalSentByteCount),
                    SocketFlags.None
                );

                if (sentByteCount == 0)
                {
                    throw new ConnectionClosedException();
                }

                totalSentByteCount += sentByteCount;
            }

            return totalSentByteCount;
        }

        public Task<int> SendObjectAsync<T>(T obj)
        {
            string json = JsonSerializer.Serialize(obj, _jsonOptions);
            return SendAsync(json);
        }

        private byte[] BuildVirtualPacket(string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            byte[] messageSizeBytes = BitConverter.GetBytes(messageBytes.Length);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(messageSizeBytes);
            }

            byte[] vPacket = new byte[4 + messageBytes.Length];

            Array.Copy(messageSizeBytes, 0, vPacket, 0, 4);
            Array.Copy(messageBytes, 0, vPacket, 4, messageBytes.Length);

            return vPacket;
        }

        public async Task<T?> ReceiveObjectAsync<T>()
        {
            byte[] messageBytes = await ReceiveAsync();

            return JsonSerializer.Deserialize<T>(messageBytes, _jsonOptions);
        }

        public async Task<byte[]> ReceiveAsync()
        {
            byte[] messageSizeBytes = await ReceiveExactAsync(4);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(messageSizeBytes);
            }

            int messageSize = BitConverter.ToInt32(messageSizeBytes);

            if (messageSize <= 0)
            {
                throw new InvalidMessageException("Message size must be greater than 0.");
            }

            if (messageSize > MaxSize)
            {
                throw new InvalidMessageException($"Message size cannot greater than {MaxSize}.");
            }

            byte[] messageBytes = await ReceiveExactAsync(messageSize);

            return messageBytes;
        }

        private async Task<byte[]> ReceiveExactAsync(int size)
        {
            byte[] buffer = new byte[size];

            int totalReceivedByteCount = 0;

            while (totalReceivedByteCount < size)
            {
                int receivedByteCount = await Client.ReceiveAsync(buffer.AsMemory(totalReceivedByteCount, size - totalReceivedByteCount), SocketFlags.None);

                if (receivedByteCount == 0)
                {
                    throw new ConnectionClosedException();
                }

                totalReceivedByteCount += receivedByteCount;
            }

            return buffer;
        }

        public void Close()
        {
            if (Client == null) return;

            try
            {
                if (Client.Connected)
                {
                    Client.Shutdown(SocketShutdown.Both);
                }
            }
            catch { }

            Client.Close();
        }

        public int Send(string message)
        {
            byte[] vPacket = BuildVirtualPacket(message);

            int totalSentByteCount = 0;
            while (totalSentByteCount < vPacket.Length)
            {
                int sentByteCount = Client.Send(vPacket, totalSentByteCount, vPacket.Length - totalSentByteCount, SocketFlags.None);

                if (sentByteCount == 0)
                {
                    throw new ConnectionClosedException();
                }

                totalSentByteCount += sentByteCount;
            }

            return totalSentByteCount;
        }

        public int SendObject<T>(T obj)
        {
            string json = JsonSerializer.Serialize(obj, _jsonOptions);
            return Send(json);
        }

        public T? ReceiveObject<T>()
        {
            byte[] messageBytes = Receive();

            return JsonSerializer.Deserialize<T>(messageBytes, _jsonOptions);
        }

        public byte[] Receive()
        {
            byte[] messageSizeBytes = ReceiveExact(4);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(messageSizeBytes);
            }

            int messageSize = BitConverter.ToInt32(messageSizeBytes);

            if (messageSize <= 0)
            {
                throw new InvalidMessageException("Message size must be greater than 0.");
            }

            if (messageSize > MaxSize)
            {
                throw new InvalidMessageException($"Message size cannot greater than {MaxSize}.");
            }

            byte[] messageBytes = ReceiveExact(messageSize);

            return messageBytes;
        }

        public byte[] ReceiveExact(int size)
        {
            byte[] buffer = new byte[size];

            int totalReceivedByteCount = 0;

            while (totalReceivedByteCount < size)
            {
                int receivedByteCount = Client.Receive(buffer, totalReceivedByteCount, size - totalReceivedByteCount, SocketFlags.None);

                if (receivedByteCount == 0)
                {
                    throw new ConnectionClosedException();
                }

                totalReceivedByteCount += receivedByteCount;
            }

            return buffer;
        }
    }
}
