using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGridObject
{
    CharacterNameType CharName { get; }
    GameObject ReceiverGO { get; }
    Vector2Int GridPosition { get; }
    BaseInfoScript InfoScript { get; }
    List<Vector2Int> GridPositions { get; }
    bool IsOnField { get; set; }

    BaseCharacter ReferenceCharacter { get; }
}

public interface IDamageReceiver : IGridObject
{
    float Health { get; set; }
    bool died { get; set; }

    DefendingActionType SetDamage(DamageInfoClass damageInfo, float damage);
}

public interface IDamageMaker : IGridObject
{
    void MadeDamage(IDamageReceiver target, float damage);
}

public interface ISpineCharacter : IGridObject
{
    SpineAnimationManager SpineAnim { get; set; }
    Transform spineT { get; set; }
    Vector3 LocalSpinePosoffset { get; set; }
    float AnimSpeed { get; set; }

    void SpineAnimatorsetup(bool showHP, bool showEther, bool setSpine);
    void SpineAnimationState_Event(Spine.TrackEntry trackEntry, Spine.Event e);
    void SetAnimation(string animState, bool loop = false, float transition = 0, bool _pauseOnLastFrame = false, bool useFormeNaming = true, bool overrideRules = false);
    void SpineAnimationState_Complete(Spine.TrackEntry trackEntry);
}