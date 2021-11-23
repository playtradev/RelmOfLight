//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System.Linq;

//public class BuffDebuffsClasses
//{
//    public delegate IEnumerator BuffDebuffCoroutine(BaseCharacter character, BuffDebuffClass bdClass, BaseCharacter target, BaseCharacter meleeTarget, ref float val, ref float totalchangesVal, ref Vector2 totalchangesValV, ref Vector2 valV, ref ElementalType prevElemental, ref WalkingSideType prevWalking, ref Vector3 boxColliderSize);

//    public Dictionary<BuffDebuffStatsType, BuffDebuffCoroutineGroup> _CoroutineGroups = null;
//    public Dictionary<BuffDebuffStatsType, BuffDebuffCoroutineGroup> CoroutineGroups
//    {
//        get
//        {
//            if(_CoroutineGroups == null)
//                _CoroutineGroups = new Dictionary<BuffDebuffStatsType, BuffDebuffCoroutineGroup>
//                {
//                    { BuffDebuffStatsType.Regen, new BuffDebuffCoroutineGroup(Regen_Start, Regen_During, Regen_End) },
//                    { BuffDebuffStatsType.Drain, new BuffDebuffCoroutineGroup(Drain_Start, Drain_During, Drain_End) },
//                    { BuffDebuffStatsType.Zombie, new BuffDebuffCoroutineGroup(Zombie_Start, Zombie_During, Zombie_End) },
//                    { BuffDebuffStatsType.Bleed, new BuffDebuffCoroutineGroup(Bleed_Start, Bleed_During, Bleed_End) },
//                    { BuffDebuffStatsType.AttackChange, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.Legion, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.Rage, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.Element, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.HP, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.HP_Regen, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.Armour, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.ArmourType, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.HP_Regen_OnGrid_OnOff, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.StopChar, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.Speed_Base, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.Speed_Movement, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.Speed_Weak_Bullet, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.Speed_Strong_Bullet, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.Damage_Base, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.Damage_Weak_Damage_Multiplier, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.Damage_Strong_Damage_Multiplier, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.Shield, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.Shield_Regen, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.Shield_Absorbtion_Weak, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.Shield_Absorbtion_Strong, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.Shield_InvulnerabilityTime, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.Shield_Minion_Normal_Chances, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.Shield_Minion_Perfect_Chances, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.Ether, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.SoulClash, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.Bliss, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.Ether_Regen, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.Ether_Regen_OnGrid_OnOff, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.Luck_Chances, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.CriticalChances_Weak, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.CriticalChances_Strong, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.Teleport, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.MeleeAttack, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.ScaleCharacterSize, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.WalkingSide, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.ShadowForm, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.KillPoolChar, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.EndWave, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.BoxColliderSize, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.ActionDisable_WeakAttack, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.ActionDisable_StrongAttack, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.ActionDisable_Skill1, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.ActionDisable_Skill2, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.ActionDisable_Mask, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.ActionDisable_Move, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.ActionDisable_Swap, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.Tile_Free, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.ChancgeColor, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.ForceAI, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.StealAttack, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.RemoveBuffs, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.RemoveDebuffs, new BuffDebuffCoroutineGroup() },
//                    { BuffDebuffStatsType.FireParticlesToChar, new BuffDebuffCoroutineGroup() },
//                };

//            return _CoroutineGroups;
//        }
//    }

//    IEnumerator Regen_Start(BaseCharacter character, BuffDebuffClass bdClass, BaseCharacter target, BaseCharacter meleeTarget, ref float val, ref float totalchangesVal, ref Vector2 totalchangesValV, ref Vector2 valV, ref ElementalType prevElemental, ref WalkingSideType prevWalking, ref Vector3 boxColliderSize)
//    {
//        val = character.GetBuffDebuffValue(bdClass, bdClass.Effect.BaseCurrentValue ? target.CharInfo.HealthStats.B_Health : target.CharInfo.HealthStats.Health);
//        target.CharInfo.Health += val;
//        character.StatsChangedEvent?.Invoke(val, BattleFieldIndicatorType.Heal, target);
//        EventManager.Instance?.UpdateHealth(target);
//        break;
//    }
//    IEnumerator Regen_During(BaseCharacter character, BuffDebuffClass bdClass, BaseCharacter target, BaseCharacter meleeTarget, ref float val, ref float totalchangesVal, ref Vector2 totalchangesValV, ref Vector2 valV, ref ElementalType prevElemental, ref WalkingSideType prevWalking, ref Vector3 boxColliderSize)
//    IEnumerator Regen_End(BaseCharacter character, BuffDebuffClass bdClass, BaseCharacter target, BaseCharacter meleeTarget, ref float val, ref float totalchangesVal, ref Vector2 totalchangesValV, ref Vector2 valV, ref ElementalType prevElemental, ref WalkingSideType prevWalking, ref Vector3 boxColliderSize)

