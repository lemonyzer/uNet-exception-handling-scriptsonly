#define DEBUGGING
#undef DEBUGGING

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEnhancements;
using System.Reflection;
#endif

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(TagManager))]
public class TagManagerEditor : Editor {
	TagManager targetObject;

	[MenuItem("SMW/ScriptableObject/TagManager")]
	static void CreateLayerManager()
	{
		UnityEnhancements.ScriptableObjectUtility.CreateAsset<TagManager> ();
	}

	void OnEnable()
	{
		targetObject = (TagManager)target;
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

		if (GUILayout.Button ("Ready Tag Fields")) {
			ReadClassFields();
		}

		if (GUILayout.Button ("Apply Tags in Unity Editor")) {
			CreateUnityEditorTags ();
		}

		if (GUILayout.Button ("Check")) {
		}
	}

	void GUIElementsBottom () {
	}

	public void ReadClassFields ()
	{
		targetObject.TagList = new List<CustomTag> ();

		const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
		System.Type t = targetObject.GetType();
		FieldInfo[] fields = t.GetFields (flags);


		foreach (var field in fields) {
#if DEBUGGING
Debug.Log ("Field: " + field.Name + " = " + field.GetValue (this));
#endif
			if (field.Name.StartsWith ("tag_"))
			{
				targetObject.TagList.Add (new CustomTag (field.Name, field.GetValue (targetObject).ToString ()));
			}
		}
	}

	public void CreateUnityEditorTags ()
	{
		foreach (CustomTag cTag in targetObject.TagList) {
			TagsAndLayers.AddTag (cTag.Value);
		}
	}
}
#endif

[System.Serializable]
public class CustomTag {

	[SerializeField]
	string name;		// variable reference name

	[SerializeField]
	string value;		// the actually tag name in Unity Editor

	public CustomTag (string name, string value)
	{
		this.name = name;
		this.value = value;
	}

	public string Name {
		get { return name; }
		set { name = value; }
	}

	public string Value {
		get { return this.value; }
		set { this.value = value; }
	}
//	string tag;
//	int id;
}



[System.Serializable]
public class TagManager : ScriptableObject {

	static TagManager instance;

	public static TagManager Instance {
		get
		{
			if (instance == null)
			{
				// singleton instance not defined
				instance = (TagManager) FindObjectOfType<TagManager>();
			}

			return instance;

		}
		private set { instance = value; }
	}

	void OnEnable ()
	{
		if (Instance == null)
			Instance = this;
	}

	[SerializeField]
	List<CustomTag> tagList;

	public List<CustomTag> TagList {
		get { return tagList; }
		set { tagList = value; }
	}


	public string guiAnalogStick = "AnalogStick";
	public string guiStick = "Stick";

	public string tag_Map = "Map";
	public string tag_MapBackgroundLayer = "MapBackgroundLayer";
	public string tag_MapLayer0 = "MapLayer0";
	public string tag_MapLayer1 = "MapLayer1";
	public string tag_MapLayer2 = "MapLayer2";
	public string tag_MapLayer3 = "MapLayer3";
	

	/**
	 * Body Parts (Parent and Children)
	 **/
	public string name_player = "Player";
	public string tag_player = "Player";
	public string tag_player_clone = "Player Clone";
	public string name_cloneLeft = "Clone Left";
	public string tag_cloneLeft = "Clone Left";
	public string name_cloneRight = "Clone Right";
	public string tag_cloneRight = "Clone Right";
	public string name_CloneTop = "Clone Top";
	//public string tag_CloneTop = "Clone Top";
	public string name_CloneBottom = "Clone Bottom";
	//public string tag_CloneBottom = "Clone Bottom";
	public string name_head = "Head";
	public string tag_head = "Head";
	public string name_body = "Body";
	public string tag_body = "Body";
	public string name_feet = "Feet";
	public string tag_feet = "Feet";
	public string name_itemCollector = "ItemCollector";
	public string tag_itemCollector = "ItemCollector";
	public string name_powerUpHitArea = "PowerUpHitArea";
	public string tag_powerUpHitArea = "PowerUpHitArea";
	public string name_groundStopper = "GroundStopper";
	public string tag_groundStopper = "GroundStopper";
	public string name_king = "King";
	public string tag_king = "King";

	//public string boxCollider = "BoxCollider";
	public string name_lastReceivedPos = "LastRecvedPos";
	public string tag_lastReceivedPos = "";
	public string name_CurrentEstimatedPosOnServer = "CurrentEstimatedPosOnServer";
	public string tag_CurrentEstimatedPosOnServer = "";
	public string name_PredictedPosSimulatedWithLastInput = "PredictedPosSimulatedWithLastInput";
	public string tag_PredictedPosSimulatedWithLastInput = "";
	public string name_PredictedPosCalculatedWithLastInput = "PredictedPosCalculatedWithLastInput";
	public string tag_PredictedPosCalculatedWithLastInput = "";
	public string name_PredictedPosV3 = "PredictedPosV3";
	public string tag_PredictedPosV3 = "";
	public string name_iceWalled = "IceWalled";
	public string tag_iceWalled = "";

	/**
	 * Scene, Level Parts
	 **/
	public string name_gameController = "GameController";
	public string tag_gameController = "GameController";
//	public string itemLibrary = "ItemLibrary";
	public string name_background = "Background";
	public string tag_background = "Background";
	public string name_invincibleSound = "InvincibleSound";
	public string tag_invincibleSound = "InvincibleSound";
	public string name_powerUp = "PowerUp";
	public string tag_powerUp = "PowerUp";
	public string name_fader = "Fader";
	public string tag_fader = "Fader";
	public string name_countDown = "CountDown";
	public string tag_countDown = "CountDown";

//	/**
//	 * Character Selection
//	 **/
//	public string tag_CharacterPreview = "ChararacterPreview";
//	public string character = "Character";
//	public string ai = "AI";
//	public string death = "Death";
//	public string enemy = "Enemy";
//
//	
//	/**
//	 * Team Selection
//	 **/
//	public string Team1 = "Team1";
//	public string TeamRed = "TeamRed";
//
//	public string Team2 = "Team2";
//	public string TeamGreen = "TeamGreen";
//
//	public string Team3 = "Team3";
//	public string TeamYellow = "TeamYellow";
//
//	public string Team4 = "Team4";
//	public string TeamBlue = "TeamBlue";

	
}
