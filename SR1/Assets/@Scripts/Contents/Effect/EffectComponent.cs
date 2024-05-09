using System;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class EffectComponent : MonoBehaviour
{
    public List<EffectBase> ActiveEffects = new List<EffectBase>();
    private InteractionObject _owner;

    public void SetInfo(InteractionObject Owner)
    {
        _owner = Owner;
    }

    public List<EffectBase> GenerateEffects(List<int> effectIds, EEffectSpawnType spawnType, InteractionObject source)
    {
        List<EffectBase> generatedEffects = new List<EffectBase>();
        foreach (var id in effectIds)
        {
            EffectBase effect = null;

            EEffectType type = Managers.Data.EffectDic[id].EffectType;
            GameObject go = Managers.Object.SpawnGameObject(_owner.CenterPosition, "EffectPrefab");
            go.transform.SetParent(_owner.Effects.transform, false);
            go.transform.localPosition = Vector3.zero;
            switch (type)
            {
                case EEffectType.Instant:
                case EEffectType.Dot:
                    effect = go.GetOrAddComponent<DotBase>();
                    break;
                case EEffectType.Buff:
                case EEffectType.Debuff:
                case EEffectType.Infinite:
                    effect = go.GetOrAddComponent<BuffBase>();
                    break;
                case EEffectType.Knockback:
                case EEffectType.Airborne:
                case EEffectType.Freeze:
                case EEffectType.Stun:
                case EEffectType.Pull:
                    effect = go.GetOrAddComponent<CCBase>();
                    break;
            }

            effect.enabled = true;
            ActiveEffects.Add(effect);
            generatedEffects.Add(effect);

            effect.SetInfo(Managers.Data.EffectDic[id], _owner, source, spawnType);
            effect.ApplyEffect();
        }

        return generatedEffects;
    }

    public float GetStatModifier( ECalcStatType calcStatType, EStatModType type)
    {
        float result = 0;

        foreach (var modifier in ActiveEffects)
        {
            if(calcStatType != modifier.EffectData.calcStatType)
                continue;
            if (modifier.EffectType == EEffectType.Buff || 
                modifier.EffectType == EEffectType.Debuff || 
                modifier.EffectType == EEffectType.Infinite)
            {
                switch (type)
                {
                    case EStatModType.Add:
                        result += modifier.EffectData.Amount;
                        break;
                    case EStatModType.PercentAdd:
                        result += modifier.EffectData.PercentAdd;
                        break;
                    case EStatModType.PercentMult:
                        result += modifier.EffectData.PercentMult;
                        break;
                }
            }
        }

        return result;
    }

    public void RemoveEffect(EffectBase effect)
    {
        // effect.ClearEffect(EEffectClearType.Disable);
        ActiveEffects.Remove(effect);
        Managers.Object.DespawnGameObject(effect);
        effect.enabled = false;
    }

    public void ClearEffectsByCondition(Func<EffectBase, bool> condition)
    {
        List<EffectBase> effectsToRemove = new List<EffectBase>();

        foreach (var effect in ActiveEffects.ToArray())
        {
            if (condition(effect))
            {
                effect.ClearEffect(EEffectClearType.ClearSkill);
                effectsToRemove.Add(effect);
            }
        }

        foreach (var effect in effectsToRemove)
        {
            ActiveEffects.Remove(effect);
        }
    }
    
    public void Clear()
    {
        List<EffectBase> effectsToRemove = new List<EffectBase>();

        foreach (var buff in ActiveEffects.ToArray())
        {
            buff.ClearEffect(EEffectClearType.Disable);
            effectsToRemove.Add(buff);
        }
        
        foreach (var effect in effectsToRemove)
        {
            ActiveEffects.Remove(effect);
        }
    }
}