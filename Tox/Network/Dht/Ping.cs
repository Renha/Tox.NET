using System;

namespace Tox.Network
{
    internal class Ping
    {
        public DateTime Time { get; private set; }
        public ulong ID { get; private set; }
        public bool Confirmed { get; private set; }

        public Ping(DateTime time, ulong id)
        {
            Time = time;
            ID = id;
        }
    }
}