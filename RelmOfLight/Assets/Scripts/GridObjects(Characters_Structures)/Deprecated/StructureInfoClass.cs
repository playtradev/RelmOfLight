using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Spine;
using Spine.Unity;

public class StructureInfoClass : BaseInfoScript
{
    [Header("Display___________________________________")]
    [Tooltip("Sets the character up with the corresponding components")] public StructureDisplayType displayType = StructureDisplayType.Spine;
    //[Tooltip("When enabled, will automatically configure the gameobject with the missing components for the displayType")] public bool generateDisplayComponents = false;









    private void OnValidate()
    {

    }

    void OnValidate_DisplayType()
    {
        //if(displayType == StructureDisplayType.Spine)
        //{
        //    if (GetComponent<SpriteRenderer>() != null) DestroyImmediate(GetComponent<SpriteRenderer>());
        //    if (GetComponent<UnityEngine.Animation>() != null) DestroyImmediate(GetComponent<UnityEngine.Animation>());
        //    if (GetComponent<MeshFilter>() == null) gameObject.AddComponent(typeof(MeshFilter));
        //    if (GetComponent<MeshRenderer>() == null) gameObject.AddComponent(typeof(MeshRenderer));
        //    if (GetComponent<SkeletonAnimation>() == null) gameObject.AddComponent(typeof(SkeletonAnimation));
        //    if (GetComponent<SpineAnimationManager>() == null) gameObject.AddComponent(typeof(SpineAnimationManager));
        //}
        //else if(displayType == StructureDisplayType.Sprite)
        //{
        //    if (GetComponent<MeshFilter>() != null) DestroyImmediate(GetComponent<MeshFilter>());
        //    if (GetComponent<SkeletonAnimation>() != null) DestroyImmediate(GetComponent<SkeletonAnimation>());
        //    if (GetComponent<SpineAnimationManager>() != null) DestroyImmediate(GetComponent<SpineAnimationManager>());
        //    if (GetComponent<MeshRenderer>() != null) DestroyImmediate(GetComponent<MeshRenderer>());
        //    if (GetComponent<SpriteRenderer>() == null) gameObject.AddComponent(typeof(SpriteRenderer));
        //    if (GetComponent<UnityEngine.Animation>() == null) gameObject.AddComponent(typeof(UnityEngine.Animation));
        //}
    }
}

public enum StructureDisplayType { Spine, Sprite }