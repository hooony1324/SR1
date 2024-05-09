using System;
using System.Collections.Generic;
using Spine;
using static Define;

public class GuidedAreaAttack : AreaSkill
{
    public override void SetInfo(int skillId)
    {
        base.SetInfo(skillId);

        AddIndicatorComponent();
        
        if (_indicator != null)
            _indicator.SetInfo(Owner, SkillData, EIndicatorType.Cone);
    }

    protected override void OnAttackEvent(TrackEntry arg1, Spine.Event arg2)
    {
        List<InteractionObject> targets = Managers.Object.FindConeRangeTargets(Owner, _skillDir, Util.GetEffectRadius(SkillData.EffectSize), _angleRange);
        foreach (var target in targets)
        {
            if (target.IsValid())
            {
                ApplyEffects(target);
            }
        }
    }
    
    public override void DoSkill(Action callback = null)
    {
        base.DoSkill(callback);

        //SkillDuration의 길이 만큼 스펠 인디케이터 길이 설정(스파인의 이벤트의 시간 구하는게 너무 비효율적임) 
        _indicateDuration = SkillData.Duration;
        _indicator.ShowCone(Owner.transform.position, _skillDir.normalized, _angleRange, _indicateDuration);
    }

}