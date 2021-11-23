using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Effect/ItemPowerUps")]
public class ScriptableObjectItemPowerUps : ScriptableObjectAttackEffect
{
    public string powerUpText = "D";
    public PowerUpColorTypes color = PowerUpColorTypes.White;
    public GameObject activeParticles = null;
    public GameObject terminationParticles = null;
    public bool InfiniteOnFieldDuration = false;
    [ConditionalField("InfiniteOnFieldDuration", true)] public float DurationOnField;

}