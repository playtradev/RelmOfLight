using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ScriptableObjectStageProfileContainer")]
public class ScriptableObjectStageProfileContainer : ScriptableObject
{
    public StageProfile_Base[] Profiles;
}