//    IEnumerator Drain_Start(BaseCharacter character, BuffDebuffClass bdClass, BaseCharacter target, BaseCharacter meleeTarget, ref float val, ref float totalchangesVal, ref Vector2 totalchangesValV, ref Vector2 valV, ref ElementalType prevElemental, ref WalkingSideType prevWalking, ref Vector3 boxColliderSize)
//    IEnumerator Drain_During(BaseCharacter character, BuffDebuffClass bdClass, BaseCharacter target, BaseCharacter meleeTarget, ref float val, ref float totalchangesVal, ref Vector2 totalchangesValV, ref Vector2 valV, ref ElementalType prevElemental, ref WalkingSideType prevWalking, ref Vector3 boxColliderSize)
//    IEnumerator Drain_End(BaseCharacter character, BuffDebuffClass bdClass, BaseCharacter target, BaseCharacter meleeTarget, ref float val, ref float totalchangesVal, ref Vector2 totalchangesValV, ref Vector2 valV, ref ElementalType prevElemental, ref WalkingSideType prevWalking, ref Vector3 boxColliderSize)

//    IEnumerator Zombie_Start(BaseCharacter character, BuffDebuffClass bdClass, BaseCharacter target, BaseCharacter meleeTarget, ref float val, ref float totalchangesVal, ref Vector2 totalchangesValV, ref Vector2 valV, ref ElementalType prevElemental, ref WalkingSideType prevWalking, ref Vector3 boxColliderSize)
//    IEnumerator Zombie_During(BaseCharacter character, BuffDebuffClass bdClass, BaseCharacter target, BaseCharacter meleeTarget, ref float val, ref float totalchangesVal, ref Vector2 totalchangesValV, ref Vector2 valV, ref ElementalType prevElemental, ref WalkingSideType prevWalking, ref Vector3 boxColliderSize)
//    IEnumerator Zombie_End(BaseCharacter character, BuffDebuffClass bdClass, BaseCharacter target, BaseCharacter meleeTarget, ref float val, ref float totalchangesVal, ref Vector2 totalchangesValV, ref Vector2 valV, ref ElementalType prevElemental, ref WalkingSideType prevWalking, ref Vector3 boxColliderSize)

//    IEnumerator Bleed_Start(BaseCharacter character, BuffDebuffClass bdClass, BaseCharacter target, BaseCharacter meleeTarget, ref float val, ref float totalchangesVal, ref Vector2 totalchangesValV, ref Vector2 valV, ref ElementalType prevElemental, ref WalkingSideType prevWalking, ref Vector3 boxColliderSize)
//    IEnumerator Bleed_During(BaseCharacter character, BuffDebuffClass bdClass, BaseCharacter target, BaseCharacter meleeTarget, ref float val, ref float totalchangesVal, ref Vector2 totalchangesValV, ref Vector2 valV, ref ElementalType prevElemental, ref WalkingSideType prevWalking, ref Vector3 boxColliderSize)
//    IEnumerator Bleed_End(BaseCharacter character, BuffDebuffClass bdClass, BaseCharacter target, BaseCharacter meleeTarget, ref float val, ref float totalchangesVal, ref Vector2 totalchangesValV, ref Vector2 valV, ref ElementalType prevElemental, ref WalkingSideType prevWalking, ref Vector3 boxColliderSize)


//}

//public struct BuffDebuffCoroutineGroup
//{
//    BuffDebuffsClasses.BuffDebuffCoroutine StartCo;
//    BuffDebuffsClasses.BuffDebuffCoroutine DuringCo;
//    BuffDebuffsClasses.BuffDebuffCoroutine EndCo;

//    public BuffDebuffCoroutineGroup(BuffDebuffsClasses.BuffDebuffCoroutine StartCo, BuffDebuffsClasses.BuffDebuffCoroutine DuringCo, BuffDebuffsClasses.BuffDebuffCoroutine EndCo)
//    {
//        this.StartCo = StartCo;
//        this.DuringCo = DuringCo;
//        this.EndCo = EndCo;
//    }
//}