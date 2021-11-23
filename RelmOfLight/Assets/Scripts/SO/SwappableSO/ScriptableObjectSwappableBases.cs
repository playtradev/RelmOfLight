using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "data", menuName = "ScriptableObjects/SwappableBasesContainer")]
public class ScriptableObjectSwappableBases : ScriptableObject
{
    public List<ScriptableObjectSwappableBase> Bases;

    public List<RuntimeBasesClass> _RuntimeBases = new List<RuntimeBasesClass>();

    public List<RuntimeBasesClass> RuntimeBases
    {
        get
        {
            return _RuntimeBases;
        }
        set
        {
            _RuntimeBases = value;
        }
    }

}

public class RuntimeBasesClass
{
    public string BaseName;
    public ScriptableObjectSwappableBase Swappable;

    public RuntimeBasesClass()
    {
    }

    public RuntimeBasesClass(string baseName, ScriptableObjectSwappableBase swappable)
    {
        BaseName = baseName;
        Swappable = swappable;
    }
}
