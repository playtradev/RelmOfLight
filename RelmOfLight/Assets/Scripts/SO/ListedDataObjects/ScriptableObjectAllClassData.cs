using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MyBox;

[CreateAssetMenu(fileName = "New Character Class Data Profile", menuName = "ScriptableObjects/Classes/ClassDataProfile")]
public class ScriptableObjectAllClassData : ScriptableObject
{
    public ScriptableObjectClassData[] ClassDatas = new ScriptableObjectClassData[0];
    public ScriptableObjectClassData GetClassDatas(CharacterClassType CharacterClass) => ClassDatas.Where(r => r.Type == CharacterClass).FirstOrDefault();
}