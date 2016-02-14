using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum SmwCharacterAnimation
{
    Spritesheet,
    Idle,
    Run,
    Jump,
    Skid,
    Die,
    HeadJumped
}

[System.Serializable]
public class TeamCharacter
{
    public GameObject charPrefab;
    public GameObject unityNetworkPrefab;

    public Sprite[] charSpritesheet;
    public RuntimeAnimatorController runtimeAnimatorController;

    public Sprite[] charIdleSprites;
    public Sprite[] charRunSprites;
    public Sprite[] charJumpSprites;
    public Sprite[] charSkidSprites;
    public Sprite[] charDieSprites;
    //	public Sprite[] charGameOverSprites;
    public Sprite[] charHeadJumpedSprites;


    public void SetCharSpritesheet(Sprite[] sprites)
    {
        charSpritesheet = sprites;

        if (sprites.Length < 6)
        {
            Debug.LogError("Sprite needs do be prepared (sliced to 6 sprites), no automating slicing");
            return;
        }

        //Idle
        charIdleSprites = new Sprite[1];
        charIdleSprites[0] = charSpritesheet[0];
        //charIdleSprites[0] = Sprite.Create(charSpriteSheet[0].texture, charSpriteSheet[0].rect, charSpriteSheet[0].pivot);

        //Run
        charRunSprites = new Sprite[2];
        charRunSprites[0] = charSpritesheet[0];
        charRunSprites[1] = charSpritesheet[1];

        //Jump
        charJumpSprites = new Sprite[1];
        charJumpSprites[0] = charSpritesheet[2];

        //Skid - ChangeRunDirection
        charSkidSprites = new Sprite[1];
        charSkidSprites[0] = charSpritesheet[3];

        //Die
        charDieSprites = new Sprite[1];
        charDieSprites[0] = charSpritesheet[4];

        //HeadJumped
        charHeadJumpedSprites = new Sprite[1];
        charHeadJumpedSprites[0] = charSpritesheet[5];

        //TODO important
        Save();             // speichere Asset (Änderung wird übernommen)
        //TODO important
    }

    public void Save()
    {
#if UNITY_EDITOR
        //UnityEditor.EditorUtility.SetDirty(this);                   // vielleicht
#endif
    }

    public void SetUnityNetworkPrefab(GameObject prefab)
    {
        this.unityNetworkPrefab = prefab;
    }

    public void SetPrefab(GameObject prefab)
    {
        this.charPrefab = prefab;
    }

}

[System.Serializable]
public class SmwCharacter : ScriptableObject {


	// Awake() wird bei ScriptableObject.Create asugeführt!!!!

//	public SmwCharacter ()
//	{
//		Debug.Log(this.ToString() + " konstruktor ()");			// wird auch at Runtime ausgeführt
//	}
//	public void Awake ()
//	{
//		Debug.Log(this.ToString() + " Awake ()");				// Awake() wird bei ScriptableObject.Create asugeführt!!!!
//	}
//	public void Start ()
//	{
//		Debug.Log(this.ToString() + " Start ()");
//	}
	public void OnEnable()
	{
		Debug.Log("<color=green>" + this.ToString() + " OnEnable () </color>", this);		// OnEnable() wird bei ScriptableObject.Create asugeführt!!!!
		Check();
	}

	void Check()
	{
        //for (int i=0; i<characters.Length; i++)
        //{
        //    if (characters[i] == null)
        //    {
        //        characters[i] = new TeamCharacter();
        //        Debug.Log(this.ToString() + " Character " + (Teams)i + " created");
        //    }
        //}
        
        //// UnityEditor.EditorUtility.SetDirty(this);
	}

	public string charName;
	public int charId;
	public bool charInUse;
	public NetworkPlayer netPlayer;
	public Player player;

    public TeamCharacter[] characters = new TeamCharacter[4];

    public TeamCharacter currentTeamCharacter;

    public GameObject GetUnityNetworkPrefab()
    {
        return currentTeamCharacter.unityNetworkPrefab;
    }

    public GameObject GetUnityNetworkPrefab(Teams teamId)
    {
        return GetCharacter(teamId).unityNetworkPrefab;
    }

    public TeamCharacter GetCharacter(Teams teamId)
    {
        return characters[(int)teamId];
    }

    public void SetUnityNetworkPrefab(Teams teamId, GameObject unetPrefab)
    {
        GetCharacter(teamId).SetUnityNetworkPrefab(unetPrefab);
    }

    public void AddCharacter(int teamId, TeamCharacter teamChar)
    {
        characters[teamId] = teamChar;
    }

    public void SetTeam(int teamId)
    {
        currentTeamCharacter = characters[teamId];
    }

    public RuntimeAnimatorController GetRuntimeAnimationController(Teams teamId)
    {
        return GetCharacter(teamId).runtimeAnimatorController;
    }

    public RuntimeAnimatorController SetRuntimeAnimationController(Teams teamId, RuntimeAnimatorController runtimeAnimatorController)
    {
        return GetCharacter(teamId).runtimeAnimatorController = runtimeAnimatorController;
    }

    public Sprite[] GetSprites(Teams teamId, SmwCharacterAnimation type)
    {
        if (GetCharacter(teamId) == null)
        {
            Debug.LogError(this.name + " " + teamId + " not initialized");
            return null;
        }

        Sprite[] value = null;

        switch (type)
        {
            case SmwCharacterAnimation.Spritesheet:
                value = GetCharacter(teamId).charSpritesheet;
                break;
            case SmwCharacterAnimation.Idle:
                value = GetCharacter(teamId).charIdleSprites;
                break;
            case SmwCharacterAnimation.Run:
                value = GetCharacter(teamId).charRunSprites;
                break;
            case SmwCharacterAnimation.Jump:
                value = GetCharacter(teamId).charJumpSprites;
                break;
            case SmwCharacterAnimation.Skid:
                value = GetCharacter(teamId).charSkidSprites;
                break;
            case SmwCharacterAnimation.Die:
                value = GetCharacter(teamId).charDieSprites;
                break;
            case SmwCharacterAnimation.HeadJumped:
                value = GetCharacter(teamId).charHeadJumpedSprites;
                break;
            default:
                break;
        }
        return value;
    }

    public void SetID(int i)
	{
		charId = i;
		Save ();
	}


    public void Save()
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);                   // vielleicht
#endif
    }

    public void SetCharSpritesheet(Teams teamId, Sprite[] slicedSprites)
    {
        GetCharacter(teamId).SetCharSpritesheet(slicedSprites);
    }



    //	void SetupAnimationStateSprites(Sprite[] stateSprites, uint spriteCount)
    //	{
    //		stateSprites = new Sprite[spriteCount];
    //	}


}
