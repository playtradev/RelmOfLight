using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using MyBox;

public class ParticleHelperScript : MonoBehaviour
{
    private List<ParticleChildSimulationSpeed> Children = new List<ParticleChildSimulationSpeed>();
    [Tooltip("Insert particles that consist in only one long particle")]
    public List<ParticleSystem> LongParticles = new List<ParticleSystem>();
    public float PSTime = 10f;
    [Tooltip("All trails inside the group")]
    List<TrailRenderer> Trails = new List<TrailRenderer>();
    [Tooltip("Cache of trail initial information")]
    List<float> TrailInitialTime = new List<float>();
    public float timet = 0;
    [Tooltip("Specify the Sorting Layer Name")]
    public bool ChangeSortingLayer = true;
    public string SortingLayer = "Player";
    ParticleSystem PS;
    ParticleSystem[] PSChildren;
    public float AddedDuration = 0;
    IEnumerator SimulationSpeedSetter = null;
    public float StartLifeTimeLongParticles = 1;
    public bool HasExplosion = false;
    [ConditionalField("HasExplosion", false)] public ParticlesType PsExplosion;
    [ConditionalField("HasExplosion", false)] public float TimeAdjust = 0;
    IEnumerator Explosion = null;
    float ExplosionTime = 0;


    public List<EmitterCLass> MeshEmitters = new List<EmitterCLass>();

    private void Awake()
    {
        foreach (ParticleSystem item in GetComponentsInChildren<ParticleSystem>(true))
        {
            ParticleSystemRenderer renderer = item.GetComponent<ParticleSystemRenderer>();
            renderer.sortingLayerName = ChangeSortingLayer==true?SortingLayer : renderer.sortingLayerName;
            Children.Add(new ParticleChildSimulationSpeed(item.main.simulationSpeed, item, item.main.duration, item.main.startDelay));
        }

        foreach (TrailRenderer trail in GetComponentsInChildren<TrailRenderer>())
        {
            trail.sortingLayerName = ChangeSortingLayer == true ? SortingLayer : trail.sortingLayerName;
            Trails.Add(trail);
            if (trail.GetComponent<VFXBulletSpeedCalibration>())
            {
                //VFXBulletSpeedCalibration vfx = trail.GetComponent<VFXBulletSpeedCalibration>();
                //TrailInitialTime.Add(vfx.trailTimeCache * (vfx.BulletDuration / vfx.BulletOriginalDuration));
                TrailInitialTime.Add(trail.time);
            }
            else
            {
                TrailInitialTime.Add(trail.time);
            }
        }


        SetStopAction(ParticleSystemStopAction.Disable);
    }


    public void SetStopAction(ParticleSystemStopAction sAction)
    {
        if(PS == null)
        {
            PS = GetComponent<ParticleSystem>();
        }
        var main = PS.main;
        main.stopAction = sAction;
    }

    void OnParticleSystemStopped()
    {
       // Debug.Log("System has stopped!");
        ResetParticle();
    }

    public void SetMeshInMeshEmitters(MeshRenderer mesh, Texture2D texture)
    {
        for (int i = 0; i < MeshEmitters.Count; i++)
        {
            if (MeshEmitters[i].MeshMod)
            {
                var m = MeshEmitters[i].PS.shape;
                m.enabled = true;
                m.shapeType = ParticleSystemShapeType.MeshRenderer;
                m.meshRenderer = mesh;
                if (MeshEmitters[i].UseTexture)
                {
                    m.texture = texture;
                }
            }

            if (MeshEmitters[i].DurationMod)
            {
                var main = MeshEmitters[i].PS.main;
                MeshEmitters[i].PS.Stop();
                main.duration = Children.Where(r => r.Child == MeshEmitters[i].PS).First().DurationBaseValue;
                MeshEmitters[i].PS.Play();
            }
        }
    }

    private void OnEnable()
    {
        for (int i = 0; i < Trails.Count; i++)
        {
           // Trails[i].Clear();
            Trails[i].time = 0;
        }
        Invoke("ResetTrailsTime", 0.1f);
        SetSimulationSpeed(BattleManagerScript.Instance.BattleSpeed);
    }

    private void OnDisable()
    {
        Explosion = null;
        StopAllCoroutines();
        SetSimulationSpeedToBase();
    }

    private void ResetTrailsTime()
    {
        for (int i = 0; i < Trails.Count; i++)
        {
            Trails[i].time = TrailInitialTime[i];
            Trails[i].emitting = true;
        }
    }

    public void ResetParticle()
    {
        foreach (ParticleChildSimulationSpeed item in Children)
        {
            item.Child.Clear();
        }
        transform.parent = null;
        gameObject.SetActive(false);
    }

    public void SetSimulationSpeedToBase()
    {
        foreach (ParticleChildSimulationSpeed item in Children)
        {
            var main = item.Child.main;
            main.simulationSpeed = item.SimulationSpeedBaseValue;
        }
     }

