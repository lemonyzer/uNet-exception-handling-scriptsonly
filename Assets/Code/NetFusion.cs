using UnityEngine;
using UnityEngine.Networking;
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

    public static bool isAuthoritative(NetworkIdentity identity, RealOwner realOwner)
    {
        if (identity != null)
            return isAuthoritative(identity.isLocalPlayer, realOwner);
        else
            return true;
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
