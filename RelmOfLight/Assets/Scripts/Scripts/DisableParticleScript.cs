using System.Collections.Generic;
using UnityEngine;

public class DisableParticleScript : MonoBehaviour
{
    private ParticleSystem ps;
    private List<ParticleChildSimulationSpeed> Children = new List<ParticleChildSimulationSpeed>();
    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();

        foreach (ParticleSystem item in GetComponentsInChildren<ParticleSystem>(true))
        {
            Children.Add(new ParticleChildSimulationSpeed(item.main.simulationSpeed, item, item.main.duration, item.main.startDelay.constant));
        }
    }

    void Update()
    {
        if(!ps.IsAlive(true))
        {
            ResetParticle();
        }
    }

    private void OnEnable()
    {
        foreach (TrailRenderer item in GetComponentsInChildren<TrailRenderer>())
        {
            item.Clear();
        }
    }

    private void OnDisable()
    {
        SetSimulationSpeedToBase();
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

    public void SetSimulationSpeed(float speed)
    {
        foreach (ParticleChildSimulationSpeed item in Children)
        {
            var main = item.Child.main;
            main.simulationSpeed *= speed;
        }
    }
}


