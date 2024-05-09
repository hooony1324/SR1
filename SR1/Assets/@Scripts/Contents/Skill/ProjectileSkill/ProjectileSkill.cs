using System;
using Spine;
using UnityEngine;
using Event = Spine.Event;

public class ProjectileSkill : SkillBase
{
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        SkillType = Define.ESkillType.ProjectileSkill;

        return true;
    }

    public override void DoSkill(Action callback = null)
    {
        base.DoSkill(callback);
        if (Owner.CreatureState != Define.ECreatureState.Skill)
            return;
    }

    protected override void OnAttackEvent(TrackEntry arg1, Event arg2)
    {
        if(Owner.Skills.CurrentSkill.SkillData.TempleteId == SkillData.TempleteId)
            GenerateProjectile(Owner.FireSocketPos);
    }

    protected override void MotionFinished(Vector3 endPos, Vector3 targetPos, int skillTempleteId)
    {
        base.MotionFinished(endPos, targetPos, skillTempleteId);
        
        // Projectile 자체데미지는 Projectile Class에서 함
        
        if (skillTempleteId == SkillData.TempleteId)
        {
            if (SkillData.AoEId != 0)
            {
                GenerateAoE(endPos, targetPos);
            }
        }

    }
}