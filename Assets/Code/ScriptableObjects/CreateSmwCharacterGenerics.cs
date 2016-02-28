#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

public class CreateSmwCharacterGenerics {


	[MenuItem("SMW/Assets/Create/SMW Character Generics SO")]
	public static SmwCharacterGenerics Create()
	{
		SmwCharacterGenerics asset = ScriptableObject.CreateInstance<SmwCharacterGenerics>();

        string uniquePath = AssetDatabase.GenerateUniqueAssetPath("Assets/SMWCharacterGenerics.asset");
        AssetDatabase.CreateAsset(asset, uniquePath);
		AssetDatabase.SaveAssets();
		return asset;
	}
}
#endif