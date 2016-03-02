#define DEBUGGING

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

#if UNITY_EDITOR
using UnityEnhancements;
using UnityEditor;
[CustomEditor(typeof(LayerManager))]
public class LayerManagerEditor : Editor {
	LayerManager targetObject;

	[MenuItem("SMW/ScriptableObject/LayerManager")]
	static void CreateLayerManager()
	{
		UnityEnhancements.ScriptableObjectUtility.CreateAsset<LayerManager> ();
	}

	void OnEnable()
	{
		targetObject = (LayerManager)target;
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

		if (GUILayout.Button ("Read Layers")) {
			ReadClassFields ();
		}

		if (GUILayout.Button ("Apply Layers")) {
			CreateUnityEditorLayers();
		}

		if (GUILayout.Button ("ReadLayerNamesApplyLayerId")) {
			ReadLayerNamesApplyLayerId ();
		}

		if (GUILayout.Button ("Init Layer Masks")) {
			targetObject.Init ();
			EditorUtility.SetDirty (targetObject);
			AssetDatabase.SaveAssets ();
		}
	}

	void GUIElementsBottom () {
	}

	public void ReadClassFields ()
	{
		targetObject.LayerList = new List<CustomLayer> ();

		const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
		System.Type t = targetObject.GetType();
		FieldInfo[] fields = t.GetFields (flags);


		foreach (var field in fields) {

			if (field.Name.Contains ("LayerName")) {
#if DEBUGGING
				Debug.Log ("<color=red>LayerName: " + field.Name + " = " + field.GetValue (targetObject) + "</color>");
#endif
				targetObject.LayerList.Add (new CustomLayer (field.Name, field.GetValue (targetObject).ToString ()));
			}
			else {
#if DEBUGGING
				Debug.Log ("Field: " + field.Name + " = " + field.GetValue (targetObject));
#endif
			}
				
		}
		EditorUtility.SetDirty (targetObject);
		AssetDatabase.SaveAssets ();
	}

	public void CreateUnityEditorLayers ()
	{
		foreach (CustomLayer cLayer in targetObject.LayerList) {
			TagsAndLayers.AddLayer (cLayer.Value);
		}
	}

	public void ReadLayerNamesApplyLayerId ()
	{
		const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
		System.Type t = targetObject.GetType();
		FieldInfo[] fields = t.GetFields (flags);

		foreach (var field in fields) {

			if (field.Name.EndsWith ("LayerName"))
			{
#if DEBUGGING
				Debug.Log ("<color=red>LayerName: " + field.Name + " = " + field.GetValue (targetObject) + "</color>");
#endif
				TagsAndLayers.AddLayer (field.GetValue (targetObject).ToString ());
			}
			else if (field.Name.EndsWith ("LayerId"))
			{
#if DEBUGGING
				Debug.Log ("<color=green>LayerId: " + field.Name + " = " + field.GetValue (targetObject) + "</color>");
#endif
				Debug.Log ("Init " + field.Name + "");

				string layerName = "";
				if (TryGetLayerName (field.Name, fields, targetObject, out layerName))
					field.SetValue (targetObject, LayerMask.NameToLayer (layerName));
				else
					Debug.LogError ("LayerID " + field.Name + " no corresponding LayerName Field found!");
			}
			else
			{
#if DEBUGGING
				Debug.Log ("Field: " + field.Name + " = " + field.GetValue (targetObject) + " typeof: <color=white>" + field.GetType() + "</color>");
#endif
			}


		}

		EditorUtility.SetDirty (targetObject);
		AssetDatabase.SaveAssets ();
	}

	bool TryGetLayerName (string layerIdFieldName, FieldInfo[] fields, object targetObject, out string layerName) {
		foreach (var field in fields) {
			if (field.Name.ToLower().Equals ( (layerIdFieldName.Substring (0, layerIdFieldName.Length - "LayerId".Length) + "LayerName").ToLower() )) {
				layerName = field.GetValue (targetObject).ToString ();
				return true;
			}
		}
		layerName = "";
		return false;
	}
}
#endif

[System.Serializable]
public class CustomLayer {

	[SerializeField]
	string name;		// variable reference name

	[SerializeField]
	string value;		// the actually layer name in Unity Editor

	public CustomLayer (string name, string value)
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
}

public class LayerManager : ScriptableObject {

	static LayerManager instance;

	public static LayerManager Instance {
		get
		{
			if (instance == null)
			{
				// singleton instance not defined
				instance = (LayerManager) FindObjectOfType<LayerManager>();
			}

			return instance;
				
		}
		private set { instance = value; }
	}

	void OnEnable ()
	{
		Debug.Log (this.ToString () + " initialized");
		if (Instance == null)
			Instance = this;
	}

	[SerializeField]
	List<CustomLayer> layerList;

	public List<CustomLayer> LayerList {
		get { return layerList; }
		set { layerList = value; }
	}


	// Physic Layer
	
