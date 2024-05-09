using System;
using Spine;
using UnityEngine;
using static Define;
using Event = Spine.Event;

public class ComboSkill : SkillBase
{
    public int HitCount;
    
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        SkillType = ESkillType.ComboSkill;

        return true;
    }

    public override void DoSkill(Action callback = null)
    {
        base.DoSkill(callback);
        Debug.Log($"DoSkill");
        if (Owner.CreatureState != ECreatureState.Skill)
            return;
    }

    protected override void OnAttackEvent(TrackEntry arg1, Event arg2)
    {
        base.OnAttackEvent(arg1, arg2);

        if (SkillData.ProjectileId > 0)
        {
            GenerateProjectile(Owner.FireSocketPos);
            return;
        }

        //톰 같은 동시에 두마리 때리는애들은?
        //if skilldata.targetCount ==1 일때랑 그 이상일때 나눠서
        if (SkillTarget.IsValid() && Owner.Target.ObjectType == Util.DetermineTargetType(Owner.ObjectType, false))
        {
            float dist = (SkillTarget.Position - Owner.Position).sqrMagnitude;
            if(dist < Owner.GetAttackDistanceSqr())
                ApplyEffect(SkillTarget, SkillData.EffectIds[HitCount]);
        }


        HitCount++;
        if (SkillData.EffectIds.Count == HitCount)
            HitCount = 0;
    }
    
    protected override void MotionFinished(Vector3 endPos, Vector3 targetPos, int skillTempleteId)
    {
        base.MotionFinished(endPos, targetPos, skillTempleteId);
        if(skillTempleteId != SkillData.TempleteId)
            return;

        if (SkillTarget.IsValid())
        {
            ApplyEffect(SkillTarget, SkillData.EffectIds[HitCount]);
        }
       
        HitCount++;
        if (SkillData.EffectIds.Count == HitCount)
            HitCount = 0;
    }
}
