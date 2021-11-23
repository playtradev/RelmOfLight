using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleNameChanger : MonoBehaviour
{
    public bool ChangeName = true;
    public bool DestroyAfterUse = true;
    public bool ApplyScalingMode = true;
    [Tooltip("number of object to exclode from naming, with 1 you exclude only this object")]
    public int ExcludedParents = 1;
    public bool ChangeSortingName = true;
    public string SortingLayer = "Player";
    public ParticleSystemScalingMode ScalingMode = ParticleSystemScalingMode.Hierarchy;
    public void NameParticles()
    {
        string name = "";
        string s = "";
        int i = 0;
        foreach (ParticleSystem p in GetComponentsInChildren<ParticleSystem>())
        {
            var main = p.main;
            main.scalingMode = ScalingMode;
            if(i>= ExcludedParents && ChangeName)
            {
                name = "";
                var m = p.textureSheetAnimation;
                ParticleSystemRenderer r = p.GetComponent<ParticleSystemRenderer>();
                if (ChangeSortingName)
                    r.sortingLayerName = SortingLayer;
                if (p.trails.enabled)//if a trail is activate, write it + the material name
                {
                    name += "ParticleTrail_";
                    s = r.trailMaterial.name;
                    name += s + "_";
                }
                else//if it's a normal particle, write it + the material name
                {
                    name += "Particle_";
                    if (r.sharedMaterial != null)
                    {
                        s = r.sharedMaterial.name;
                        name += s;
                    }
                }
                if (m.spriteCount >= 1)//check if the sprite exist and add its name to the string
                {
                    if (m.GetSprite(0) != null)
                    {
                        s = m.GetSprite(0).name;
                        name += "_" + s;
                    }
                        
                }
                p.gameObject.name = name;//Write the name in the game object
            }
            i++;
        }

        if(DestroyAfterUse)
            DestroyImmediate(gameObject.GetComponent<ParticleNameChanger>());
    }
}
