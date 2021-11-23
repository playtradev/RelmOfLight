using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New AllParticles Object", menuName = "ScriptableObjects/AllParticlesObject")]
public class ScriptableObjectContainingAllParticles : ScriptableObject
{
    public ScriptableObjectParticle[] ListOfParticles = new ScriptableObjectParticle[0];
}
