using System;
using System.Net.Sockets;
using System.Net;

namespace Tox.Network
{
    internal class ToxSocket : Socket
    {
        public bool Ipv6Enabled { get; private set; }

        public ToxSocket(bool enableIpv6)
            : base(enableIpv6 ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
        {
            int n = 1024 * 1024 * 2;
            SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, n);
            SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, n);
            SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);

            //this shouldn't be needed in mono
            #if !__MonoCS__
            SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, 0);
            #endif

            Blocking = false;
            Ipv6Enabled = enableIpv6;
        }

        public bool TryBind(Tuple<int, int> range)
        {
            for (int port = range.Item1; port < range.Item2; port++)
            {
                try
                {
                    Bind(new IPEndPoint(Ipv6Enabled ? IPAddress.IPv6Any : IPAddress.Any, port));

                    return true;
                }
                catch { }
            }

            return false;
        }
    }
}
