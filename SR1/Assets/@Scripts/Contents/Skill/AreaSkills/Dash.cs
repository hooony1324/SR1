using System;
using System.Collections;
using System.Collections.Generic;
using Spine;
using UnityEngine;
using static Define;
using Random = UnityEngine.Random;

public class Dash : AreaSkill
{
    Vector3 dir;
    Vector3Int dest = Vector3Int.zero;
    Vector3 worldDest = Vector3.zero;
    private float length;
    bool isFindPos = false;

    public override void SetInfo(int skillId)
    {
        base.SetInfo(skillId);
        _indicatorType = EIndicatorType.Rectangle;
        AddIndicatorComponent();
        if (_indicator != null)
            _indicator.SetInfo(Owner, SkillData, EIndicatorType.Rectangle);
    }
    
    public override void DoSkill(Action callback = null)
    {
        base.DoSkill(callback);
       
        List<InteractionObject> targets = Managers.Object.FindCircleRangeTargets(SkillTarget.Position, SkillData.SkillRange, Owner.ObjectType);
        SkillTarget = targets[Random.Range(0, targets.Count)];

        if (Owner.CreatureState != ECreatureState.Skill)
            return;

        MakeIndicator();
    }

    public override void CancelSkill()
    {
        if (_indicator)
            _indicator.Cancel();
    }

    protected override void OnAttackEvent(TrackEntry arg1, Spine.Event arg2)
    {
        if(isFindPos)
            StartCoroutine(DoDash());
    }

    private void MakeIndicator()
    {
        _indicateDuration = CastingTrackEntry.Animation.Duration - MIX_DURATION;
        
        // 1. 넉백 방향 결정
        dir = (SkillTarget.Position - Owner.Position).normalized;
        
        // 2. 셀 이동
        for (float i = SkillData.SkillRange; i >= 3; i -= 0.5f)
        {
            dest = Managers.Map.World2Cell(Owner.Position + dir * i);
            worldDest = Owner.Position + dir * i;
            if (Managers.Map.CanGo(Owner, dest, true))
            {
                isFindPos = true;
                break;
            }
        }
        
        //인디케이터 생성
        length = Vector3.Distance(Owner.Position, worldDest);
        _indicator.ShowRectangle(Owner.transform.position, _skillDir.normalized, _indicateDuration, length);

    }

    IEnumerator DoDash()
    {
        const float dashSpeed = 20;

        Owner.MoveSpeed += dashSpeed; 
        Owner.MoveToCellPos(dest);
        Owner.CurrentCollider.isTrigger = false;
        
        float width = Owner.CurrentCollider.radius * 2f;
        List<InteractionObject> foundTargets = new List<InteractionObject>();
        foundTargets = Managers.Object.FindRectRangeTargets(Owner.Position, SkillData.SkillRange, width, length, dir,
            Owner.ObjectType);
        
        foreach (var target in foundTargets)
        {
            ApplyEffects(target);
        }
        
        while (Owner.LerpCellPosCompleted == false)
        {
            yield return null;
        }
        
        // 넉백 상태가 끝난 후 상태 복귀
        Owner.CreatureState = ECreatureState.Idle;
        Owner.MoveSpeed -= dashSpeed; 
    }

}
