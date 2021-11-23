using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Particles/AttackParticle")]
public class ScriptableObjectAttackParticle : ScriptableObject
{
   // public AttackParticleType PSType;
    public GameObject CastRightPS;
    public GameObject BulletRightPS;
    public GameObject ImpactRightPS;
    public GameObject CastLeftPS;
    public GameObject BulletLeftPS;
    public GameObject ImpactLeftPS;
    public GameObject Skill1CastRightPS;
    public GameObject Skill1BulletRightPS;
    public GameObject Skill1ImpactRightPS;
    public GameObject Skill1CastLeftPS;
    public GameObject Skill1BulletLeftPS;
    public GameObject Skill1ImpactLeftPS;
    public GameObject Skill2CastRightPS;
    public GameObject Skill2BulletRightPS;
    public GameObject Skill2ImpactRightPS;
    public GameObject Skill2CastLeftPS;
    public GameObject Skill2BulletLeftPS;
    public GameObject Skill2ImpactLeftPS;

    public GameObject CastLoopPS;
    public GameObject CastActivationPS;
    //public GameObject 
}



[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SkillParticles")]
public class ScriptableObjectSkillParticles : ScriptableObject
{
    public CharacterClassType CharacterClass;
    public List<ScriptableObjectSkillParticlesClass> CastPS = new List<ScriptableObjectSkillParticlesClass>();
    public List<ScriptableObjectSkillParticlesClass> AttackPS = new List<ScriptableObjectSkillParticlesClass>();
    public List<ScriptableObjectSkillParticlesClass> EffectPS = new List<ScriptableObjectSkillParticlesClass>();
}


[System.Serializable]
public class ScriptableObjectSkillParticlesClass
{
    public string Name;
    public ElementalType ElementalT;
    public GameObject Particle;
}