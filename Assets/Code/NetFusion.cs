using UnityEngine;
using System.Collections;

public class NetFusion {

    static bool useUNet = true;

    public static bool IsServer (bool isServer)
    {
        if (useUNet)
        {
            return isServer;
        }
        else
        {
            return Network.isServer;
        }
    }

    public static bool isAuthoritative(bool isLocalPlayer, RealOwner realOwner)
    {

        if (useUNet)
        {
            if (isLocalPlayer)
                return true;

            return false;
        }
        else
        {
            if (Network.peerType == NetworkPeerType.Disconnected)
                return true;
            else
                if (Network.player == realOwner.owner)
                return true;

            return false;
        }
    }
}
