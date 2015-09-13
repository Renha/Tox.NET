using System;
using System.Net.Sockets;
using System.Net;

namespace Tox
{
    #if __MonoCS__
    public static class IPAddressExtensions
    {
        public static IPAddress MapToIPv6(this IPAddress address)
        {
            if (address.AddressFamily == AddressFamily.InterNetworkV6)
                return address;
            
            if (address.AddressFamily != AddressFamily.InterNetwork)
                System.Diagnostics.Debugger.Break();

            byte[] ipv4 = address.GetAddressBytes();
            byte[] ipv6 = new byte[16];

            Array.Copy(new byte[] { 0xFF, 0xFF, ipv4[0], ipv4[1], ipv4[2], ipv4[3] }, 0, ipv6, 10, 6);
            return new IPAddress(ipv6);
        }
    }
    #endif
}
