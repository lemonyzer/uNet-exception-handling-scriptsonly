using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class PlatformCharacterScript : NetworkBehaviour {

    Vector2 inputX;
    bool inputJump;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (isLocalPlayer)
        {
            ReadInput();
        }
    }

    void ReadInput ()
    {
        inputX.x = Input.GetAxis("Horizontal");
        inputJump = Input.GetButton("Jump");
    }

    void FixedUpdate ()
    {

    }

    internal void StartBetterSpawnDelay()
    {
        throw new NotImplementedException();
    }

    internal void Protection()
    {
        throw new NotImplementedException();
    }

    public void SetSmwCharacterSO(SmwCharacter characterSO)
    {
        throw new NotImplementedException();
    }
}
