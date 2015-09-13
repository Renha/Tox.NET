using System;

namespace Tox.Network.Packets
{
    enum PacketID : byte
    {
        PingRequest = 0,
        PingResponse = 1,
        GetNodes = 2,
        SendNodesIpv6 = 4,
        CookieRequest = 24,
        CookieResponse = 25,
        CryptoHandhake = 26,
        CryptoData = 27,
        Crypto = 32,
        LanDiscovery = 33,
        OnionSendInitial = 128,
        OnionSend1 = 129,
        OnionSend2 = 130,
        AnnounceRequest = 131,
        AnnounceResponse = 132,
        OnionDataRequest = 133,
        OnionDataResponse = 134,
        OnionReceive3 = 140,
        OnionReceive2 = 141,
        OnionReceive1 = 142,
        BootstrapInfo = 240
    }
}