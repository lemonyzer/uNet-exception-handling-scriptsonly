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
public class SmwCharacter : ScriptableObject {

	public void Awake ()
	{
        if (characters != null)
            return;

        characters = new TeamCharacter[4];

        for (int i=0; i<(int)Teams.count; i++)
        {
            characters[i] = new TeamCharacter();
            characters[i].name = ((Teams)i).ToString();
        }

        Save();
	}


	void Check()
	{

	}

	public string charName;
	public int charId;
	public bool charInUse;
	public NetworkPlayer netPlayer;
	public Player player;

    public TeamCharacter[] characters;

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