	public LayerMask allPlayer;
	public LayerMask whatIsStaticGround;
	public LayerMask whatIsJumpOnPlatform;
	public LayerMask whatIsAllGround;
	public LayerMask whatIsWall;

	public int defaultLayerId;
	public int deathLayerId;
	public int superDeathLayerId;
	public int playerLayerId;

//	public int player1;
//	public int player2;
//	public int player3;
//	public int player4;
	
//	public int enemy;
	public int feetLayerId;
	public int headLayerId;
	public int bodyLayerId;
	public int itemLayerId;

	public int groundLayerId;
	public int groundIcyLayerId;
//	public int tagAble;
//	public int floor;
	public int blockLayerId;
	public int jumpAblePlatformLayerId;
//	public int jumpAblePlatformSaveZone;
	
	public int powerUpLayerId;
//	public int bullet;
	
	public int groundStopperLayerId;
	
//	public int fader;


	public string defaultLayerName = "Default";
	public string deathLayerName = "Death";
	public string superDeathLayerName = "SuperDeath";
	public string playerLayerName = "Player";

//	public string player1LayerName = "Player1";
//	public string player2LayerName = "Player2";
//	public string player3LayerName = "Player3";
//	public string player4LayerName = "Player4";
//	
	public string feetLayerName = "Feet";
	public string headLayerName = "Head";
	public string bodyLayerName = "Body";
	public string itemLayerName = "Item";
	public string powerUpLayerName= "PowerUp";

//	public string enemyLayerName = "Enemy";

	public string groundLayerName = "Ground";
	public string groundIcyLayerName = "GroundIcy";
//	public string tagAbleLayerName = "TagAble";
//	public string floorLayerName = "Floor";
	public string blockLayerName = "Block";
	public string jumpAblePlatformLayerName = "JumpOnPlatform";
//	public string jumpAblePlatformSaveZoneLayerName = "JumpSaveZone";

//	public string bulletLayerName = "Bullet";
	public string groundStopperLayerName = "GroundStopper";
	
//	public string faderLayerName = "Fader";
	
	public void Init()
	{
		Debug.LogWarning(this.ToString() + ": Awake() - init public layer integers, scripts layer instantiation have to be AFTER this initialisation, NOT IN AWAKE!!!" );

		defaultLayerId = LayerMask.NameToLayer(defaultLayerName);
		deathLayerId = LayerMask.NameToLayer(deathLayerName);
		superDeathLayerId = LayerMask.NameToLayer(superDeathLayerName);
		playerLayerId = LayerMask.NameToLayer(playerLayerName);
//		player1 = LayerMask.NameToLayer(player1LayerName);
//		player2 = LayerMask.NameToLayer(player2LayerName);
//		player3 = LayerMask.NameToLayer(player3LayerName);
//		player4 = LayerMask.NameToLayer(player4LayerName);

		allPlayer = 1 << playerLayerId;

//		allPlayer = 1 << player1;
//		allPlayer |= 1 << player2;
//		allPlayer |= 1 << player3;
//		allPlayer |= 1 << player4;
		
		feetLayerId = LayerMask.NameToLayer(feetLayerName);
		headLayerId = LayerMask.NameToLayer(headLayerName);
		bodyLayerId = LayerMask.NameToLayer(bodyLayerName);
		itemLayerId = LayerMask.NameToLayer(itemLayerName);
		groundStopperLayerId = LayerMask.NameToLayer(groundStopperLayerName);
		powerUpLayerId = LayerMask.NameToLayer(powerUpLayerName);

//		bullet = LayerMask.NameToLayer(bulletLayerName);

		
//		enemy = LayerMask.NameToLayer(enemyLayerName);

		groundLayerId = LayerMask.NameToLayer(groundLayerName);
		groundIcyLayerId = LayerMask.NameToLayer (groundIcyLayerName);
//		tagAble = LayerMask.NameToLayer(tagAbleLayerName);
//		floor = LayerMask.NameToLayer(floorLayerName);
		blockLayerId = LayerMask.NameToLayer(blockLayerName);
		jumpAblePlatformLayerId = LayerMask.NameToLayer(jumpAblePlatformLayerName);
//		jumpAblePlatformSaveZone = LayerMask.NameToLayer(jumpAblePlatformSaveZoneLayerName);

		whatIsStaticGround = 1 << groundLayerId;
//		whatIsGround |= 1 << tagAble;
//		whatIsGround = 1 << floor;
		whatIsStaticGround |= 1 << blockLayerId;

		whatIsJumpOnPlatform = 1 << jumpAblePlatformLayerId;

		whatIsAllGround = whatIsStaticGround;
		whatIsAllGround |= 1 << jumpAblePlatformLayerId;

		whatIsWall = whatIsStaticGround;
		

//		fader = LayerMask.NameToLayer(faderLayerName);
	}

//	void OnLevelWasLoaded()
//	{
//		Debug.LogWarning(this.ToString() + ": OnLevelWasLoaded()" );
//	}
//
//	void Start()
//	{
//		Debug.LogWarning(this.ToString() + ": Start()" );
//	}
	
}
