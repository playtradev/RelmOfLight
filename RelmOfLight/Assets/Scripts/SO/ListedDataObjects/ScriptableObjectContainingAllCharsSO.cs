using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AllCharsScriptableObjects Object", menuName = "ScriptableObjects/Character Management/AllCharsScriptableObjects")]
public class ScriptableObjectContainingAllCharsSO : ScriptableObject
{
    public ScriptableObjectCharacterPrefab[] CharacterSo = new ScriptableObjectCharacterPrefab[0];
}
