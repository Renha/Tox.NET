using System;
using System.Linq;
using Tox.Sodium;

namespace Tox.Network
{
    public class NodeID
    {
        public byte[] PublicKey { get; private set; }

        private NodeID(byte[] publicKey)
        {
            if (publicKey.Length != CryptoBox.PublicKeySize)
                throw new ArgumentException("Public key should be 32 bytes long", "publicKey");

            PublicKey = publicKey;
        }

        public NodeID GetClosest(NodeID node1, NodeID node2)
        {
            byte[] key = this;
            byte[] key1 = node1;
            byte[] key2 = node2;

            for (int i = 0; i < CryptoBox.PublicKeySize; i++)
            {
                byte distance1 = (byte)(key[i] ^ key1[i]);
                byte distance2 = (byte)(key[i] ^ key2[i]);

                if (distance1 < distance2)
                    return node1;

                if (distance1 > distance2)
                    return node2;
            }
            
            return this;
        }

        public static implicit operator NodeID(byte[] publicKey)
        {
            return new NodeID(publicKey);
        }

        public static implicit operator byte[](NodeID nodeID)
        {
            return nodeID.PublicKey;
        }

        public static bool operator >(NodeID node1, NodeID node2) 
        {
            byte[] key1 = node1;
            byte[] key2 = node2;

            for (int i = 0; i < CryptoBox.PublicKeySize; i++)
            {
                if (key1[i] < key2[i])
                    return false;
                else if (key1[i] > key2[i])
                    return true;
            }

            return false;
        }

        public static bool operator <(NodeID node1, NodeID node2) 
        {
            byte[] key1 = node1;
            byte[] key2 = node2;

            for (int i = 0; i < CryptoBox.PublicKeySize; i++)
            {
                if (key1[i] < key2[i])
                    return true;
                else if (key1[i] > key2[i])
                    return false;
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            var nodeID = obj as NodeID;
            if (nodeID == null)
                return false;

            if (nodeID == this)
                return true;

            return nodeID.PublicKey.SequenceEqual(this.PublicKey);
        }
    }
}