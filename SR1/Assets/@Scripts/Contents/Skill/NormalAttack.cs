using System;
using Spine;
using Event = Spine.Event;

public class NormalAttack : SkillBase
{
    protected override bool Init()
    {
        base.Init();
        
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
        base.OnAttackEvent(arg1, arg2);
        
        if (!SkillTarget.IsValid()) 
            return;
        
        if (SkillData.ProjectileId == 0)
        {
            ApplyEffects(SkillTarget);
        }
        else
        {
            //RangedAttack
            if (SkillTarget.ObjectType != Define.EObjectType.Env)
            {
                GenerateProjectile(Owner.FireSocketPos);
            }
            else
            {
                ApplyEffects(SkillTarget);
            }
        }
    }

}