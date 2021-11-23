using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Particles/Particle")]
public class ScriptableObjectParticle : ScriptableObject
{
    public ParticlesType PSType;
    public GameObject PS;
}