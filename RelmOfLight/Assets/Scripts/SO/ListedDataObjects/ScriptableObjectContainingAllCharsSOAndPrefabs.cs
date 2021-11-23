using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Spine.Unity;

[CreateAssetMenu(fileName = "New AllCharsScriptableObjectsAndPrefabs Object", menuName = "ScriptableObjects/Character Management/AllCharsScriptableObjectsAndPrefabs")]
public class ScriptableObjectContainingAllCharsSOAndPrefabs : ScriptableObject
{
    public SkeletonAssetContainer SkeletonC;
    public ScriptableObjectContainingAllCharsSO CharactersData = null;
    [SerializeField] public List<CharacterSOPrefabGroup> SOPrefabGroups;


    /// <summary>
    /// Resize the list
    /// </summary>
    public void Refresh_SOPrefabGroups()
    {
        if (CharactersData == null || CharactersData.CharacterSo == null)
            return;

        if (SOPrefabGroups == null)
            SOPrefabGroups = new List<CharacterSOPrefabGroup>();

        foreach (ScriptableObjectCharacterPrefab scriptableObj in CharactersData.CharacterSo)
        {
            if(SOPrefabGroups.Where(r => r.ScriptableObject == scriptableObj).Count() != 0)
                continue;
            SOPrefabGroups.Add(new CharacterSOPrefabGroup(scriptableObj));
        }

        foreach (CharacterSOPrefabGroup SOPrefabGroup in SOPrefabGroups.ToArray())
        {
            if (SOPrefabGroup.ScriptableObject != null && CharactersData.CharacterSo.ToList().Contains(SOPrefabGroup.ScriptableObject))
                continue;
            SOPrefabGroups.Remove(SOPrefabGroup);
        }
    }

    /// <summary>
    /// Set the abridged stats from the gameobjects in this list
    /// </summary>
    public void Update_ScriptableObjectStats_FromSOPrefabGroup()
    {
        foreach (CharacterSOPrefabGroup SOPrefabGroup in SOPrefabGroups)
        {
            SOPrefabGroup.ScriptableObject.AbridgedCharInfo = new AbridgedCharacterInfo(SOPrefabGroup.Prefab);
        }
    }

    /// <summary>
    /// Set the abridged stats from the gameobjects in the scriptable objects
    /// </summary>
    public void Update_ScriptableObjectStats_FromScriptableObjects()
    {
        foreach (CharacterSOPrefabGroup SOPrefabGroup in SOPrefabGroups)
        {
            if (SOPrefabGroup.ScriptableObject == null)
                continue;
           // SOPrefabGroup.ScriptableObject.AbridgedCharInfo = new AbridgedCharacterInfo(SOPrefabGroup.ScriptableObject.CharacterPrefab);
        }
    }

    /// <summary>
    /// Remove the prefab data from the scriptable objects, but not from the list in this class
    /// </summary>
    public void RemoveAllPrefabsFromWithinScriptableObjects()
    {
        foreach (CharacterSOPrefabGroup SOPrefabGroup in SOPrefabGroups)
        {
            if (SOPrefabGroup.ScriptableObject == null)
                continue;
          //  SOPrefabGroup.ScriptableObject.CharacterPrefab = null;
        }
    }

    /// <summary>
    /// Set the prefabs in every scriptable object to be the ones defined by this list
    /// </summary>
    public void AddPrefabsToScriptableObjectsFromList()
    {
        foreach (CharacterSOPrefabGroup SOPrefabGroup in SOPrefabGroups)
        {
            if (SOPrefabGroup.ScriptableObject == null)
                continue;
           // SOPrefabGroup.ScriptableObject.CharacterPrefab = SOPrefabGroup.Prefab;
        }
    }
}

[System.Serializable]
public class CharacterSOPrefabGroup
{
    public ScriptableObjectCharacterPrefab ScriptableObject;
    public GameObject Prefab;

    public CharacterSOPrefabGroup(ScriptableObjectCharacterPrefab CharacterSO)
    {
        ScriptableObject = CharacterSO;
      //  Prefab = ScriptableObject != null ? ScriptableObject.CharacterPrefab : null;
    }
}