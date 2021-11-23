using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Class Data", menuName = "ScriptableObjects/Skins")]
public class ScriptableObjectMenuSkin : ScriptableObject
{
    public List<ScriptableObjectCharacterPrefab> test = new List<ScriptableObjectCharacterPrefab>();
    public List<MenuSkinClass> MenuSkins = new List<MenuSkinClass>();
}

[System.Serializable]
public class MenuSkinClass
{
    public CharacterNameType CharId;
    public SkeletonDataAsset Skeleton;

    public MenuSkinClass()
    {

    }

    public MenuSkinClass(CharacterNameType charId, SkeletonDataAsset skeleton)
    {
        CharId = charId;
        Skeleton = skeleton;
    }
}
