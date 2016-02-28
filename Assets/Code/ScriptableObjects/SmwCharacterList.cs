using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/*

    ScriptableObjects

    1] Create
        ScritpableObject.CreateInstance

    2] Initialisierung
        init in classmember definition (überschreibt andere initialisierungswerte??!!)
       void Awake () or
       void OnEnable ()

    DONT'S:
        create through Constructor 

    */

public class SmwCharacterList : ScriptableObject {

    [SerializeField]
    public List<Character> charactersList;
	[SerializeField]
	private List<SmwCharacter> characterSOList;

    public List<SmwCharacter> CharacterSOList { get { return characterSOList; } }

    public int Count 
	{ 
		get { return characterSOList.Count; }
	}

	private void Check()
	{
        if (charactersList == null)
        {
            //Debug.LogWarning("<color=red>" + this.ToString() + " charactersList war nicht vorhanden</color>", this);
            charactersList = new List<Character>();
            //Debug.Log("<color=green>" + this.ToString() + " charactersList initialisiert</color>", this);
        }
        //else
        //{
        //    Debug.LogWarning("<color=green>" + this.ToString() + " charactersList war vorhanden</color>", this);
        //}

        if (characterSOList == null)
        {
            //Debug.LogWarning("<color=red>" + this.ToString() + " characterSOList war nicht vorhanden</color>", this);
            characterSOList = new List<SmwCharacter>();
            //Debug.Log("<color=green>" + this.ToString() + " characterSOList initialisiert</color>", this);
        }
        //else
        //{
        //    Debug.LogWarning("<color=green>" + this.ToString() + " characterSOList war vorhanden</color>", this);
        //}

        //charactersList.Add(new Character());
        ////characterSOList.Add(ScriptableObject.CreateInstance<SmwCharacter>());   // not possible
        //characterSOList.Add(CreateSmwCharacter.CreateAssetAndSetup());
        //Save();
        //TODO hideFlags = HideFlags.HideAndDontSave;
    }

    public void Awake()
    {
        //Debug.LogWarning(this.ToString() + " Awake ()");        // Awake() wird bei ScriptableObject.Create asugeführt!!!!
        Check();
    }

    // use for initialisation
    public void OnEnable()
	{
		//Debug.LogWarning(this.ToString() + " OnEnable ()");		// OnEnable() wird bei ScriptableObject.Create asugeführt!!!!
	}

	public void Start()
	{
		Debug.LogWarning(this.ToString() + " Start ()");
	}

	public void Save()
	{
#if UNITY_EDITOR
		UnityEditor.EditorUtility.SetDirty(this);
#endif
	}

	public void SetCharacterIDs()
	{
		for(int i=0; i < characterSOList.Count; i++)
		{
			if(characterSOList[i] != null)
			{
				characterSOList[i].SetID(i);
			}
		}
	}

	public void SetAllNotInUse()
	{
		for(int i=0; i < characterSOList.Count; i++)
		{
			if(characterSOList[i] != null)
			{
				characterSOList[i].charInUse = false;
//				characterList[i].netPlayer = null;		// geht nicht NetworkPlayer = valuetype
				characterSOList[i].player = null;
			}
		}
	}

	public SmwCharacter GetFirstUnselected()
	{
		for (int i=0; i < characterSOList.Count; i++)
		{
			if (characterSOList[i] != null)
			{
				if (characterSOList[i].charInUse == false)
				{
					Debug.Log (this.ToString() + " Character " + i + " frei");
					return characterSOList[i];
				}
			}
		}
		Debug.LogError (this.ToString() + " alle Charactere in Benutzung");
		return null;
	}


	public SmwCharacter GetNextUnselected(int currentId)
	{
		SmwCharacter temp;
		
		if(currentId < 0)
			currentId = 0;
		
		for(int i=currentId; i < characterSOList.Count; i++)
		{
			temp = characterSOList[i];
			if(temp == null)
			{
				Debug.LogError("CharactersArray Element "+ i +" has no SmcCharacter, check " + this.ToString());
				break;
			}
			if(!temp.charInUse)
			{
				Debug.Log (this.ToString() + " Character " + i + " frei");
				return temp;
			}
		}
		
		if(currentId > 0 && currentId < characterSOList.Count)
		{
			for(int i=0; i < currentId; i++)
			{
				temp = characterSOList[i];
				if(temp == null)
				{
					Debug.LogError("CharactersArray Element "+ i +" has no SmcCharacter check " + this.ToString());
					break;
				}
				if(!temp.charInUse)
				{
					Debug.Log (this.ToString() + " Character " + i + " frei");
					return temp;
				}
			}
		}
		
		return null;
	}


    public SmwCharacter Get(int i)
    {
        if (i < characterSOList.Count && i >= 0)
        {
            return characterSOList[i];
        }
        else
            return null;
    }

    public SmwCharacter Get(string name)
    {
        for (int i=0;i<characterSOList.Count; i++)
        {
            if (characterSOList[i].charName.Equals(name))
            {
                return characterSOList[i];
            }
        }
        return null;
    }

    public void Add(SmwCharacter charSO)
	{
		if(charSO != null)
			characterSOList.Add(charSO);
	}

	public void RemoveAt (int index)
	{
		characterSOList.RemoveAt (index);
	}

	public void Clear ()
	{
		characterSOList.Clear ();
	}

}
