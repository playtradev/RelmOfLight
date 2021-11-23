using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ScriptableObjectAchievementsDataContainer")]
public class ScriptableObjectAchievementsDataContainer : ScriptableObject
{
    public ScriptableObjectAchievementData[] Achievements;
}

