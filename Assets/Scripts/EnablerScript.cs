using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnablerScript : MonoBehaviour {

	[SerializeField]
	List<GameObject> gameObjects = new List<GameObject>();

	// Use this for initialization
	void Start () {
		Activate ();
	}

	void Activate ()
	{
		foreach (GameObject go in gameObjects)
			go.SetActive (true);
	}

}
