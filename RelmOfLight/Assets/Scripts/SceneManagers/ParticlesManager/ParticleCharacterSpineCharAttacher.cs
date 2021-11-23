using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCharacterSpineCharAttacher : MonoBehaviour
{
    BaseCharacter attachedChar = null;
    ParticleHelperScript psHS = null;


    public void Initialise(BaseCharacter charToTrack)
    {
        attachedChar = charToTrack;
        psHS = GetComponentInChildren<ParticleHelperScript>();
        attachedChar.CurrentCharIsDeadEvent += AttachedCharDied;
        attachedChar.CurrentCharIsSpawnedEvent += AttachedCharSpawned;
    }

    public void AnimationTriggered(string animName)
    {

    }

    public void AttachedCharSpawned(BaseCharacter notImportant)
    {
        psHS.UpdatePSTime(99999999f);
    }

    public void AttachedCharDied(BaseCharacter notImportant)
    {
        psHS.StopPs();
    }
}
