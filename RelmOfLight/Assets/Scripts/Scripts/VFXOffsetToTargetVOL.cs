using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXOffsetToTargetVOL : MonoBehaviour
{
    public Transform Target;
    public Transform CurrTarget
    {
        get
        {
            return cb != null ? cb.spineT : Target;
        }
    }
    public BaseCharacter cb = null;
    public Vector3 Adjustment;
    public bool IncludeChildren = true;

    GameObject DeathTarget;
    ParticleSystem PS;
    ParticleSystem[] PSChildren;
    bool IsPSAttached = false;

    private void Awake()
    {
        if(GetComponent<ParticleSystem>())
        {
            PS = GetComponent<ParticleSystem>();
            IsPSAttached = true;
        }
        if (!Target)
        {
            Target = transform;
        }
        if (IncludeChildren)
        {
            PSChildren = GetComponentsInChildren<ParticleSystem>();
        }
    }

    private void OnDisable()
    {
        cb = null;
    }


    bool test = false;
    // Update is called once per frame
    void FixedUpdate()
    {
        if(cb != null && (cb.died || !cb.IsOnField))
        {
            ParticleSystem.Particle[] m_Particles = new ParticleSystem.Particle[PS.particleCount];
            PS.GetParticles(m_Particles, PS.particleCount, 0);
            for (int i = 0; i < m_Particles.Length; i++)
            {
                m_Particles[i].remainingLifetime = 0f;
            }
            PS.SetParticles(m_Particles);
        }

        if((cb != null && cb.IsOnField && !cb.isTeleporting && (cb.CharInfo.Behaviour.MovementActionN == MovementActionType.Teleport && cb.isMoving ? false : true)) || (cb == null && Target != null))
        {
            Execute();
        }


    }

    public void KillTrail()
    {
        StartCoroutine(KillTrailCo());
    }


    public IEnumerator KillTrailCo()
    {
        Vector3 offset;
        if(DeathTarget == null)
        {

            DeathTarget = Instantiate(new GameObject(), CurrTarget.position, Quaternion.identity, transform);
        }
        cb = null;
        Target = DeathTarget.transform;
        offset = DeathTarget.transform.position;
        float timer = 0;
        while (timer < 1)
        {
            timer += BattleManagerScript.Instance.FixedDeltaTime;
            yield return BattleManagerScript.Instance.WaitFixedUpdate(()=> BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause);
            Target.position = Vector3.Lerp(offset, transform.position, timer);
        }
        yield return BattleManagerScript.Instance.WaitFor(0.5f,() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause);
        gameObject.SetActive(false);
    }

    public void Execute()
    {
        var VOL = PS.velocityOverLifetime;
        VOL.orbitalOffsetXMultiplier = CurrTarget.position.x - transform.position.x - Adjustment.x;
        VOL.orbitalOffsetYMultiplier = CurrTarget.position.y - transform.position.y - Adjustment.y;
        if (IncludeChildren)
        {
            foreach (ParticleSystem pS in PSChildren)
            {
                var VOLChild = pS.velocityOverLifetime;
                VOLChild.orbitalOffsetXMultiplier = CurrTarget.position.x - transform.position.x - Adjustment.x;
                VOLChild.orbitalOffsetYMultiplier = CurrTarget.position.y - transform.position.y - Adjustment.y;
            }
        }
        ParticleSystem.Particle[] m_Particles = new ParticleSystem.Particle[PS.particleCount];
        PS.GetParticles(m_Particles, PS.particleCount, 0);
        test = false;
        for (int i = 0; i < m_Particles.Length; i++)
        {
            if (Vector3.Distance(m_Particles[i].position, CurrTarget.position) < 0.1f && m_Particles[i].remainingLifetime > 0.2f)
            {
                test = true;
                m_Particles[i].startLifetime = m_Particles[i].startLifetime - m_Particles[i].remainingLifetime + 0.2f;
                m_Particles[i].remainingLifetime = 0.2f;
            }
        }
        if (test)
        {
            PS.SetParticles(m_Particles);
        }
    }
}
