using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEnhancements;

public class CharacterEditor : EditorWindow {

    [MenuItem("SMW/Single Character Editor")]
    static void Init()
    {
        GetWindow(typeof(CharacterEditor));
    }

    bool debug = false;

    void OnGUI()
    {
        debug = EditorGUILayout.Toggle("Debugging", debug);

        sliderPosition = EditorGUILayout.BeginScrollView(sliderPosition);

        window_SmwCharacterGenerics = EditorGUILayout.ObjectField("SmwCharacterGenerics", window_SmwCharacterGenerics, typeof(SmwCharacterGenerics), false) as SmwCharacterGenerics;

        InitFoldStyle();    // TODO put in OnEnable

        showCharacterEditor = EditorGUILayout.Foldout(showCharacterEditor, "Single Character Editor", myFoldoutStyle);
        if (showCharacterEditor)
            OnGUI_CharacterEditor();

        showSpritesheetEditor = EditorGUILayout.Foldout(showSpritesheetEditor, "Spritesheet Editor", myFoldoutStyle);
        if (showSpritesheetEditor)
            OnGUI_SpritesheetEditor();

        showAnimatorEditor = EditorGUILayout.Foldout(showAnimatorEditor, "Animator Editor", myFoldoutStyle);
        if (showAnimatorEditor)
            OnGUI_AnimatorEditor();

        showPrefabEditor = EditorGUILayout.Foldout(showPrefabEditor, "Prefab Editor", myFoldoutStyle);
        if (showPrefabEditor)
            OnGUI_PrefabEditor();

        EditorGUILayout.EndScrollView();
    }

    void OnGUI_CharacterEditor()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("SMW Character Editor", EditorStyles.boldLabel);

        if (window_SmwCharacter != null)
        {
            // wenn ein SMW Character gesetzt ist
            if (GUILayout.Button("Show SMW Character SO"))
            {
                // ... kann man die Datei im ProjectWindow (Datei Explorer) öffnen
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = window_SmwCharacter;
            }
        }
        if (GUILayout.Button("New Character"))
        {
            // neuen Character erzeugen
            window_SmwCharacter = CreateSmwCharacter.CreateAssetAndSetup();
        }
        GUILayout.EndHorizontal();

