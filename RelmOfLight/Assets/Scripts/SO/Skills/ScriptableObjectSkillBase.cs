using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class ScriptableObjectSkillBase : ScriptableObject
{
    public bool IsPlayerAttack => !name.ToUpper().Contains("ENEMY");

    /// <summary>
    /// The lowest level requirement needed to use this skill
    /// </summary>
    //public virtual int MinLevel { get; set; }
    //public virtual int MaxLevel { get; set; }

    [SerializeField] protected string _SkillID = "";
    public string SkillID
    {
        get
        {
            if (_SkillID == "" && name != "")
                _SkillID = name.ToUpper().Replace(' ', '_');
            return _SkillID;
        }
        set
        {
            _SkillID = value;
        }
    }
    public string AttackDisplayName = "SKILL_NAME";
    public string AttackDescription = "DESCRIPTION";
    [Tooltip("Used in the UI to show the players an example of the skill, will default to a fallback if not set")] public VideoClip AttackVideoClip;
    [Tooltip("Used in the battle UI to show the ability")] public Sprite AttackIcon;
    public AttackDisplayType attackDisplayType = AttackDisplayType.Damage;
    public enum AttackDisplayType { Damage, Buff, Debuff }
    public bool IsMaskAttack = false;
}
