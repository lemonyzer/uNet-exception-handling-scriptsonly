using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;
using System.Collections;

// 
[CustomEditor(typeof(NetworkedPlayer))]
public class NetworkedPlayerEditor : Editor {

	NetworkedPlayer targetObject;
	NetworkTransform netTransform;

	void OnEnable()
	{
		targetObject = (NetworkedPlayer)target;
		netTransform = targetObject.GetComponent<NetworkTransform> ();

	}

	public override void OnInspectorGUI()
	{
		//base.OnInspectorGUI ();
		GUIElementsTop();

		DrawDefaultInspector();

		GUILayout.Space(10);
		GUIElementsBottom();
	}

	void GUIElementsTop () {
		GUILayout.BeginVertical ();
		GUILayout.Label ("SendIntervall = " + netTransform.sendInterval);
		GUILayout.Label ("movementTheshold = " + netTransform.movementTheshold);
		GUILayout.Label ("snapThreshold = " + netTransform.snapThreshold);
		GUILayout.Label ("syncRotationAxis = " + netTransform.syncRotationAxis);

		GUILayout.EndVertical ();

		if (GUILayout.Button ("SendInterval")) {
			Debug.Log (netTransform.sendInterval);
		}


	}

	void GUIElementsBottom () {
	}
}
