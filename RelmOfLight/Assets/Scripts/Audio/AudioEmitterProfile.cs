using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New_Audio_Emitter_Premade", menuName = "ScriptableObjects/Audio/Audio Emitter Premade")]
public class AudioEmitterProfile : ScriptableObject
{
    [Tooltip("The percentage volume to which all of the lesser priority sounds should be dampened")] public float dampen = 1f;
    [Tooltip("The priority level for this particular sound")] public int priority = 0;
}


