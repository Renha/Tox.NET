using System;
using System.Net;
using Tox.Network;
using Tox.Sodium;

namespace Tox
{
    public class Tox : IDisposable
    {
        internal NetCore Net { get; private set; }
        private bool _running = false;

        public Tox(bool enableIpv6)
        {
            Net = new NetCore(enableIpv6);
        }

        public void Dispose()
        {
            Stop();
            Net.Dispose();
        }

        public void Start()
        {
            if (!_running)
                Net.Start();
        }

        public void Stop()
        {
            if (_running)
            {
                _running = false;
                Net.Stop();
            }
        }

        public void Bootstrap(IPEndPoint endpoint, string publicKey)
        {
            Bootstrap(endpoint, Tools.StringToHexBin(publicKey));
        }

        public void Bootstrap(IPEndPoint endpoint, byte[] publicKey)
        {
            if (publicKey.Length != CryptoBox.PublicKeySize)
                throw new ArgumentException("Public key should be 32 bytes long", "publicKey");

            Net.Bootstrap(endpoint, publicKey);
        }
    }
}
