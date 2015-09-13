using System;

namespace Tox.NET
{
    public class PortRange
    {
        internal class PortRange
        {
            internal int MinPort {get;set;}
            internal int MaxPort{get;set;}

            internal PortRange(int minPort, int maxPort)
            {
                MinPort = minPort;
                MaxPort = maxPort;
            }
        }
    }
}
