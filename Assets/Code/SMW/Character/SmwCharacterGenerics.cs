using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SmwCharacterGenerics : ScriptableObject {

    public bool AllPropertysSet()
    {
        if (spawnAnimClip == null) return false;
        if (protectionAnimClip == null) return false;
        if (rageAnimClip == null) return false;

        return true;
    }

	// properties for all characters
	public AnimationClip spawnAnimClip;
	public AnimationClip protectionAnimClip;
	public AnimationClip rageAnimClip;

    // können nicht im inspector zugewiesen werden!
    //	public SpawnStateScript spawnStateScript;
    //	public SpawnDelayStateScript spawnDelayStateScript;

    public GameObject frictionSmokeTrailPrefab;

	
	public Sprite kingSprite;
	public RuntimeAnimatorController iceWandAnimatorController;
	
	public Color color_rootRenderer 						= new Color(1f,1f,1f,1f);		// ALL (ROOT SpriteRenderer)
	public Color color_rootCloneRenderer 					= new Color(1f,1f,1f,1f);		// ALL
	public Color color_kingRenderer		 					= new Color(1f,1f,1f,1f);		// ALL
	public Color color_iceWallRenderer	 					= new Color(1f,1f,1f,1f);		// ALL
	public Color color_currentEstimatedPosOnServer 			= new Color(1f,1f,1f,0.1f);	// localplayer Character's	only
	public Color color_LastRecvedPos 						= new Color(1f,1f,1f,0.1f);	// all other Character's	vergangene Position
	public Color color_PredictedPosSimulatedWithLastInput 	= new Color(1f,1f,1f,0.1f);	// all other Character's	vergangene Position
	public Color color_PredictedPosCalculatedWithLastInput 	= new Color(1f,1f,1f,0.1f);	// all other Character's	vergangene Position
	
	public int rootRendererSortingLayer;
	public string rootRendererSortingLayerName = SortingLayerScript.name_CharacterBackground;
	public int rootCloneRendererSortingLayer;
	public string rootCloneRendererSortingLayerName = SortingLayerScript.name_CharacterBackground;
	public int kingRendererSortingLayer;
	public string kingRendererSortingLayerName = SortingLayerScript.name_CharacterKing;
	public int iceWalledRendererSortingLayer;
	public string iceWalledRendererSortingLayerName = SortingLayerScript.name_CharacterForeground;
	public int currentEstimatedPosOnServerSortingLayer;
	public string currentEstimatedPosOnServerSortingLayerName = SortingLayerScript.name_CharacterForeground;
	public int lastRecvdPosRendererSortingLayer;
	public string lastRecvdPosRendererSortingLayerName = SortingLayerScript.name_CharacterForeground;
	public int preSimPosRendererSortingLayer;
	public string preSimPosRendererSortingLayerName = SortingLayerScript.name_CharacterForeground;
	public int preCalclastRecvdPosRendererSortingLayer;
	public string preCalclastRecvdPosRendererSortingLayerName = SortingLayerScript.name_CharacterForeground;

}
