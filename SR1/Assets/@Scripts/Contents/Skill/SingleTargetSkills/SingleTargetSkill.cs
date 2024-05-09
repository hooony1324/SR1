using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Spine;
using UnityEngine;
using static Define;
using Event = Spine.Event;

public class SingleTargetSkill : SkillBase
{
    private bool _isHeal = false;
    
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        SkillType = ESkillType.SingleTargetSkill;

        return true;
    }

    public override void SetInfo(int skillId)
    {
        base.SetInfo(skillId);
        _isHeal = IsHeal();
    }

    public override void DoSkill(Action callback = null)
    {
        base.DoSkill(callback);
        if (Owner.CreatureState != ECreatureState.Skill)
            return;
        //1. 이로운 효과(힐)이면 SkillTarget을 다시 찾는다.(기본적으로 적타겟만 잡게 되어있어서)
        if (_isHeal)
        {
            List<InteractionObject> targets =Managers.Object.FindCircleRangeTargets(Owner.Position, SkillData.SkillRange, Owner.ObjectType, true);
            List<Creature> creatures = targets.OfType<Creature>().ToList();
            Creature minHpCreature = creatures.OrderBy(c => c.Hp).FirstOrDefault();

            SkillTarget = minHpCreature;
            LookAtTarget(SkillTarget);
        }

    }

    protected override void OnAttackEvent(TrackEntry arg1, Event arg2)
    {
        base.OnAttackEvent(arg1, arg2);
        
        Apply();
    }

    private void Apply()
    {
        if (!SkillTarget.IsValid()) 
            return;
        
        if (SkillData.ProjectileId == 0)
        {
            //MeleeAttack
            ApplyEffects(SkillTarget);
        }
        else
        {
            //RangedAttack
            if (Owner.Target.ObjectType != EObjectType.Env)
            {
                GenerateProjectile(Owner.FireSocketPos);
            }
            else
            {
                ApplyEffects(SkillTarget);
            }
        }
    }

    private bool IsHeal()
    {
        foreach (int id in SkillData.EffectIds)
        {
            if(Managers.Data.EffectDic.TryGetValue(id, out EffectData effectData) == false)
                continue;

            if (effectData.Amount < 0)
                return true;
        }

        return false;
    }
}