    public void SetDurationToBase()
    {
        foreach (ParticleChildSimulationSpeed item in Children)
        {
            var main = item.Child.main;
            item.Child.Stop();
            main.duration = item.DurationBaseValue;
            item.Child.Play();
        }
    }

    public void SetSimulationSpeed(float speed)
    {
        SetSimulationSpeedToBase();

        foreach (ParticleChildSimulationSpeed item in Children)
        {
            var main = item.Child.main;
            main.simulationSpeed *= speed;
        }
    }

  

    public void SetSimulationSpeedOverTime(SlowDownOnHitClass slowDown)
    {
        if (SimulationSpeedSetter != null) StopCoroutine(SimulationSpeedSetter);
        SimulationSpeedSetter = SimulationSpeedSetterCo(slowDown);
        StartCoroutine(SimulationSpeedSetter);
    }

    IEnumerator SimulationSpeedSetterCo(SlowDownOnHitClass slowDown)
    {
        BattleState currentBs = BattleManagerScript.Instance.CurrentBattleState;
        yield return BattleManagerScript.Instance.WaitFor(slowDown.TimeEffectDelay,
            () => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause,
            () => (BattleManagerScript.Instance.CurrentBattleState != BattleState.Pause &&
            BattleManagerScript.Instance.CurrentBattleState != currentBs));
        SetSimulationSpeed(slowDown.TimeEffect);
        yield return BattleManagerScript.Instance.WaitFor(slowDown.DurationOfTimeEffect,
            () => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause,
            () => (BattleManagerScript.Instance.CurrentBattleState != BattleState.Pause &&
            BattleManagerScript.Instance.CurrentBattleState != currentBs));
        SetSimulationSpeedToBase();
    }

    public void UpdatePSMeshTime(float time)
    {
        for (int i = 0; i < MeshEmitters.Count; i++)
        {
            if (MeshEmitters[i].DurationMod)
            {
                var main = MeshEmitters[i].PS.main;
                MeshEmitters[i].PS.Stop();
                main.duration = time / main.simulationSpeed;
                MeshEmitters[i].PS.Play();
            }
        }
    }
    bool restartTimer = false;
    private IEnumerator Explosion_Co()
    {
        float timer = 0;

        while (timer < (ExplosionTime - TimeAdjust))
        {
            yield return null;
            if(restartTimer)
            {
                restartTimer = false;
                timer = 0;
            }
            timer += BattleManagerScript.Instance.DeltaTime;
        }
        if(PsExplosion != ParticlesType.None)
        {
            ParticleManagerScript.Instance.FireParticlesInPosition(PsExplosion, transform.position);

        }
    }


    void SetExplosion()
    {
        if(isActiveAndEnabled)
        {
            Explosion = Explosion_Co();
            StartCoroutine(Explosion);
        }
    }

    public void UpdatePSTime()
    {
        bool parent = true;
        bool fired = false;
        foreach (ParticleSystem p in LongParticles)
        {
            var m = p.main;
            float s = m.startLifetime.constant;
            s = PSTime;// p.time >= PSTime ? s : (PSTime - p.time) + StartLifeTimeLongParticles;
            m.startLifetime = s;
            ParticleSystem.Particle[] m_Particles = new ParticleSystem.Particle[p.particleCount];
            p.GetParticles(m_Particles, p.particleCount, 0);
            for (int i = 0; i < m_Particles.Length; i++)
            {
                m_Particles[i].startLifetime = p.time >= PSTime || p.time <= 0.2f ? m_Particles[i].startLifetime : (PSTime - p.time) + StartLifeTimeLongParticles;

                m_Particles[i].remainingLifetime = p.time >= PSTime ? PSTime : (PSTime - p.time);

                ExplosionTime = p.time >= PSTime ? PSTime : (PSTime - p.time);
            }
            p.SetParticles(m_Particles);
            if (Explosion == null && !fired)
            {
                Invoke("SetExplosion", 0.1f);
                fired = true;
            }
            else
            {
                restartTimer = true;
            }
        }

        //set the simulation time in every particle, minding the the simulation speed
        foreach (ParticleChildSimulationSpeed item in Children)
        {
            var m = item.Child.main;
            m.loop = false;
            //To change the duration the particle needs to pe paused
            item.Child.Stop();
            float PST = PSTime;
            m.duration =  (PST + (parent ? AddedDuration : 0)) / item.SimulationSpeedBaseValue;
            parent = false;
            //check if the particle is finished
            item.Child.Play();
            if (item.Child.time >= m.duration)
            {
                for (int i = 0; i < Trails.Count; i++)
                {
                    TrailRenderer trail = Trails[i];
                    trail.time = trail.time / 1.1f;
                    if (trail.time < 0.005f)
                    {
                        trail.time = 0;
                        trail.emitting = false;
                    }
                }
            }
        }
    }

