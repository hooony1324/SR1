using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Spine;
using UnityEngine;
using static Define;
using Event = Spine.Event;

public abstract class SkillBase : BaseObject
{
    public Creature Owner { get; set; }
    
    #region Level
    int level = 0;
    public int Level
    {
        get { return level; }
        set { level = value; }
    }
    #endregion
    
    #region skillData

    public ESkillType SkillType { get; set; }
    [SerializeField]
    private Data.SkillData _skillData;
    public Data.SkillData SkillData 
    {
        get
        { 
            return _skillData;
        }
        set 
        { 
            _skillData = value;
        }
    }
    #endregion

    public float RemainCoolTime { get; set; }
    public float SkillAnimDuration = 0;
    public TrackEntry CastingTrackEntry;
    public TrackEntry SkillTrackEntry;

    public InteractionObject SkillTarget { get; set; }

    protected bool _activated = true;
    public bool Activated { get { return _activated; } }

    protected override bool Init()
    {
        base.Init();
        return true;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (Managers.Game == null)
            return;
        if (Owner.IsValid() == false)
            return;
        if (Owner.SkeletonAnim == null)
            return;
        if (Owner.SkeletonAnim.AnimationState == null)
            return;

        Owner.SkeletonAnim.AnimationState.Event -= OnOwnerAnimEventHandler;
        // Owner.SkeletonAnim.AnimationState.Complete -= OnAnimCompleteHandler;
    }
    
    public virtual void OnChangedSkillData() { }

    public virtual void DoSkill(Action callback = null)
    {
        RemainCoolTime = SkillData.CoolTime;
        SkillTarget = Owner.Target;
        //준비된스킬에서 해제
        if(Owner.Skills.ReadySkills.Contains(this))
            Owner.Skills.ReadySkills.Remove(this);

        //공속은 기본스킬에만 적용
        float timeScale = Owner.AttackSpeedRate;
        if (Owner.Skills.DefaultSkill == this)//평타인 경우
        {
            TrackEntry skill = Owner.PlayAnimation(0, SkillData.AnimName, false);
            skill.TimeScale = timeScale;
            SkillAnimDuration = skill.Animation.Duration;        
        }
        else
        {
            //캐스팅이 있는 경우 캐스팅 애니메이션 먼저 play
            if (string.IsNullOrEmpty(SkillData.CastingAnimname) == false)
            {
                CastingTrackEntry = Owner.PlayAnimation(0, SkillData.CastingAnimname, false);
                SkillTrackEntry = Owner.AddAnimation(0, SkillData.AnimName, false, 0);

                CastingTrackEntry.TimeScale = 1;
                SkillTrackEntry.TimeScale = 1;
                SkillAnimDuration = CastingTrackEntry.Animation.Duration + SkillTrackEntry.Animation.Duration - MIX_DURATION;
            }
            else
            {
                SkillTrackEntry = Owner.PlayAnimation(0, SkillData.AnimName, false);
                SkillTrackEntry.TimeScale = 1;
                SkillAnimDuration = SkillTrackEntry.Animation.Duration;
            }
        }

        StartCoroutine(CoCooldown());
    }

    public virtual void CancelSkill() { }

    public virtual void SetInfo(int skillId)
    {
        Owner = GetComponent<Creature>();
        Owner.SkeletonAnim.AnimationState.Event -= OnOwnerAnimEventHandler;
        Owner.SkeletonAnim.AnimationState.Event += OnOwnerAnimEventHandler;
        // Owner.SkeletonAnim.AnimationState.Complete -= OnAnimCompleteHandler;
        // Owner.SkeletonAnim.AnimationState.Complete += OnAnimCompleteHandler;
        SkillData = Managers.Data.SkillDic[skillId];
        RemainCoolTime = SkillData.CoolTime - Owner.CooldownReduction;
        _activated = true;
    }

    protected virtual void GenerateProjectile(Vector3 spawnPos)
    {
        Projectile projectile = null;

        GameObject go = Managers.Object.SpawnProjectile(spawnPos, "ProjectilePrefab");
        projectile = go.GetOrAddComponent<Projectile>();
        projectile.SetInfo(Owner, this, MotionFinished);
    }

    protected virtual void OnOwnerAnimEventHandler(TrackEntry arg1, Event arg2)
    {
        // 다른스킬의 애니메이션 이벤트도 받기 때문에 자기꺼만 써야함
        if (arg1.Animation.Name != _skillData.AnimName)
            return;
        
        if(Owner.Skills.CurrentSkill.SkillData.TempleteId != SkillData.TempleteId)
            return;

        if (arg1.Animation.Name == _skillData.AnimName)
        {
            OnAttackEvent(arg1, arg2);
        }
    }

    protected virtual void OnAttackEvent(TrackEntry arg1, Event arg2) { }

    protected virtual void MotionFinished(Vector3 endPos, Vector3 targetPos, int skillTempleteId) { }
    
    protected virtual void GenerateAoE(Vector3 spawnPos, Vector3 targetPos)
    {
        AoEBase aoe = null;
        int id = SkillData.AoEId;
        EAoEType type = Managers.Data.AoEDic[id].Type;
        
        GameObject go = Managers.Object.SpawnGameObject(spawnPos, "AoEPrefab");

        switch (type)
        {
            case EAoEType.ConeShape:
                aoe = go.GetOrAddComponent<AoEBase>();
                break;
            case EAoEType.CircleShape:
                aoe = go.GetOrAddComponent<AoEBase>();
                break;
            case EAoEType.SingleTarget:
                aoe = go.GetOrAddComponent<AoEBase>();
                break;
            case EAoEType.CircleTrigger:
                aoe = go.GetOrAddComponent<CircleTriggerAoE>();
                break;
        }
        // aoe = go.GetOrAddComponent<>()AddComponent(componentType) as AoEBase;
        aoe.SetInfo(SkillData.AoEId, Owner, this, targetPos);
    }

    protected List<EffectBase> ApplyEffects(InteractionObject target)
    {
        if(SkillData.EffectIds != null)
            return target.Effects.GenerateEffects(SkillData.EffectIds, EEffectSpawnType.Skill, Owner);
        return null;
    }

    //For ComboSkill
    protected void ApplyEffect(InteractionObject target, int effectId)
    {
        var list = new List<int> { effectId };
        target.Effects.GenerateEffects(list, EEffectSpawnType.Skill, Owner);
    }

    private IEnumerator CoCooldown()
    {
        RemainCoolTime = _skillData.CoolTime;
        _activated = false;
        yield return new WaitForSeconds(_skillData.CoolTime);
        RemainCoolTime = 0;

        _activated = true;

        // 준비된 스킬에 추가
        if (Owner.Skills != null)
            Owner.Skills.ReadySkills.Add(this);
    }

    public void Clear()
    {
        StopAllCoroutines();
        RemainCoolTime = SkillData.CoolTime;
    }
}
