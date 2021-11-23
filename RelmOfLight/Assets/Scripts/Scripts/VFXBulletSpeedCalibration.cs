using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXBulletSpeedCalibration : MonoBehaviour
{
    //PUBLIC
    public bool Trail = true;
    public bool Particle = true;
    [Header("Duration in seconds desired and original")]
    //use this to set the new time
    public float BulletDuration = 1;
    //use this to set the speed in which the particle was created
    public float BulletOriginalDuration = 1;


    //PRIVATE
    TrailRenderer trail;
    ParticleSystem particle;
    [HideInInspector]
    public float trailTimeCache;
    float gravityMultiplierCache;
    float noiseMultiplierCache;
    float startSpeedMultiplier;
    float startLife;
    AnimationCurve speedModifierCache;

    private void Awake()
    {
        if(GetComponents<TrailRenderer>().Length>0)
        {
            trail = GetComponent<TrailRenderer>();
            trailTimeCache = trail.time;
        }
        if (GetComponents<ParticleSystem>().Length > 0)
        {
            particle = GetComponent<ParticleSystem>();
            var main = particle.main;
            startLife = main.startLifetimeMultiplier;
            startSpeedMultiplier = main.startSpeedMultiplier;
            gravityMultiplierCache = main.gravityModifierMultiplier;
            var velocityOverLifetime = particle.velocityOverLifetime;
            speedModifierCache = velocityOverLifetime.speedModifier.curve;
            noiseMultiplierCache = particle.noise.strengthMultiplier;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        MultiplyTime();
    }

    /// <summary>
    /// Change the time of trails and particles to fit the parametrized time;
    /// </summary>
    public void MultiplyTime()
    {
        if (Trail && trail != null)
        {
            trail.time = trailTimeCache * (BulletDuration / BulletOriginalDuration);
        }
        if (Particle && particle != null)
        {
            var main = particle.main;
            var velocityOverLifetime = particle.velocityOverLifetime;
            main.startSpeedMultiplier = (BulletOriginalDuration / BulletDuration) * startSpeedMultiplier;
            main.startLifetimeMultiplier = (BulletDuration / BulletOriginalDuration)*startLife;
            main.gravityModifierMultiplier = (BulletOriginalDuration / BulletDuration)*gravityMultiplierCache;
            var noise = particle.noise;
            noise.strengthMultiplier = (BulletOriginalDuration / BulletDuration) * noiseMultiplierCache;
            velocityOverLifetime.speedModifier = new ParticleSystem.MinMaxCurve(BulletOriginalDuration / BulletDuration, speedModifierCache);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
