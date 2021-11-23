using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Class Data", menuName = "ScriptableObjects/Classes/Data")]
public class ScriptableObjectClassData : ScriptableObject
{
    public string Name => Type.ToString();
    public CharacterClassType Type = CharacterClassType.Any;
    public Sprite Icon = null;
    public Color Color = Color.white;
}
