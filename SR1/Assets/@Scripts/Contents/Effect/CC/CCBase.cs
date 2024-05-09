using System.Collections;
using UnityEngine;
using static Define;

public class CCBase : EffectBase
{
    protected ECreatureState lastState;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines();
        _knockbackCoroutine = null;
    }

    public override void ApplyEffect()
    {
        base.ApplyEffect();

        lastState = Owner.CreatureState;
        if (lastState == ECreatureState.OnDamaged)
            return;

        Owner.CreatureState = ECreatureState.OnDamaged;

        switch (EffectData.EffectType)
        {
            case EEffectType.Knockback:
                if (_knockbackCoroutine == null)
                {
                    _knockbackCoroutine = StartCoroutine(DoKnockback());
                }
                break;
            case EEffectType.Airborne:
                StopCoroutine((DoAirborn(lastState)));
                StartCoroutine(DoAirborn(lastState));
                break;
            case EEffectType.Stun:
            case EEffectType.Pull:
            case EEffectType.Freeze:
                StartCoroutine(StartTimer());
                break;
        }
    }

    public override bool ClearEffect(EEffectClearType clearType)
    {
        base.ClearEffect(clearType);

        return false;
    }
  
    #region Airborn
    private float _airborneDistance = 5f;

    IEnumerator DoAirborn(ECreatureState  lastState)
    {
        Vector3 originalPosition = Owner.SkeletonAnim.transform.localPosition;
        Vector3 upPosition = originalPosition + Vector3.up * _airborneDistance;

        float halfTickTime = EffectData.TickTime * 0.5f;

        // 위로 올라갈 때
        for (float t = 0; t < halfTickTime; t += Time.deltaTime)
        {
            float normalizedTime = t / halfTickTime;
            Owner.SkeletonAnim.transform.localPosition = Vector3.Lerp(originalPosition, upPosition, normalizedTime);
            yield return null;
        }

        // 잠시 대기
        // yield return new WaitForSeconds(_data.TickTime * 0.04f);

        // 아래로 내려갈 때
        for (float t = 0; t < halfTickTime; t += Time.deltaTime)
        {
            float normalizedTime = t / halfTickTime;
            Owner.SkeletonAnim.transform.localPosition = Vector3.Lerp(upPosition, originalPosition, normalizedTime);
            yield return null;
        }

        Owner.SkeletonAnim.transform.localPosition = originalPosition;

        if (Owner.CreatureState == ECreatureState.OnDamaged)
            Owner.CreatureState = ECreatureState.Idle;

        ClearEffect(EEffectClearType.EndOfCC);
    }
    #endregion

    #region Knockback
    private Coroutine _knockbackCoroutine;

    IEnumerator DoKnockback()
    {
        const float knockbackSpeed = 20;
        // 1. 넉백 방향 결정
        Vector3 dir = (Owner.Position - Source.Position).normalized;
        Vector3Int dest = Vector3Int.zero;
        bool isFindPos = false;
        // 2. 셀 이동
        for (float i = 3; i >= 0; i -= 0.5f)
        {
            dest = Managers.Map.World2Cell(Owner.Position + dir * i);
            if (Managers.Map.CanGo(Owner, dest))
            {
                isFindPos = true;
                break;
            }
        }

        if (isFindPos)
        {
            Owner.MoveSpeed += knockbackSpeed;
            Owner.MoveToCellPos(dest);

            yield return new WaitForSeconds(0.15f);
        }
        Owner.MoveSpeed -= knockbackSpeed;

        ClearEffect(EEffectClearType.EndOfCC);
        _knockbackCoroutine = null;
    }
    #endregion
}