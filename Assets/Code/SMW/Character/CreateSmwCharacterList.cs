#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

public class CreateSmwCharacterList {


	[MenuItem("SMW/Assets/Create/SMW Character List SO")]
	public static SmwCharacterList Create()
	{
		SmwCharacterList asset = ScriptableObject.CreateInstance<SmwCharacterList>();

        string uniquePath = AssetDatabase.GenerateUniqueAssetPath("Assets/SMWCharacterList.asset");
		AssetDatabase.CreateAsset(asset, uniquePath);
		AssetDatabase.SaveAssets();
		return asset;
	}
}
#endif