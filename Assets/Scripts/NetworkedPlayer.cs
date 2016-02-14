using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class NetworkedPlayer : NetworkBehaviour {

    [SerializeField]
    List<Behaviour> localEnabledScripts;

    public override void OnStartLocalPlayer()
    {
        foreach (var behaviour in localEnabledScripts)
        {
            behaviour.enabled = true;
        }

        base.OnStartLocalPlayer();
    }
}
