#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

public class CreateSmwCharacter {

	[MenuItem("SMW/Assets/Create/SMW Character SO")]
	public static void CreateAsset()
	{
		SmwCharacter asset = ScriptableObject.CreateInstance<SmwCharacter>();

        string uniquePath = AssetDatabase.GenerateUniqueAssetPath("Assets/newSmwCharacterSO.asset");
        AssetDatabase.CreateAsset(asset, uniquePath);
		AssetDatabase.SaveAssets();

		EditorUtility.FocusProjectWindow();
		Selection.activeObject = asset;
	}

	public static SmwCharacter CreateAssetAndSetup()
	{
		SmwCharacter asset = ScriptableObject.CreateInstance<SmwCharacter>();

        string uniquePath = AssetDatabase.GenerateUniqueAssetPath("Assets/newSmwCharacterSO.asset");
        AssetDatabase.CreateAsset(asset, uniquePath);
		AssetDatabase.SaveAssets();
		
//		EditorUtility.FocusProjectWindow();
//		Selection.activeObject = asset;

		return asset;
	}

	public static SmwCharacter CreateAssetWithPathAndName(string relPath, string name)
	{
		SmwCharacter asset = ScriptableObject.CreateInstance<SmwCharacter>();
		asset.charName = name;
        string uniquePath = AssetDatabase.GenerateUniqueAssetPath(relPath + "/" + name + ".asset");
		AssetDatabase.CreateAsset(asset, uniquePath);
		AssetDatabase.SaveAssets();
		
//		EditorUtility.FocusProjectWindow();
//		Selection.activeObject = asset;
		
		return asset;
	}
}
#endif