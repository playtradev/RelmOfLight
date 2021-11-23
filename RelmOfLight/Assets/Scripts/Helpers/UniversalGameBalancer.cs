using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalGameBalancer : MonoBehaviour
{
    public static UniversalGameBalancer Instance;

    [Header("Defending")]
    [Tooltip("The set cost incurred in the character's defence bar, each time they start defending")] public float defenceCost = 20f;
    [Tooltip("The set cost incurred in the character's defence bar, each time they block an incoming attack imprefectly")] public float partialDefenceCost = 10f;
    [Tooltip("The set cost incurred in the character's defence bar, each time they block an incoming attack perfectly")] public float fullDefenceCost = 10f;
    [Tooltip("The set amount of stamina regained by the character upon timing a defence perfectly")] public float staminaRegenOnPerfectBlock = 10f;
    [Tooltip("The set amount of stamina regained by the character upon Damage received in perc on the damage")][Range(1,100)]public float EtherRegenOnDamage = 0f;

    [Header("Ether Depletion"), Space(10)]
    public float BaseSpeedSlowDownOnLowEther_Multiplier = 0.5f;
 
    private void Awake()
    {
        Instance = this;
    }
}


[System.Serializable]
public class DifficultyScalingProfile
{
    [HideInInspector] public string Name;
    [Tooltip("The amount of enemies in each wave should increase as the players will be able to kill them sooner, NOTE: This will also increase the number of enemies allowed on the board at any moment")]
    [Range(1f, 5f)] public float enemySpawnScaler = 1f;

    [Tooltip("Enemy attacks should have more damage as the amount of health pool on the allied side of the board is now much larger")]
    [Range(1f, 3f)] public float enemyDamageScaler = 1f;

    [Tooltip("Enemy attacks should have less of a cooldown as the amount of health pool on the allied side of the board is now much larger")]
    [Range(0.01f, 1f)] public float enemyAttackCooldownScaler = 1f;

    [Tooltip("Enemy move speed should be lessened with more players, to allow them to dodge/move into place quicker, avoiding the barrage of bullets coming from 2 or more attackers")]
    [Range(0.01f, 1f)] public float enemyMoveDurationScaler = 1f;

    [Tooltip("The Health of Bosses and Enemies should increase depending on how many players there are, so enemies aren't instantly wiped off of the board by 2 or more attackers")]
    [Range(1f,5f)] public float enemyHealthScaler = 1f;

    [Tooltip("Higher player counts should be able to complete waves in less time IF the enemy spawn scaler is less than number of players (ie. 2x players, with 1.3x enemies spawning means the wave will die sooner)")]
    [Range(0f,2f)] public float waveDurationScaler = 1f;
}