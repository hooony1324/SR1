using System.Collections;
using Data;
using DG.Tweening;
using UnityEngine;
using static Define;

public class Monster : Creature
{
    public MonsterData MonsterData;
    public bool IsReturning = false;
    Vector3 _destPos;

    protected override bool Init()
    {
        base.Init();
        ObjectType = EObjectType.Monster;
        return true;
    }

    public override void SetInfo(int templateId)
    {
        base.SetInfo(templateId);
        MonsterData = CreatureData as MonsterData;

        ExtraCells = MonsterData.Size;
        SkeletonAnim.skeleton.A = 1;
        // stat
        CreatureState = ECreatureState.Idle;
    }

    protected override float CalculateFinalStat(float baseValue, ECalcStatType calcStatType)
    {
        float finalValue = baseValue;
        finalValue += Effects.GetStatModifier(calcStatType, EStatModType.Add);
        finalValue *= 1 + Effects.GetStatModifier(calcStatType, EStatModType.PercentAdd);
        finalValue *= 1 + Effects.GetStatModifier(calcStatType, EStatModType.PercentMult);

        return finalValue;
    }
    
    protected override void UpdateAnimation()
    {
        base.UpdateAnimation();
        switch (CreatureState)
        {
            case ECreatureState.Idle:
                break;
            case ECreatureState.Skill:
                break;
            case ECreatureState.Move:
                break;
            case ECreatureState.OnDamaged:
                break;
            case ECreatureState.Dead:
                break;
            default:
                break;
        }
    }

    #region AI
    protected override void UpdateIdle()
    {
        base.UpdateIdle();
    }

    protected override void UpdateMove()
    {
        if (Target.IsValid() == false)
        {
            // CreatureState = ECreatureState.Idle;
            // return;
            
            // Move
            FindPathAndMoveToCellPos(SpawnInfo.CellPos, MONSTER_DEFAULT_MOVE_DEPTH);

            if (LerpCellPosCompleted)
            {
                CreatureState = ECreatureState.Idle;
                IsReturning = false;
                return;
            }
            return;
        }
        
        Vector3 dir = (Target.CenterPosition - CenterPosition);
        float distToTargetSqr = dir.sqrMagnitude;
        float attackDistanceSqr = GetAttackDistanceSqr();

        if (distToTargetSqr <= attackDistanceSqr)
        {
            // 공격 범위 이내로 들어왔다면 공격.
            CreatureState = ECreatureState.Skill;
            return;
        }    
        else
        {
            // 너무 멀리 왔으면 원래자리로 되돌아감 (캠프모드일떄는 돌아가지않음)
            // float range = SCAN_RANGE * SCAN_RANGE;
            // float distToSpawnPosSqr = (Position - SpawnInfo.WorldPos).sqrMagnitude;
            // if (distToSpawnPosSqr > range && Managers.Object.HeroCamp.CampState != ECampState.CmapMode)
            // {
            //     Target = null;
            //     return;
            // }

            float distToSpawnPosSqr = (Position - SpawnInfo.WorldPos).sqrMagnitude;
            float range = SCAN_RANGE * SCAN_RANGE;
            if (distToSpawnPosSqr > range && Managers.Object.HeroCamp.CampState != ECampState.CampMode)
            {
                Target = null;
                IsReturning = true;
                return;
            }
            
            // 공격 범위 밖이라면 추적.
            ChaseOrAttackTarget(GetAttackDistanceSqr() ,SCAN_RANGE );

        }
    }

    protected override void UpdateDead()
    {
        base.UpdateDead();
    }

    #endregion
    
    protected override void OnDead()
    {
        base.OnDead();

        BroadcastOnDead();
        
        DropItem(MonsterData.DropItemId);
        // Broadcast
        Managers.Game.BroadcastEvent(EBroadcastEventType.KillMonster, ECurrencyType.None, MonsterData.TemplateId);

        StartCoroutine(CoOndead());
    }
    
    IEnumerator CoOndead()
    {
        yield return new WaitForSeconds(1f);
        DOTween.To(()=> SkeletonAnim.skeleton.A, x=> SkeletonAnim.skeleton.A = x, 0, 0.5f).OnComplete(
            () =>
            {
                Managers.Object.Despawn(this);
            });
    }
}
