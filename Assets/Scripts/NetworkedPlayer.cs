using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class NetworkedPlayer : NetworkBehaviour {

    [SerializeField]
	List<Behaviour> localEnabledScripts = new List<Behaviour>();

	public void AddBehaviour (Behaviour script)
	{
		localEnabledScripts.Add (script);
		script.enabled = false;
	}

    public override void OnStartLocalPlayer()
    {
        foreach (var behaviour in localEnabledScripts)
        {
            behaviour.enabled = true;
        }

        base.OnStartLocalPlayer();
    }
}
