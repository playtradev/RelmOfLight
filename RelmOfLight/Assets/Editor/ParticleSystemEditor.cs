using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


//[CustomEditor(typeof(ParticleSystem))]
public class ParticleSystemEditor : Editor
{
    bool isHelper = false;
    public override void OnInspectorGUI()
    {
        ParticleSystem origin = (ParticleSystem)target;
        if(origin.transform.parent == null)
        {
            List<Component> removeComponent = new List<Component>();
            foreach (Component item in origin.GetComponents(typeof(Component)))
            {
                if(item.GetType() == typeof(ParticleSystem))
                {

                }
                else if (item.GetType() == typeof(ParticleHelperScript))
                {
                    isHelper = true;
                }
                else if (item.GetType() == typeof(Transform))
                {
                }
                else if (item.GetType() == typeof(VFXOffsetToTargetVOL))
                {
                }
                else if (item.GetType() == typeof(ParticleSystemRenderer))
                {
                }
                else 
                {
                    removeComponent.Add(item);
                }
            }
            if(!isHelper)
            {
                origin.gameObject.AddComponent<ParticleHelperScript>();

            }
            test(removeComponent);
            EditorUtility.SetDirty(origin);

        }
        base.OnInspectorGUI();

    }


    void test(List<Component> removeComponent)
    {
        foreach (var item in removeComponent)
        {
            DestroyImmediate(item, true);
        }
    }
}
