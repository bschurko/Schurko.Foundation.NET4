using System;
using System.Net;
using System.Net.Sockets;

namespace Schurko.Foundation.Network.Sockets
{
    public abstract class SocketClientBase : SocketBase, IDisposable
    {
        public abstract Socket Client { get; }

        public abstract bool Connect(IPEndPoint serverEndPoint);
        public abstract bool Connect(IPAddress serverAddress, int serverPort);
        public abstract bool Connect(string serverNameOrAddress, int serverPort);
        public abstract bool Connect();

        public abstract void ConnectAsync(IPEndPoint serverEndPoint);
        public abstract void ConnectAsync(IPAddress serverAddress, int serverPort);
        public abstract void ConnectAsync(string serverNameOrAddress, int serverPort);
        public abstract void ConnectAsync();

        public abstract void Disconnect();
        public abstract void SendData(byte[] data);

        public abstract void Dispose();
    }
}
