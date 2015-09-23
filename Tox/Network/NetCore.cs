using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using Tox.Network.Packets;

namespace Tox.Network
{
    internal class NetCore : IDisposable
    {
        public bool Ipv6Enabled { get; private set; }
        public ToxSocket Socket { get; private set; }

        private Dht _dht;
        private bool _running;
        private Dictionary<PacketID, List<PacketHandler>> _handlers = new Dictionary<PacketID, List<PacketHandler>>();
        private CancellationTokenSource _cancelToken = new CancellationTokenSource();

        public NetCore(bool enableIpv6)
        {
            Ipv6Enabled = enableIpv6;

            Socket = new ToxSocket(enableIpv6);
            _dht = new Dht(this);

            var range = Tuple.Create<int, int>(33445, 33545);
            if (!Socket.TryBind(range))
                throw new Exception(string.Format("Could not bind to any of the ports in range: {0} - {1}", range.Item1, range.Item2));
        }

        public void Dispose()
        {
            Stop();
            _dht.Dispose();
        }

        public void Start()
        {
            if (!_running)
                Listen();
        }

        public void Stop()
        {
            if (_running && _cancelToken != null)
            {
                _running = false;
                _cancelToken.Cancel();
            }
        }

        private void Listen()
        {
            _running = true;

            Task.Factory.StartNew(async() =>
                {
                    while (_running)
                    {
                        //check for a cancel signal
                        if (_cancelToken != null && _cancelToken.IsCancellationRequested)
                            break;
                        
                        while (Socket.Available > 0)
                        {
                            var endpoint = (EndPoint)new IPEndPoint(Ipv6Enabled ? IPAddress.IPv6Any : IPAddress.Any, 0);
                            byte[] buffer = new byte[Socket.Available]; //should be plenty r-right?
                            int received;

                            try { received = Socket.ReceiveFrom(buffer, ref endpoint); }
                            catch { continue; /* can happen if the buffer is too small */ }

                            if (received < buffer.Length)
                            {
                                byte[] newBuffer = new byte[received];
                                Array.Copy(buffer, 0, newBuffer, 0, newBuffer.Length);
                                buffer = newBuffer;
                            }
                            else if (received > buffer.Length)
                            {
                                throw new Exception("Received more data than our buffer could hold, amazing");
                            }

                            HandleData((IPEndPoint)endpoint, buffer);
                        }

                        //check if it's time to send pings again
                        _dht.DoPings();

                        await Task.Delay(50);
                    }
                }, _cancelToken.Token);
        }

        private void HandleData(IPEndPoint endpoint, byte[] packet)
        {
            var packetID = (PacketID)packet[0];

            if (_handlers.ContainsKey(packetID))
                _handlers[packetID].ForEach(h => h.Invoke(endpoint, packet));
        }

        public void RegisterHandler(PacketID packetID, PacketHandler handler)
        {
            if (!_handlers.ContainsKey(packetID))
                _handlers.Add(packetID, new List<PacketHandler>());

            _handlers[packetID].Add(handler);
        }

        public void SendPacket(DhtFriend friend, IToxPacket packet)
        {
            byte[] data = packet.Pack(friend.SharedKey);
            Socket.SendTo(data, friend.EndPoint);
        }

        public void Bootstrap(IPEndPoint endpoint, byte[] publicKey)
        {
            _dht.Bootstrap(endpoint, publicKey);
        }
    }
}