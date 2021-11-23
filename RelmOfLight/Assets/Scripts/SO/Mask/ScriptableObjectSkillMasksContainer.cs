using MyBox;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ScriptableObjectSkillMasksContainer")]
public class ScriptableObjectSkillMasksContainer : ScriptableObject
{
    public ScriptableObjectSkillMask[] Masks;
}
