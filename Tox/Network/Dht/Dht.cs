using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using Tox.Sodium;
using Tox.Network.Packets;

namespace Tox.Network
{
    internal class Dht : IDisposable
    {
        private List<DhtFriend> _friends = new List<DhtFriend>(64);
        private object _friendsLock = new object();
        private NetCore _net;
        private DateTime _lastGetNodes;

        public KeyPair KeyPair { get; private set; }

        public ReadOnlyCollection<DhtFriend> Friends
        {
            get
            {
                lock (_friendsLock)
                    return _friends.AsReadOnly();
            }
        }

        public Dht(NetCore net)
        {
            _net = net;
            _net.RegisterHandler(PacketID.SendNodesIpv6, new PacketHandler(HandleSendNodes));
            _net.RegisterHandler(PacketID.GetNodes, new PacketHandler(HandleGetNodes));
            _net.RegisterHandler(PacketID.PingRequest, new PacketHandler(HandlePingRequest));
            _net.RegisterHandler(PacketID.PingResponse, new PacketHandler(HandlePingResponse));

            KeyPair = CryptoBox.GenerateKeyPair();
        }

        public void Dispose()
        {
            
        }

        public DhtFriend GetNode(IPEndPoint endpoint, byte[] publicKey)
        {
            lock (_friendsLock)
            {
                var friend = _friends.FirstOrDefault((f) => f.PublicKey.Equals((NodeID)publicKey));
                if (friend == null)
                    return null;

                //check if the endpoint is still the same, if not, update it
                if (!friend.EndPoint.Equals(endpoint))
                    friend.EndPoint = endpoint;

                return friend;
            }
        }

        private DhtFriend AddNode(IPEndPoint endpoint, byte[] publicKey)
        {
            lock (_friendsLock)
            {
                var friend = GetNode(endpoint, publicKey);
                if (friend != null)
                    return friend;

                //if the list is full, remove a node that's further away
                //TODO: actually remove the node with the biggest distance difference
                int index = _friends.Count;
                if (_friends.Count == _friends.Capacity)
                {
                    if (ShouldInsert(publicKey, ref index))
                        return null;

                    if (_friends.Count == _friends.Capacity)
                        _friends.RemoveAt(index);
                }

                byte[] sharedKey = CryptoBox.BeforeNm(publicKey, KeyPair.SecretKey);
                friend = new DhtFriend(endpoint, publicKey, sharedKey);

                _friends.Insert(index, friend);
                Console.WriteLine("Added a new node to our close list: {0}", endpoint);

                return friend;
            }
        }

        public DhtFriend GetNode(IPEndPoint endpoint)
        {
            lock (_friendsLock)
            {
                return _friends.FirstOrDefault((f) => f.EndPoint.Equals(endpoint));
            }
        }

        public void Do()
        {
            lock (_friendsLock)
            {
                //send pings if needed
                var now = DateTime.Now;
                var toRemove = new List<DhtFriend>();

                foreach (var friend in _friends)
                {
                    if ((now - friend.LastSentPingTime).TotalSeconds >= 60)
                    {
                        SendPingRequest(friend);
                        continue;
                    }

                    //check for timeout
                    if ((now - friend.LastPingTime).TotalSeconds >= 122 && (now - friend.LastSentPingTime).TotalSeconds >= 30)
                    {
                        toRemove.Add(friend);
                        continue;
                    }
                }

                foreach (var friend in toRemove)
                    _friends.Remove(friend);

                //send one getnodes request to a random node every 20 seconds
                if ((now - _lastGetNodes).TotalSeconds >= 20 && _friends.Count != 0)
                {
                    int index = new Random().Next(_friends.Count);
                    SendGetNodes(_friends[index], KeyPair.PublicKey);
                }
            }
        }

        private bool ShouldInsert(NodeID nodeID, ref int index)
        {
            //we don't have to lock here because this method is only called from GetNode which already locks _friendsLock
            for (int i = 0; i < _friends.Count; i++)
            {
                var selfNode = (NodeID)KeyPair.PublicKey;
                if (selfNode.GetClosest(_friends[i].PublicKey, nodeID) == nodeID)
                {
                    index = i;
                    return true;
                }
            }

            return false;
        }

        public void Bootstrap(IPEndPoint endpoint, byte[] publicKey)
        {
            if (_net.Ipv6Enabled)
                endpoint.Address = endpoint.Address.MapToIPv6();

            var friend = AddNode(endpoint, publicKey);
            SendGetNodes(friend, publicKey);
        }

        private void SendGetNodes(DhtFriend friend, byte[] publicKey)
        {
            var packet = new GetNodesRequest(KeyPair.PublicKey, publicKey);
            friend.AddPing(packet.PingID);

            _net.SendPacket(friend, packet);
            _lastGetNodes = DateTime.Now;
        }

        private void SendPingRequest(DhtFriend friend)
        {
            var pingRequest = new PingRequest(KeyPair.PublicKey, CryptoRandom.NextUInt64());
            friend.AddPing(pingRequest.PingID);

            _net.SendPacket(friend, pingRequest);
        }

        private void SendPingResponse(DhtFriend friend, ulong pingID)
        {
            var response = new PingResponse(KeyPair.PublicKey, pingID);
            _net.SendPacket(friend, response);
        }

        private void HandleSendNodes(IPEndPoint endpoint, byte[] data)
        {
            Console.WriteLine("Received send nodes response from: {0}", endpoint);

            byte[] publicKey = new byte[32];
            Array.Copy(data, 1, publicKey, 0, publicKey.Length);

            var friend = GetNode(endpoint, publicKey);
            if (friend == null)
                return;

            //ignore sendnodes packets we didn't ask for
            var packet = new SendNodesResponse(data, friend.SharedKey);
            if (!friend.ContainsPing(packet.PingID))
                return;
            else
                friend.RemovePing(packet.PingID);

            foreach (var node in packet.Nodes)
            {
                //don't send getnodes request to nodes we already know
                if (GetNode(node.Item1, node.Item2) != null)
                    continue;

                //we don't want ipv6 address to end up in our list if we have ipv6 disabled
                if (!_net.Ipv6Enabled && node.Item1.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    continue;

                Bootstrap(node.Item1, node.Item2);
            }
        }

        private void HandlePingRequest(IPEndPoint endpoint, byte[] data)
        {
            Console.WriteLine("Received ping request from: {0}", endpoint);

            //we can't respond to a ping request of an unknown client
            var friend = GetNode(endpoint);
            if (friend == null)
                return;

            var request = new PingRequest(data, friend.SharedKey);

            //send a pong back immediately
            SendPingResponse(friend, request.PingID);
        }

        private void HandlePingResponse(IPEndPoint endpoint, byte[] data)
        {
            Console.WriteLine("Received ping response from: {0}", endpoint);

            var friend = GetNode(endpoint);
            if (friend == null)
                return;

            var response = new PingResponse(data, friend.SharedKey);

            if (friend.ContainsPing(response.PingID))
                friend.RemovePing(response.PingID);
        }

        private void HandleGetNodes(IPEndPoint endpoint, byte[] data)
        {
            Console.WriteLine("Received getnodes request from: {0}", endpoint);
        }
    }
}
