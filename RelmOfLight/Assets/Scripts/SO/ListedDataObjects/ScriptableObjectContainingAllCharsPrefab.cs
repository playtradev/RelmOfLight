using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AllCharsPrefab")]
public class ScriptableObjectContainingAllCharsPrefab : ScriptableObject
{
    public List<GameObject> Stage00 = new List<GameObject>();
    public List<GameObject> Stage01 = new List<GameObject>();
    public List<GameObject> Stage02 = new List<GameObject>();
    public List<GameObject> Stage03 = new List<GameObject>();
    public List<GameObject> Stage04 = new List<GameObject>();
    public List<GameObject> Stage05 = new List<GameObject>();
    public List<GameObject> Stage06 = new List<GameObject>();
    public List<GameObject> Stage07 = new List<GameObject>();
    public List<GameObject> Stage08 = new List<GameObject>();
    public List<GameObject> Stage09 = new List<GameObject>();


    public List<CharacterInfoScript> Prefabs
    {
        get
        {
            List<CharacterInfoScript> tlist = new List<CharacterInfoScript>();
            Stage00.ForEach(r=> { tlist.Add(r.GetComponentInChildren<CharacterInfoScript>()); });
            Stage01.ForEach(r=> { tlist.Add(r.GetComponentInChildren<CharacterInfoScript>()); });
            Stage02.ForEach(r=> { tlist.Add(r.GetComponentInChildren<CharacterInfoScript>()); });
            Stage03.ForEach(r=> { tlist.Add(r.GetComponentInChildren<CharacterInfoScript>()); });
            Stage04.ForEach(r=> { tlist.Add(r.GetComponentInChildren<CharacterInfoScript>()); });
            Stage05.ForEach(r=> { tlist.Add(r.GetComponentInChildren<CharacterInfoScript>()); });
            Stage06.ForEach(r=> { tlist.Add(r.GetComponentInChildren<CharacterInfoScript>()); });
            Stage07.ForEach(r=> { tlist.Add(r.GetComponentInChildren<CharacterInfoScript>()); });
            Stage08.ForEach(r=> { tlist.Add(r.GetComponentInChildren<CharacterInfoScript>()); });
            Stage09.ForEach(r=> { tlist.Add(r.GetComponentInChildren<CharacterInfoScript>()); });
            return tlist;
        }
    }

}