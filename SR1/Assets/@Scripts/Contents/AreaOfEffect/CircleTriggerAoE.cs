using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

// TODO  장판형 AOE
public class CircleTriggerAoE : AoEBase
{
    protected override void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines();
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }

    public override void SetInfo(int templateId, InteractionObject owner, SkillBase skill, Vector3 endPos)
    {
        base.SetInfo(templateId, owner, skill, endPos);

        StartCoroutine(CoReserveDestroy());
        StartCoroutine(DetectTargetsPeriodically());
    }

    private IEnumerator DetectTargetsPeriodically()
    {
        while (true)
        {
            DetectTargets();
            yield return new WaitForSeconds(1f);
        }
    }

    private void DetectTargets()
    {
        List<InteractionObject> detectedCreatures = new List<InteractionObject>();
        List<InteractionObject> rangeTargets = Managers.Object.FindCircleRangeTargets(Position, _radius, Owner.ObjectType);

        foreach (var target in rangeTargets)
        {
            Creature t = target as Creature;
            
            detectedCreatures.Add(target);
            if (t.IsValid() && !_targets.Contains(target))
            {
                _targets.Add(t);
                _activeEffects.AddRange(target.Effects.GenerateEffects(_aoEData.EnemyEffects, EEffectSpawnType.External, Owner));
            }
        }

        // 이전에 탐지되었으나 이제 범위 밖에 있는 Creature 제거
        foreach (var target in _targets.ToArray())
        {
            if (target.IsValid() && !detectedCreatures.Contains(target))
            {
                // 범위 밖으로 나간 Creature 처리
                _targets.Remove(target);
                RemoveEffect(target);
            }
        }
    }

    private void RemoveEffect(Creature target)
    {
        List<EffectBase> effectsToRemove = new List<EffectBase>();

        foreach (var effect in _activeEffects)
        {
            if (target.Effects.ActiveEffects.Contains(effect))
            {
                effect.ClearEffect(EEffectClearType.TriggerOutAoE);
                effectsToRemove.Add(effect);
            }
        }

        foreach (var effect in effectsToRemove)
        {
            _activeEffects.Remove(effect);
        }
    }
}