using System;
using Scripts.Components.Skill;
using Spine;
using UnityEngine;

public class AreaSkill : SkillBase
{
    protected SpellIndicator _indicator;
    protected Vector2 _skillDir;
    protected Define.EIndicatorType _indicatorType = Define.EIndicatorType.Cone;
    protected int _angleRange = 360;
    protected float _indicateDuration = -1;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        SkillType = Define.ESkillType.AreaSkill;
        return true;
    }

    public override void SetInfo(int skillId)
    {
        base.SetInfo(skillId);
        _angleRange = Util.GetAngleRange(SkillData.EffectSize);
        // if(ObjectType == Define.EObjectType.Monster  /*&& Casting Anim이 있으면*/)
    }

    protected override void OnAttackEvent(TrackEntry arg1, Spine.Event arg2)
    {
        base.OnAttackEvent(arg1, arg2);

    }

    public override void DoSkill(Action callback = null)
    {
        base.DoSkill(callback);
        if (Owner.CreatureState != Define.ECreatureState.Skill)
            return;

        _skillDir = (SkillTarget.transform.position - Owner.Position).normalized;
    }

    public override void CancelSkill()
    {
        if (_indicator)
            _indicator.Cancel();
    }

    protected void AddIndicatorComponent()
    {
        // _indicator = Util.FindChild<SpellIndicator>(gameObject, recursive: true);
        // if (_indicator == null)
        // {
        GameObject go = Managers.Resource.Instantiate("SpellIndicator", gameObject.transform);
        _indicator = Util.GetOrAddComponent<SpellIndicator>(go);
        // }
    }
}