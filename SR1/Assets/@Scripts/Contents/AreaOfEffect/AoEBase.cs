using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using Spine;
using UnityEngine;
using static Define;
using Event = UnityEngine.Event;

public class AoEBase : BaseObject
{
    [SerializeField] protected List<EffectBase> _activeEffects = new List<EffectBase>();

    public Creature Owner;
    protected HashSet<Creature> _targets = new HashSet<Creature>();
    protected SkillBase _skillBase;
    [SerializeField] protected AoEData _aoEData;
    protected Vector3 _skillDir;
    protected float _radius;

    private CircleCollider2D _collider;
    private EEffectSize _effectSize;

    protected override void OnDisable()
    {
        base.OnDisable();

        //1. clear target
        _targets.Clear();

        //2. clear Effect
        foreach (var effect in _activeEffects)
        {
            if (effect.IsValid())
                effect.Owner.Effects.RemoveEffect(effect);
        }

        _activeEffects.Clear();
        enabled = false;
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        _collider = GetComponent<CircleCollider2D>();
        _collider.isTrigger = true;
        return true;
    }

    public virtual void SetInfo(int templateId, InteractionObject owner, SkillBase skill, Vector3 endPos)
    {
        enabled = true;
        transform.localEulerAngles = Vector3.zero;
        _aoEData = Managers.Data.AoEDic[templateId];
        Owner = owner as Creature;
        _skillBase = skill;
        _effectSize = skill.SkillData.EffectSize;
        _radius = Util.GetEffectRadius(_effectSize);
        _collider.radius = _radius;
        _skillDir = (endPos - Owner.Position).normalized;

        if (string.IsNullOrEmpty(_aoEData.SkeletonDataID) == false)
        {
            //스파인
            SetSpineAnimation(_aoEData.SkeletonDataID, SortingLayers.SKILL_EFFECT, gameObject.name);
            PlayAnimation(0, _aoEData.AnimName, false);
        }
        else
        {
            // 파티클
            GameObject go = Managers.Object.SpawnGameObject(transform.position, _aoEData.PrefabName);
            go.transform.SetParent(transform, false);
            go.transform.localPosition = Vector3.zero;

            ParticleController pc = go.GetComponent<ParticleController>();
            if (pc != null)
            {
                pc.SetInfo(OnParticleStopped);
            }
    
            ApplyEffectsInRange();
        }
        
        if (_aoEData.Type == EAoEType.ConeShape)
            transform.eulerAngles = GetLookAtRotation(_skillDir);
    }

    protected void ApplyEffectsInRange()
    {
        // 아군에게 버프 적용
        if (_aoEData.AllyEffects.Count > 0)
        {
            var allies = FindTargets(true);
            ApplyEffectsToTargets(allies, _aoEData.AllyEffects, false);
        }

        // 적군에게 버프 적용
        if (_aoEData.EnemyEffects.Count > 0)
        {
            var enemies = FindTargets(false);
            ApplyEffectsToTargets(enemies, _aoEData.EnemyEffects, true);
        }
    }

    private List<InteractionObject> FindTargets(bool isAlly)
    {
        switch (_aoEData.FindRangeType)
        {
            case EFindRangeType.None:
            case EFindRangeType.Circle:
            case EFindRangeType.Single:
            default:
                return Managers.Object.FindCircleRangeTargets(Position, _radius, Owner.ObjectType, isAlly);
            case EFindRangeType.Cone:
                return Managers.Object.FindConeRangeTargets(Owner, _skillDir, _radius, 90, isAlly);
            case EFindRangeType.Rectangle:
                float width = Owner.CurrentCollider.radius * 2f;
                return Managers.Object.FindRectRangeTargets(Owner.Position, (float)_skillBase.SkillData.SkillRange,
                    width, Owner.CurrentCollider.radius, _skillDir, Owner.ObjectType, isAlly);
            // case EFindRangeType.MinHp:
            //     //TODO 체력이 낮은 순으로
            //     List<InteractionObject> targets =  Managers.Object.FindCircleRangeTargets(Position, _radius, Owner.ObjectType, isAlly);
            //     List<Creature> creatures = targets.OfType<Creature>().ToList();
            //     Creature minHpCreature = creatures.OrderBy(c => c.Hp).FirstOrDefault();
            //     return minHpCreature != null ? new List<InteractionObject> { minHpCreature } : new List<InteractionObject>();
        }
    }

    private void ApplyEffectsToTargets(List<InteractionObject> targets, List<int> effects, bool applyDamage)
    {
        foreach (var target in targets)
        {
            Creature t = target as Creature;
            if (t.IsValid())
            {
                t.Effects.GenerateEffects(effects, EEffectSpawnType.Skill, Owner);
            }
        }
    }

    protected IEnumerator CoReserveDestroy()
    {
        yield return new WaitForSeconds(_aoEData.Duration);
        DestroyAoE();
    }

    private void OnParticleStopped()
    {
        DestroyAoE();
    }
    
    #region Spine Event

    protected override void OnAnimEventHandler(TrackEntry arg1, Spine.Event arg2)
    {
        base.OnAnimEventHandler(arg1, arg2);
        ApplyEffectsInRange();
    }

    protected override void OnAnimCompleteHandler(TrackEntry arg1)
    {
        base.OnAnimCompleteHandler(arg1);
        DestroyAoE();
    }

    #endregion


    protected void DestroyAoE()
    {
        Managers.Object.DespawnGameObject(this);
    }
}