using UnityEngine;
using System.Collections;

using UnityEngine.Networking;

public class RealOwner : NetworkBehaviour {

	public NetworkPlayer owner;
	public Behaviour characterControls;

	void Awake()
	{
		this.enabled = false;
		characterControls = GetComponent<PlatformUserControl>();
	}

	//[RPC]
	//void SetCharacterControlsOwner(NetworkPlayer player)
	//{
	//	owner = player;
	//	if (owner == Network.player)
	//	{
	//		//Hey thats us! We can control this player: enable this script (this enables Update());
	//		characterControls.enabled = true;
	//	}
	//}

    [ClientRpc]
    void RpcSetCharacterControlsOwner(int connectionId)
    {
        //NetworkConnection netCon;
        //netCon.connectionId;
        //owner = player;
        if (NetFusion.isAuthoritative(isLocalPlayer, this))
        { 
            //Hey thats us! We can control this player: enable this script (this enables Update());
            characterControls.enabled = true;
        }
    }

}
