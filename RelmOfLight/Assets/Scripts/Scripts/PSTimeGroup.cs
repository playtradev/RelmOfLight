using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSTimeGroup : MonoBehaviour
{
    [Tooltip("Set this to override the time of the particles")]
    public float PSTime = 10f;
    [Tooltip("If enabled it will constantly update the duration of every particles")]
    public bool AutoUpdate = true;
    [Tooltip("This will disable once the particles finish")]
    bool DisableTrail = false;
    [Tooltip("All trails inside the group")]
    List<TrailRenderer> Trails = new List<TrailRenderer>();
    [Tooltip("Cache of trail initial information")]
    List<float> TrailInitialTime = new List<float>();
    [Tooltip("Insert particles that consist in only one long particle")]
    public List<ParticleSystem> LongParticles = new List<ParticleSystem>();
    bool initialized = false;

    private void Start()
    {
        //search and register the trails in the group
        foreach (TrailRenderer trail in GetComponentsInChildren<TrailRenderer>())
        {
            Trails.Add(trail);
            if (trail.GetComponent<VFXBulletSpeedCalibration>())
            {
                VFXBulletSpeedCalibration vfx = trail.GetComponent<VFXBulletSpeedCalibration>();
                TrailInitialTime.Add(vfx.trailTimeCache * (vfx.BulletDuration/ vfx.BulletOriginalDuration));
            }
            else
            {
                TrailInitialTime.Add(trail.time);
            }
        }
 
        initialized = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (initialized)
        {
            TrailStateUpdate();
        }
        
    }
    /// <summary>
    /// Set the Particle Systems Time according to PSTime variable, if put to 0 it will stop immediately
    /// </summary>
    public void UpdatePSTime()
    {
        //set the simulation time in every particle, minding the the simulation speed
        foreach (ParticleSystem PS in GetComponentsInChildren<ParticleSystem>())
        {
            var m = PS.main;
            m.loop = false;
            //To change the duration the particle needs to pe paused
            PS.Stop();
            m.duration = PSTime / m.simulationSpeed;
            //check if the particle is finished
            PS.Play();
            DisableTrail = (PS.time>=m.duration);
        }
        foreach (ParticleSystem p in LongParticles)
        {
            p.Pause();
            //p.gameObject.SetActive(false);
            var m = p.main;
            m.startLifetime = m.duration;
            // p.gameObject.SetActive(true);
            p.Play();
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

    //if the particles have finished emitting particles, disable the trail in time
    private void TrailStateUpdate()
    {
        if (DisableTrail)
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
        else
        {
            for (int i = 0; i < Trails.Count; i++)
            {
                TrailRenderer trail = Trails[i];
                trail.emitting = true;
                trail.time = TrailInitialTime[i];
            }
        }
    }
}
