using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultBoss Attack Particles", menuName = "ScriptableObjects/Particles/Bosses/AttackParticle_Stage09_Boss")]
public class ScriptableObjectAttackParticle_Stage09Boss : ScriptableObjectAttackParticle
{
    [Header("Midnight Dance")]
    public GameObject CastMidnightDance;
    public GameObject ImpactMidnightDance;
    [Header("Moonlight Bless")]
    public GameObject CastMoonlightBless;
    public GameObject ImpactMoonlightBless;
    [Header("Matriarchal Lullaby")]
    public GameObject CastMatriarchalLullaby;
    public GameObject ImpactMatriarchalLullaby;
    [Header("Deamons Army")]
    public GameObject CastDeamonsArmy;
    public GameObject ImpactDeamonsArmy;
    [Header("Deadly Tongue")]
    public GameObject CastDeadlyTongue;
    public GameObject ImpactDeadlyTongue;
    [Header("Doku Storm")]
    public GameObject CastDokuStorm;
    public GameObject ImpactDokuStorm;
}
