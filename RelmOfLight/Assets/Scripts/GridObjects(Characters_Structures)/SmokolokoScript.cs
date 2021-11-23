using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokolokoScript : SummonScript
{
    public bool usePotionParticles = false;

    GameObject activeParticles = null;

    public void Init(ScriptableObjectItemPowerUps itemPowerUpInfo, float duration = 0f)
    {
        Init(duration);

        charTracking.CharInfo.Behaviour.DeathBehaviour = DeathBehaviourType.Defeat_And_Explode;
        charTracking.RefreshSwappableSOs();

        SetParticles(itemPowerUpInfo);
    }

    public void SetParticles(ScriptableObjectItemPowerUps itemPowerUpInfo)
    {
        if (!usePotionParticles) return;

        if (itemPowerUpInfo.activeParticles != null)
        {
            activeParticles = ParticleManagerScript.Instance.FireParticlesInPosition(itemPowerUpInfo.activeParticles, transform.position);
            activeParticles.transform.SetParent(charTracking.spineT);
        }
    }

    public override void TrackedCharDies(BaseCharacter character)
    {
        if(activeParticles != null) Destroy(activeParticles);
    }
}