    public void AddPSTime(float time)
    {
        PSTime = time;
        bool fired = false;
        foreach (ParticleSystem p in LongParticles)
        {
            var m = p.main;
            float s = m.startLifetime.constant;
            s = p.time >= PSTime ? s : (PSTime - p.time) + StartLifeTimeLongParticles;
            m.startLifetime = s;
            ParticleSystem.Particle[] m_Particles = new ParticleSystem.Particle[p.particleCount];
            p.GetParticles(m_Particles, p.particleCount, 0);
            for (int i = 0; i < m_Particles.Length; i++)
            {
                m_Particles[i].startLifetime = p.time >= PSTime || p.time <= 0.2f ? m_Particles[i].startLifetime : (PSTime - p.time) + StartLifeTimeLongParticles;

                m_Particles[i].remainingLifetime = p.time >= PSTime ? PSTime : (PSTime - p.time);

                ExplosionTime = p.time >= PSTime ? PSTime : (PSTime - p.time);
            }
            p.SetParticles(m_Particles);
            if (Explosion == null && !fired)
            {
                Invoke("SetExplosion", 0.1f);
                fired = true;
            }
            else
            {
                restartTimer = true;
            }
        }

        //set the simulation time in every particle, minding the the simulation speed
        foreach (ParticleChildSimulationSpeed item in Children)
        {
            var m = item.Child.main;
            m.loop = false;
            //To change the duration the particle needs to pe paused
            item.Child.Stop();
            m.duration += PSTime / item.SimulationSpeedBaseValue;
            //check if the particle is finished
            item.Child.Play();
            if (item.Child.time >= m.duration)
            {
                for (int i = 0; i < Trails.Count; i++)
                {
                    TrailRenderer trail = Trails[i];
                    trail.time = trail.time / 1.1f;
                    if (trail.time < 0.005f)
                    {
                        trail.time = 0;
                        trail.emitting = false;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Set the Particle Systems Time with the inputted variable, if put to 0 it will stop immediately
    /// </summary>
    public void UpdatePSTime(float time)
    {
        PSTime = time;
        UpdatePSTime();
    }


    public void StopWithDelay(float delay)
    {
        StartCoroutine(StopWithDelayCo(delay));
    }

    IEnumerator StopWithDelayCo(float delay)
    {
        yield return BattleManagerScript.Instance.WaitFor(delay, () => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause);
        StopPs();
    }

    public void StopPs()
    {
        bool parent = true;
        ExplosionTime = 0;
        foreach (ParticleSystem p in LongParticles)
        {
            var m = p.main;
            ParticleSystem.Particle[] m_Particles = new ParticleSystem.Particle[p.particleCount];
            p.GetParticles(m_Particles, p.particleCount, 0);
            for (int i = 0; i < m_Particles.Length; i++)
            {
                m_Particles[i].remainingLifetime = AddedDuration;
            }
            p.SetParticles(m_Particles);
          
        }

        //set the simulation time in every particle, minding the the simulation speed
        foreach (ParticleChildSimulationSpeed item in Children)
        {
          
            //To change the duration the particle needs to pe paused
            item.Child.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            var m = item.Child.main;
            m.loop = false;
            m.duration = (item.Child.time + 0.1f + (parent ? AddedDuration : 0)) / item.SimulationSpeedBaseValue;
            parent = false;
            //check if the particle is finished
            item.Child.Play();
            if (item.Child.time >= m.duration)
            {
                for (int i = 0; i < Trails.Count; i++)
                {
                    TrailRenderer trail = Trails[i];
                    trail.time = trail.time / 1.1f;
                    if (trail.time < 0.005f)
                    {
                        trail.time = 0;
                        trail.emitting = false;
                    }
                }
            }
        }
    }


    public void UpdatePsSortingOrder(int sortingOrder)
    {
        foreach (ParticleChildSimulationSpeed item in Children)
        {
            item.PSR.sortingOrder = sortingOrder;
        }
    }

}


public class ParticleChildSimulationSpeed
{
    public float SimulationSpeedBaseValue;
    public float DurationBaseValue;
    public ParticleSystem.MinMaxCurve DelayBaseValue;
    public ParticleSystem Child;
    public ParticleSystemRenderer PSR;

    public ParticleChildSimulationSpeed(float baseValue, ParticleSystem child, float durationBaseValue, ParticleSystem.MinMaxCurve delayBaseValue)
    {
        SimulationSpeedBaseValue = baseValue;
        Child = child;
        DurationBaseValue = durationBaseValue;
        DelayBaseValue = delayBaseValue;
        PSR = child.GetComponent<ParticleSystemRenderer>();
    }
}

[System.Serializable]
public class EmitterCLass
{
    public bool MeshMod = true; 
    public ParticleSystem PS;
    public bool UseTexture = true;
    public bool DurationMod = false;
}