        window_SmwCharacter = EditorGUILayout.ObjectField("SMW Character SO", window_SmwCharacter, typeof(SmwCharacter), false) as SmwCharacter;
    }

    void OnGUI_SpritesheetEditor ()
    {

        GUILayout.Label("Spritesheet Editor", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        spritesheet = EditorGUILayout.ObjectField("Unsliced Sprite", spritesheet, typeof(Sprite), false) as Sprite;
        GUILayout.EndHorizontal();
        //		bool canSetupSprite = false;
        TextureImporter myImporter = null;
        if (spritesheet != null)
        {
            myImporter = TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(spritesheet)) as TextureImporter;
            if (myImporter != null)
            {
                if (SpriteIsPrepared(myImporter))
                {
                    GUILayout.Label("Sprite is prepared! Subsprites = " + myImporter.spritesheet.Length);
                }
                else
                {
                    GUILayout.Label("Sprite is not prepared! Subsprites = " + myImporter.spritesheet.Length);
                }

                // GUI SubSpriteCount
                subSpritesCount = EditorGUILayout.IntSlider("Sub Sprites #", subSpritesCount, 1, 6);

                // GUI pixelPerUnit
                pixelPerUnit = EditorGUILayout.IntField("Pixel per Unit", pixelPerUnit);

                GUILayout.BeginHorizontal();
                // GUI: Pivot
                bool current = GUI.enabled;
                spriteAlignment = (SpriteAlignment)EditorGUILayout.EnumPopup("Pivot", spriteAlignment);
                if (spriteAlignment != SpriteAlignment.Custom)
                {
                    // deaktiviere custom Offset
                    GUI.enabled = false;
                }
                // GUI: Custom Pivot
                customOffset = EditorGUILayout.Vector2Field("Custom Offset", customOffset);
                GUI.enabled = current;
                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Sprite Info"))
                {
                    // Spriteinfo ausgeben
                    SpriteAssetInfo(myImporter);
                }
                if (GUILayout.Button("meta. Slice"))
                {
                    //Grid Slice
                    PerformMetaSlice(myImporter);
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.Label("Error: select other Sprite");
            }
        }


        //TODO
        //TODO aktuell wird nicht direkt das Sprite [multiple] als Asset übergeben!!!
        //TODO
        if (myImporter != null &&
            SpriteIsPrepared(myImporter))
        {
            GUI.enabled = true;
            GUILayout.Label("Sprite ist vorbereitet. :)");
        }
        else
        {
            GUI.enabled = false;
            GUILayout.Label("Sprite muss vorbereitet werden!");
        }
        selectedTeam = (Teams)EditorGUILayout.EnumPopup("Team:", selectedTeam);
        if (selectedTeam == Teams.count)
            selectedTeam = Teams.yellow;

        if (GUILayout.Button("Add Spritesheet to Character"))
        {
            AddSpritesheetToSmwCharacterSO(window_SmwCharacter, selectedTeam, myImporter.assetPath);
        }

    }

    void OnGUI_AnimatorEditor ()
    {
        if (allowedToCreateAnimatorController(selectedTeam))
            GUI.enabled = true;
        else
            GUI.enabled = false;
        if (GUILayout.Button("create RuntimeAnimatorController"))
        {
            // create Prefab
            CharacterAnimator.Create(window_SmwCharacterGenerics, window_SmwCharacter, selectedTeam);
        }
    }

    void OnGUI_PrefabEditor()
    {
        networked = EditorGUILayout.Toggle("for Network", networked);

        if (allowedToCreateCharacterPrefab(selectedTeam))
            GUI.enabled = true;
        else
            GUI.enabled = false;
        window_KeepCreatedPrefabsInScene = GUILayout.Toggle(window_KeepCreatedPrefabsInScene, "Keep created prefab In Scene?");

        if (GUILayout.Button("create Prefab"))
        {
            // create Prefab
            CreateCharacterPrefab(window_SmwCharacter, selectedTeam, window_SmwCharacterGenerics, window_KeepCreatedPrefabsInScene);
        }
    }

    bool SpriteIsPrepared(TextureImporter myImporter)
    {
        if (myImporter.spritePixelsPerUnit == pixelPerUnit &&
           //myImporter.spritePivot == GetPivotValue(spriteAlignment, customOffset) &&
           myImporter.spritesheet[0].pivot == PositionTools.GetPivotValue(spriteAlignment, customOffset) &&
           myImporter.spriteImportMode == SpriteImportMode.Multiple &&
           myImporter.spritesheet.Length == 6)
        {
            return true;
        }
        return false;
    }


    private bool AddSpritesheetToSmwCharacterSO(SmwCharacter currentCharacter, Teams teamId, string relSpritePath)
    {
        //		Debug.Log("Loading Sprites @ " + relSpritePath);
        //					slicedSprite = AssetDatabase.LoadAllAssetRepresentationsAtPath (myImporter.assetPath) as Sprite[];
        //slicedSprite = ((Sprite)AssetDatabase.LoadAllAssetsAtPath(myImporter.assetPath)) //.Of //OfType<Sprite>().ToArray();

        UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(relSpritePath);

        Sprite[] slicedSprites = null;
        if (assets != null)
        {
            if (assets.Length > 1)
            {
                Debug.Log("SubAssets Anzahl = " + assets.Length);
                slicedSprites = new Sprite[assets.Length - 1];                          // generate Sprite[] aus asset
                for (int i = 1; i < assets.Length; i++)
                {
                    slicedSprites[i - 1] = assets[i] as Sprite;
                }
            }
            else
            {
                Debug.LogError("SubAssets Anzahl = " + assets.Length);
            }
        }


        if (slicedSprites != null)
        {
            Debug.Log("slicedSprites SubAssets Anzahl = " + slicedSprites.Length);
            currentCharacter.SetCharSpritesheet(teamId, slicedSprites);                             // add to SmwCharacter
            EditorUtility.SetDirty(currentCharacter);                                       // save ScriptableObject
            return true;
        }
        else
        {
            Debug.LogError("slicedSprites == null!!!");
            return false;
        }
    }


    bool allowedToCreateAnimatorController(Teams teamId)
    {
        //window_SmwCharacter != null
        //window_SmwCharacterGenerics != null
        //AnimClips set != null?
        //Spritesheet sliced ?

        if (window_SmwCharacterGenerics != null &&
           window_SmwCharacterGenerics.protectionAnimClip != null &&
           window_SmwCharacterGenerics.rageAnimClip != null &&
           window_SmwCharacterGenerics.spawnAnimClip != null &&
           window_SmwCharacter != null &&
           window_SmwCharacter.GetSprites(teamId, SmwCharacterAnimation.Spritesheet) != null &&
           window_SmwCharacter.GetSprites(teamId, SmwCharacterAnimation.Spritesheet).Length == 6)
            return true;
        else
            return false;
    }

    bool allowedToCreateCharacterPrefab(Teams teamId)
    {
        //window_SmwCharacter != null
        //window_SmwCharacterGenerics != null

        //Spritesheet sliced ?
        //RuntimeAnimator set != null ?

        if (window_SmwCharacterGenerics != null &&
           window_SmwCharacter != null &&
           window_SmwCharacter.GetRuntimeAnimationController(teamId) != null)
            return true;
        else
            return false;
    }


    GameObject CreateCharacterPrefab(SmwCharacter characterSO, Teams teamId, SmwCharacterGenerics generics, bool keepTempCreatedGoInScene)
    {
        string charName = characterSO.charName;
        if (characterSO.charName == "")
        {
            charName = "unnamedChar";
            Debug.LogError("character.charName == \"\" (leer)");
        }
        if (debug)
            Debug.Log(this.ToString() + " Create () " + charName);


        string pathRelativeToAssetsPath = "";

        if (networked)
            pathRelativeToAssetsPath = "Resources/AutoGen Characters/UnityNetwork";
        else
            pathRelativeToAssetsPath = "Prefabs/AutoGen Characters";

		string createdAssetPath = "";
		if (!UnityEnhancements.AssetTools.TryCreateFolderWithAssetDatabase (pathRelativeToAssetsPath, out createdAssetPath))
		{
			Debug.LogError("Ordner " + pathRelativeToAssetsPath + " konnte nicht erstellt werden");
			return null;
		}

        //		string pathRelativeToProject = "Assets/" + pathRelativeToAssetsPath;
        string prefabPathRelativeToProject = "Assets/" + pathRelativeToAssetsPath + "/" + charName + "_" + teamId + ".prefab";

        UnityEngine.Object emptyObj = PrefabUtility.CreateEmptyPrefab(prefabPathRelativeToProject);

        //GameObject tempObj = GameObject.CreatePrimitive(prim);
        //GameObject tempObj = new GameObject(BodyPartComponents.components.ToArray());

        // create empty
        //		GameObject tempObj = new GameObject(charName);

        // build character
        GameObject createdCharacterGO = SmartCreate(characterSO, teamId, generics);

        if (createdCharacterGO != null)
        {
            // save created GO in prefab
            GameObject createdCharacterPrefab = PrefabUtility.ReplacePrefab(createdCharacterGO, emptyObj, ReplacePrefabOptions.ConnectToPrefab);

            // destroy created GO in Scene
            if (!keepTempCreatedGoInScene)
                DestroyImmediate(createdCharacterGO);

            // return prefab
            return createdCharacterPrefab;
        }
        else
        {
            Debug.LogError("created CharacterGO ist NULL!!!");
            return null;
        }

    }


    /// <summary>
    /// Smarts the create. Create special Body Parts MultipleTimes
    /// </summary>
    /// <returns>The create.</returns>
    /// <param name="characterSO">Character S.</param>
    /// <param name="charGenerics">Char generics.</param>
    public GameObject SmartCreate(SmwCharacter characterSO, Teams teamId, SmwCharacterGenerics charGenerics)
    {
        //		Debug.Log(this.ToString() + " Create smwCharacter Name= " + characterSO.name);

        // erzeuge rootGO
        //		GameObject characterGO = new GameObject();	// wird in ChildData root erzeugt (root.gameObject)

        // erzeuge Child Liste
        childs = new List<ChildData>();

        // fülle root und Child Liste
        fillRootAndChildData(characterSO, teamId, charGenerics);

        // lese Child Liste aus und erzeuge childGO's
        foreach (ChildData child in childs)
        {
            //connect childGO with characterGO
            child.gameObject.transform.SetParent(root.gameObject.transform);

            // currentChildGO finish
        }

        return root.gameObject;
    }

    public void fillRootAndChildData(SmwCharacter characterSO, Teams teamId, SmwCharacterGenerics charGenerics)
    {

        float leftPos = -20f;   // TODO inspector
        float rightPos = 20f;   // TODO inspector
        float topPos = 15f; // TODO inspector
        float bottomPos = -15f; // TODO inspector

        Vector3 rootTransformPos = Vector3.zero;
        Vector3 centerTransformPos = rootTransformPos;
        Vector3 leftTransformPos = new Vector3(leftPos, 0f, 0f);
        Vector3 rightTransformPos = new Vector3(rightPos, 0f, 0f);
        Vector3 topTransformPos = new Vector3(0f, topPos, 0f);
        Vector3 bottomTransformPos = new Vector3(0f, bottomPos, 0f);
        Vector3 headTransformPos = new Vector3(0f, 0.3f, 0f);
        Vector3 feetTransformPos = 			new Vector3(0f,-0.3f,0f);
        Vector3 bodyTransformPos = new Vector3(0f, 0f, 0f);
        //Vector3 itemCollectorTransformPos = new Vector3(0f,0f,0f);
        //Vector3 powerHitTransformPos = 		new Vector3(0f,0f,0f);
        Vector3 groundStopperTransformPos = new Vector3(0f, -0.25f, 0f);
        Vector3 kingTransformPos = new Vector3(0f, 0.6f, 0f);

        Vector2 headBoxSize = new Vector2(0.7f, 0.25f);
        //Vector2 feetBoxSize = new Vector2(0.7f,0.25f);
        Vector2 bodyBoxSize = new Vector2(0.7f, 0.8f);
        //Vector2 itemCollectorBoxSize = new Vector2(0.7f,0.8f);
        //Vector2 powerHitBoxSize = new Vector2(0.7f,0.8f);
        Vector2 groundStopperBoxSize = new Vector2(0.7f, 0.5f);

        Vector2 colliderOffSetCenter = Vector2.zero;
        Vector2 colliderOffSetLeft = new Vector2(leftPos, 0f);
        Vector2 colliderOffSetRight = new Vector2(rightPos, 0f);

        //			Vector2 headBoxOffset; // use smartOffset
        //			Vector2[] headBoxOffset = new Vector2[3];
        //			headBoxOffset [0] = colliderOffSetCenter;
        //			headBoxOffset [1] = colliderOffSetLeft;
        //			headBoxOffset [2] = colliderOffSetRight;
        Vector2[] smartComponentOffset = new Vector2[3];
        smartComponentOffset[0] = colliderOffSetCenter;
        smartComponentOffset[1] = colliderOffSetLeft;
        smartComponentOffset[2] = colliderOffSetRight;

        bool headIsTrigger = true;
        //bool feetIsTrigger = true;
        bool bodyIsTrigger = false;
        //bool itemCollectorIsTrigger = true;
        //bool powerHitAreaIsTrigger = true;
        bool groundStopperIsTrigger = false;


        // root
        root = new ChildData(characterSO.charName, TagManager.Instance.tag_player, LayerManager.Instance.playerLayerName, centerTransformPos);     //TODO Achtung PrefabName und Name können isch unterscheieden!!!
        root.Add(root.gameObject.AddComponent<SpriteRenderer>(), true, characterSO.GetSprites(teamId, SmwCharacterAnimation.Idle)[0], charGenerics.color_rootRenderer, charGenerics.rootRendererSortingLayer);
        root.Add(root.gameObject.AddComponent<Animator>(), true, characterSO.GetRuntimeAnimationController(teamId));        //TODO inspector
        root.Add(root.gameObject.AddComponent<Rigidbody2D>(), 1f, true);    //TODO inspector
        root.Add(root.gameObject.AddComponent<AudioSource>(), true);
        //root.Add(root.gameObject.AddComponent<RealOwner>(), true);
        root.Add(root.gameObject.AddComponent<PlatformUserControl>(), true);
        PlatformCharacterScript platformCharScript = root.gameObject.AddComponent<PlatformCharacterScript>();
        // build > 0.701
        // add CharSO to CharPrefab
        platformCharScript.SetSmwCharacterSO(characterSO);
        root.Add(platformCharScript, true);
        root.Add(root.gameObject.AddComponent<PlatformJumperScript>(), true);
        //root.Add(root.gameObject.AddComponent<Rage>(), true);
        //root.Add(root.gameObject.AddComponent<Shoot>(), true);
        //root.Add(root.gameObject.AddComponent<Shield>(), true);
        NetworkedPlayer netPlayerScript = root.gameObject.AddComponent<NetworkedPlayer>();
        root.Add(netPlayerScript, true);
//        root.Add(root.gameObject.AddComponent<NetworkView>(), true, netPlayerScript);
        //root.Add(root.gameObject.AddComponent<PushSkript>(), false);
        //root.Add(root.gameObject.AddComponent<Bot>(), false);


        // Clone Left
		ChildData child = new ChildData(TagManager.Instance.name_cloneLeft, TagManager.Instance.tag_player_clone, LayerManager.Instance.playerLayerName, leftTransformPos);
        child.Add(child.gameObject.AddComponent<SpriteRenderer>(), true, characterSO.GetSprites(teamId, SmwCharacterAnimation.Idle)[0], charGenerics.color_rootCloneRenderer, charGenerics.rootCloneRendererSortingLayer);
        child.Add(child.gameObject.AddComponent<CloneSpriteScript>(), true);
        childs.Add(child);

        // Clone Right
		child = new ChildData(TagManager.Instance.name_cloneRight, TagManager.Instance.tag_player_clone, LayerManager.Instance.playerLayerName, rightTransformPos);
        child.Add(child.gameObject.AddComponent<SpriteRenderer>(), true, characterSO.GetSprites(teamId, SmwCharacterAnimation.Idle)[0], charGenerics.color_rootCloneRenderer, charGenerics.rootCloneRendererSortingLayer);
        child.Add(child.gameObject.AddComponent<CloneSpriteScript>(), true);
        childs.Add(child);

        // Clone Top
		child = new ChildData(TagManager.Instance.name_CloneTop, TagManager.Instance.tag_player_clone, LayerManager.Instance.playerLayerName, topTransformPos);
        child.Add(child.gameObject.AddComponent<SpriteRenderer>(), true, characterSO.GetSprites(teamId, SmwCharacterAnimation.Idle)[0], charGenerics.color_rootCloneRenderer, charGenerics.rootCloneRendererSortingLayer);
        child.Add(child.gameObject.AddComponent<CloneSpriteScript>(), true);
        childs.Add(child);

        // Clone Bottom
		child = new ChildData(TagManager.Instance.name_CloneBottom, TagManager.Instance.tag_player_clone, LayerManager.Instance.playerLayerName, bottomTransformPos);
        child.Add(child.gameObject.AddComponent<SpriteRenderer>(), true, characterSO.GetSprites(teamId, SmwCharacterAnimation.Idle)[0], charGenerics.color_rootCloneRenderer, charGenerics.rootCloneRendererSortingLayer);
        child.Add(child.gameObject.AddComponent<CloneSpriteScript>(), true);
        childs.Add(child);

        // Head (cloned)
        child = new ChildData(TagManager.Instance.name_head, TagManager.Instance.tag_head, LayerManager.Instance.headLayerName, headTransformPos);
        child.Add(child.gameObject.AddComponent<BoxCollider2D>(), true, headBoxSize, smartComponentOffset, headIsTrigger, 3);
        childs.Add(child);

        //// Feet (cloned)
		child = new ChildData (TagManager.Instance.name_feet, TagManager.Instance.tag_player, LayerManager.Instance.feetLayerName, feetTransformPos);
        //child.Add(child.gameObject.AddComponent<BoxCollider2D>(), true, feetBoxSize, smartComponentOffset, feetIsTrigger, 3);
        //child.Add(child.gameObject.AddComponent<SendDamageTrigger>(),true);
        childs.Add (child);

        // Body (cloned)
        child = new ChildData(TagManager.Instance.name_body, TagManager.Instance.tag_body, LayerManager.Instance.bodyLayerName, bodyTransformPos);
        child.Add(child.gameObject.AddComponent<BoxCollider2D>(), true, bodyBoxSize, smartComponentOffset, bodyIsTrigger, 3);
        childs.Add(child);

        //// ItemCollector (cloned)
        //child = new ChildData (Tags.Instance.name_itemCollector, Tags.Instance.tag_itemCollector, Layer.Instance.itemLayerName, itemCollectorTransformPos);
        //child.Add(child.gameObject.AddComponent<BoxCollider2D>(), true, itemCollectorBoxSize, smartComponentOffset, itemCollectorIsTrigger, 3);
        //child.Add(child.gameObject.AddComponent<ItemCollectorScript>(),true);
        //childs.Add (child);

        //// PowerHitArea (cloned)
        //child = new ChildData (Tags.Instance.name_powerUpHitArea, Tags.Instance.tag_powerUpHitArea, Layer.Instance.powerUpLayerName, powerHitTransformPos);
        //child.Add(child.gameObject.AddComponent<BoxCollider2D>(), true, powerHitBoxSize, smartComponentOffset, powerHitAreaIsTrigger, 3);
        //child.Add(child.gameObject.AddComponent<RageTrigger>(),true);
        //childs.Add (child);

        // GroundStopper
        child = new ChildData(TagManager.Instance.name_groundStopper, TagManager.Instance.tag_groundStopper, LayerManager.Instance.groundStopperLayerName, groundStopperTransformPos);
        child.Add(child.gameObject.AddComponent<BoxCollider2D>(), true, groundStopperBoxSize, smartComponentOffset, groundStopperIsTrigger, 1);
        childs.Add(child);

        // King
        child = new ChildData(TagManager.Instance.name_king, TagManager.Instance.tag_body, LayerManager.Instance.defaultLayerName, kingTransformPos);
        child.Add(child.gameObject.AddComponent<SpriteRenderer>(), false, charGenerics.kingSprite, charGenerics.color_kingRenderer, charGenerics.kingRendererSortingLayer);
        childs.Add(child);

        // CurrentEstimatedPosOnServer
        child = new ChildData(TagManager.Instance.name_CurrentEstimatedPosOnServer, TagManager.Instance.tag_CurrentEstimatedPosOnServer, LayerManager.Instance.defaultLayerName, centerTransformPos);
        child.Add(child.gameObject.AddComponent<SpriteRenderer>(), true, characterSO.GetSprites(teamId, SmwCharacterAnimation.Idle)[0], charGenerics.color_currentEstimatedPosOnServer, charGenerics.currentEstimatedPosOnServerSortingLayer);
        childs.Add(child);

        // LastRecvedPos
        child = new ChildData(TagManager.Instance.name_lastReceivedPos, TagManager.Instance.tag_lastReceivedPos, LayerManager.Instance.defaultLayerName, centerTransformPos);
        child.Add(child.gameObject.AddComponent<SpriteRenderer>(), true, characterSO.GetSprites(teamId, SmwCharacterAnimation.Idle)[0], charGenerics.color_LastRecvedPos, charGenerics.lastRecvdPosRendererSortingLayer);
        childs.Add(child);

        // PredictedPosSimulatedWithLastInput
        child = new ChildData(TagManager.Instance.name_PredictedPosSimulatedWithLastInput, TagManager.Instance.tag_PredictedPosSimulatedWithLastInput, LayerManager.Instance.defaultLayerName, centerTransformPos);
        child.Add(child.gameObject.AddComponent<SpriteRenderer>(), true, characterSO.GetSprites(teamId, SmwCharacterAnimation.Idle)[0], charGenerics.color_PredictedPosSimulatedWithLastInput, charGenerics.preSimPosRendererSortingLayer);
        childs.Add(child);

        // PredictedPosCalculatedWithLastInput
        child = new ChildData(TagManager.Instance.name_PredictedPosCalculatedWithLastInput, TagManager.Instance.tag_PredictedPosCalculatedWithLastInput, LayerManager.Instance.defaultLayerName, centerTransformPos);
        child.Add(child.gameObject.AddComponent<SpriteRenderer>(), true, characterSO.GetSprites(teamId, SmwCharacterAnimation.Idle)[0], charGenerics.color_PredictedPosCalculatedWithLastInput, charGenerics.preCalclastRecvdPosRendererSortingLayer);
        childs.Add(child);

        // IceWalled
        child = new ChildData(TagManager.Instance.name_iceWalled, TagManager.Instance.tag_iceWalled, LayerManager.Instance.defaultLayerName, centerTransformPos);
        child.Add(child.gameObject.AddComponent<SpriteRenderer>(), true, null, charGenerics.color_iceWallRenderer, charGenerics.iceWalledRendererSortingLayer);
        child.Add(child.gameObject.AddComponent<Animator>(), true, charGenerics.iceWandAnimatorController);
        childs.Add(child);
    }


    GUIStyle myFoldoutStyle;
    private Vector2 sliderPosition;
    private bool showCharacterEditor = true;
    private bool showSpritesheetEditor = true;
    private bool showAnimatorEditor = true;
    private bool showPrefabEditor = true;
    private SmwCharacter window_SmwCharacter;
    private Sprite spritesheet;

    public int subSpritesCount = 6;
    public int pixelPerUnit = 32;
    public int pixelSizeWidth = 32;
    public int pixelSizeHeight = 32;

    private SpriteAlignment spriteAlignment = SpriteAlignment.Center;
    private Vector2 customOffset;
    private Teams selectedTeam;
    private bool networked;
    private bool window_KeepCreatedPrefabsInScene;
    private SmwCharacterGenerics window_SmwCharacterGenerics;
	ChildData root;
    private List<ChildData> childs;

    void SpriteAssetInfo(TextureImporter importer)
    {
        // Spriteinfo ausgeben
        Debug.Log("Path = " + importer.assetPath);
        Debug.Log("Import Mode = " + importer.spriteImportMode.ToString());
        Debug.Log("Pixel Per Unit = " + importer.spritePixelsPerUnit.ToString());
        Debug.Log("Alignment = " + ((SpriteAlignment) importer.spritesheet[0].alignment).ToString());
        Debug.Log("spritePivot = " + importer.spritePivot);
    }

    private void PerformMetaSlice(TextureImporter spriteImporter)
    {
        if (spriteImporter != null)
        {
            Debug.Log("PerformMetaSlice: " + spriteImporter.assetPath);
            //			TextureImporter myImporter = null;
            //			myImporter = TextureImporter.GetAtPath (AssetDatabase.GetAssetPath(sprite)) as TextureImporter ;

            bool failed = false;
            List<SpriteMetaData> metaDataList = new List<SpriteMetaData>();

            //TODO abfragen ob sprite <multiple> ist
            //TODO abfragen ob größe stimmt, überspringen ???

            //			Debug.Log("SpriteMode geladen: " + spriteImporter.spriteImportMode.ToString());
            //			Debug.Log("SpriteMetaData länge geladen: " + spriteImporter.spritesheet.Length);
            //			if(spriteImporter.spriteImportMode == SpriteImportMode.Multiple)
            //			{
            //				spriteImporter.spriteImportMode = SpriteImportMode.Single;
            //				UnityEditor.EditorUtility.SetDirty(myImporter);
            //			}
            //			Debug.Log("SpriteMode (umgestellt): " + spriteImporter.spriteImportMode.ToString());
            //			Debug.Log("SpriteMetaData länge (umgestellt): " + spriteImporter.spritesheet.Length);

            // falls multiple 


            //			slicedSprite = new Sprite[subSpritesCount];
            // Calculate SpriteMetaData (sliced SpriteSheet)
            for (int i = 0; i < subSpritesCount; i++)
            {
                try
                {

                    SpriteMetaData spriteMetaData = new SpriteMetaData
                    {
                        alignment = (int)spriteAlignment,
                        border = new Vector4(),
                        name = System.IO.Path.GetFileNameWithoutExtension(spriteImporter.assetPath) + "_" + i,
                        pivot = PositionTools.GetPivotValue(spriteAlignment, customOffset),
                        rect = new Rect(i * pixelSizeWidth, 0, pixelSizeWidth, pixelSizeHeight)
                    };

                    // erhalte sliced Texture
                    //					slicedSprite[i] = Sprite.Create(unslicedSprite.texture, spriteMetaData.rect, spriteMetaData.pivot, pixelPerUnit);

                    metaDataList.Add(spriteMetaData);

                }
                catch (Exception exception)
                {
                    failed = true;
                    Debug.LogException(exception);
                }
            }

            if (!failed)
            {
                spriteImporter.spritePixelsPerUnit = pixelPerUnit;                  // setze PixelPerUnit
                spriteImporter.spriteImportMode = SpriteImportMode.Multiple;        // setze MultipleMode
                spriteImporter.spritesheet = metaDataList.ToArray();                // weiße metaDaten zu

                EditorUtility.SetDirty(spriteImporter);

                try
                {
                    AssetDatabase.StartAssetEditing();
                    AssetDatabase.ImportAsset(spriteImporter.assetPath);
                }
                catch (Exception e)
                {
                    Debug.LogError("wtf " + e.ToString());
                }
                finally
                {
                    AssetDatabase.StopAssetEditing();
                    //					myImporter.SaveAndReimport();
                    //Close();
                }
            }
            else
            {
                Debug.LogError(spriteImporter.assetPath + " failed");
                SpriteAssetInfo(spriteImporter);
            }
        }
        else
        {
            Debug.LogError(" sprite == null");
        }
    }

    void InitFoldStyle()
    {
        myFoldoutStyle = new GUIStyle(EditorStyles.foldout);
        myFoldoutStyle.fontStyle = FontStyle.Bold;
        myFoldoutStyle.fontSize = 14;
        Color myStyleColor = Color.red;
        myFoldoutStyle.normal.textColor = myStyleColor;
        myFoldoutStyle.onNormal.textColor = myStyleColor;
        myFoldoutStyle.hover.textColor = myStyleColor;
        myFoldoutStyle.onHover.textColor = myStyleColor;
        myFoldoutStyle.focused.textColor = myStyleColor;
        myFoldoutStyle.onFocused.textColor = myStyleColor;
        myFoldoutStyle.active.textColor = myStyleColor;
        myFoldoutStyle.onActive.textColor = myStyleColor;
    }
}
