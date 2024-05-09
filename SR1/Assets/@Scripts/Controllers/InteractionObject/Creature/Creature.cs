using System;
using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;
using static Define;

public class Creature : InteractionObject
{
    #region Stat Value

    public float MaxHpBase { get; set; }
    public float AtkBase { get; set; }
    public float CriRateBase { get; set; }
    public float CriDamageBase { get; set; }
    public float MissBase { get; set; }
    public float ReduceDamageRateBase { get; set; }
    public float ReduceDamageBase { get; set; }
    public float LifeStealRateBase { get; set; }
    public float ThornsDamageRateBase { get; set; }
    public float MoveSpeedBase { get; set; }
    public float AttackSpeedRateBase { get; set; }
    public float CooldownReductionBase { get; set; }
    [field: SerializeField] public float MaxHp { get; set; }
    [field: SerializeField] public float Atk { get; set; }
    [field: SerializeField] public float CriRate { get; set; }
    [field: SerializeField] public float CriDamage { get; set; }
    [field: SerializeField] public float MissChance { get; set; }
    [field: SerializeField] public float ReduceDamageRate { get; set; }
    [field: SerializeField] public float ReduceDamage { get; set; }
    [field: SerializeField] public float LifeStealRate { get; set; }
    [field: SerializeField] public float ThornsDamageRate { get; set; } //쏜즈
    [field: SerializeField] public float MoveSpeed { get; set; }
    [field: SerializeField] public float AttackSpeedRate { get; set; }
    [field: SerializeField] public float CooldownReduction { get; set; }
    [field: SerializeField] private bool _dirty = false;

    #endregion

    #region Value

    protected UI_HPBar _hpBar;
    [SerializeField] private float _hp;

    public float Hp
    {
        get => _hp;
        protected set => _hp = value;
    }

    [field: SerializeField] public Data.CreatureData CreatureData { get; protected set; }
    public SkillComponent Skills { get; set; }
    
    #endregion

    #region AI Value

    [Header("AI")] [SerializeField] private InteractionObject _target;

    public InteractionObject Target
    {
        get => _target;
        set { _target = value; }
    }

    [SerializeField] protected ECreatureState _creatureState = ECreatureState.Idle;

    public virtual ECreatureState CreatureState
    {
        get => _creatureState;
        set
        {
            if (_creatureState != value)
            {
                _creatureState = value;
                CancelWait();

                UpdateAnimation();
            }
        }
    }

    protected Vector3 InitPos { get; set; }

    #endregion

    protected override bool Init()
    {
        base.Init();
        Skills = gameObject.GetOrAddComponent<SkillComponent>();

        //Npc 상호작용을 위한 버튼
        GameObject obj = Managers.Resource.Instantiate("UI_HPBar", gameObject.transform);

        obj.transform.localPosition = new Vector3(0f, 3f);
        _hpBar = obj.GetComponent<UI_HPBar>();

        return true;
    }

    public virtual void SetInfo(int templateId)
    {
        InitPos = transform.position;
        TemplateId = templateId;

        if (ObjectType == EObjectType.Hero)
            CreatureData = Managers.Data.HeroDic[templateId];
        else
            CreatureData = Managers.Data.MonsterDic[templateId];

        CreatureState = ECreatureState.Idle;

        #region Collider

        CurrentCollider.radius = CreatureData.ColliderRadius;
        CurrentCollider.offset = new Vector2(0, CreatureData.ColliderOffsetY);

        #endregion

        #region Spine Animation

        SetSpineAnimation(CreatureData.SkeletonDataID, SortingLayers.HERO, "SkeletonAnimation");

        SetFireSocket();

        _hpBar.SetInfo(this);

        #endregion

        InitCreatureStat();
        CalculateStat();
        SetSkill();

        #region AI

        CreatureState = ECreatureState.Idle;
        UpdateAnimation();
        StartCoroutine(CoUpdateAI());

        #endregion

        _hpBar.SetInfo(this);
        // stat
        StartCoroutine(CoLerpToCellPos());
        _hurtFlash.Init();

    }

    protected virtual void InitCreatureStat()
    {
        Hp = CreatureData.MaxHp;

        //BaseValue
        MaxHp = MaxHpBase = CreatureData.MaxHp;
        Atk = AtkBase = CreatureData.Atk;
        CriRate = CriRateBase = CreatureData.CriRate;
        CriDamage = CriDamageBase = CreatureData.CriDamage;
        MissChance = MissBase = 0.0f;
        ReduceDamageRate = ReduceDamageRateBase = 0;
        ReduceDamage = ReduceDamageBase = 0;
        LifeStealRate = LifeStealRateBase = 0;
        ThornsDamageRate = ThornsDamageRateBase = 0;
        MoveSpeed = MoveSpeedBase = CreatureData.MoveSpeed;
        AttackSpeedRate = AttackSpeedRateBase = 1;
        CooldownReductionBase = 0;
    }

    public virtual void CalculateStat()
    {
        float prevMaxHp = MaxHp;
        MaxHp = CalculateFinalStat(MaxHpBase, ECalcStatType.MaxHp);
        Atk = CalculateFinalStat(AtkBase, ECalcStatType.Atk);
        if (ObjectType == EObjectType.Hero)
        {
            HeroInfo heroInfo = Managers.Hero.GetHeroInfo(TemplateId);
            if (heroInfo != null)
            {
                MaxHp += (heroInfo.Level - 1) * heroInfo.HeroData.UpMaxHpBonus;
                Atk += (heroInfo.Level - 1) * heroInfo.HeroData.AtkBonus;
            }
        }

        CriRate = CalculateFinalStat(CriRateBase, ECalcStatType.Critical);
        CriDamage = CalculateFinalStat(CriDamageBase, ECalcStatType.CriticalDamage);
        MissChance = CalculateFinalStat(MissBase, ECalcStatType.MissChance);
        ReduceDamageRate = CalculateFinalStat(ReduceDamageRateBase, ECalcStatType.ReduceDamageRate);
        ReduceDamage = CalculateFinalStat(ReduceDamageBase, ECalcStatType.ReduceDamage);
        LifeStealRate = CalculateFinalStat(LifeStealRateBase, ECalcStatType.LifeStealRate);
        ThornsDamageRate = CalculateFinalStat(ThornsDamageRateBase, ECalcStatType.ThornsDamageRate);
        MoveSpeed = CalculateFinalStat(MoveSpeedBase, ECalcStatType.MoveSpeed);
        AttackSpeedRate = CalculateFinalStat(AttackSpeedRateBase, ECalcStatType.AttackSpeedRate);
        CooldownReduction = CalculateFinalStat(CooldownReduction, ECalcStatType.CooldownReduction);

        // 최대 HP가 변경되면 현재 HP를 같은 비율로 조정
        if (prevMaxHp != MaxHp)
        {
            float hpRatio = Hp / prevMaxHp;
            Hp = MaxHp * hpRatio;
            Hp = Mathf.Clamp(Hp, 0, MaxHp);
        }

        float ratio = Hp / MaxHp;
        _hpBar.Refresh(ratio);
    }

    protected virtual float CalculateFinalStat(float baseValue, ECalcStatType calcStatType)
    {
        return 0;
    }

    protected virtual void SetSkill()
    {
        Skills.UpdateSkill(CreatureData.DefaultSkillId, ESkillSlot.Default);
        Skills.UpdateSkill(CreatureData.EnvSkillId, ESkillSlot.Env);
        Skills.UpdateSkill(CreatureData.SkillAId, ESkillSlot.A);
        Skills.UpdateSkill(CreatureData.SkillBId, ESkillSlot.B);
    }

    protected virtual void UpdateAnimation()
    {
        switch (CreatureState)
        {
            case ECreatureState.Idle:
                PlayAnimation(0, AnimName.IDLE, true);
                break;
            case ECreatureState.Cooltime:
                PlayAnimation(0, AnimName.IDLE, true);
                break;
            case ECreatureState.Skill:
                break;
            case ECreatureState.Move:
                PlayAnimation(0, AnimName.MOVE, true);
                break;
            case ECreatureState.OnDamaged:
                PlayAnimation(0, AnimName.IDLE, true);
                if (Skills.CurrentSkill != null)
                    Skills.CurrentSkill.CancelSkill();
                break;
            case ECreatureState.Dead:
                PlayAnimation(0, AnimName.DEAD, false);
                OnDead();
                break;
            default:
                break;
        }
    }

    #region AI

    public float UpdateAITick { get; protected set; } = 0.0f;

    public float GetAttackDistanceSqr()
    {
        float baseValue = 0f;

        baseValue = Skills.CurrentSkill.SkillData.SkillRange + (ExtraCells * 1.5f);

        if (baseValue < 2.5f) // 근접공격이면
        {
            baseValue += (Target.ExtraCells * 1.5f); // 덩치 큰 타겟은 근접 범위 증가
        }

        if (Target.ObjectType == EObjectType.Env)
        {
            baseValue = Skills.EnvSkill.SkillData.SkillRange;
        }

        return baseValue * baseValue;
    }

    protected IEnumerator CoUpdateAI()
    {
        while (true)
        {
            switch (CreatureState)
            {
                case ECreatureState.Idle:
                    UpdateIdle();
                    UpdateAITick = 0.1f;
                    break;
                case ECreatureState.Cooltime:
                    UpdateCooltime();
                    UpdateAITick = 0.1f;
                    break;
                case ECreatureState.Move:
                    UpdateAITick = 0.0f;
                    UpdateMove();
                    break;
                case ECreatureState.Skill:
                    UpdateAITick = 0.1f;
                    UpdateSkill();
                    break;
                case ECreatureState.Dead:
                    UpdateAITick = 1f;
                    UpdateDead();
                    break;
            }

            if (UpdateAITick > 0)
                yield return new WaitForSeconds(UpdateAITick);
            else
                yield return null;
        }
    }

    protected virtual void UpdateIdle()
    {
        // 1. Target을 찾으면 (ObjectManager.PerformMatching으로 찾아줌)
        if (Target.IsValid())
            CreatureState = ECreatureState.Move;

        Skills.CurrentSkill = Skills.GetReadySkill();
    }

    protected virtual void UpdateCooltime()
    {
        Skills.CurrentSkill = Skills.GetReadySkill();
        if (Skills.CurrentSkill != null)
            CreatureState = ECreatureState.Skill;
    }

    protected virtual void UpdateMove()
    {
    }

    protected virtual void UpdateSkill()
    {
        if (_coWait != null)
        {
            return;
        }

        if (Target.IsValid() == false || Target.ObjectType == EObjectType.Camp)
        {
            CreatureState = ECreatureState.Idle;
            return;
        }

        // atkrange보다 길면
        Vector3 dir = (Target.CenterPosition - CenterPosition);
        float distToTargetSqr = dir.sqrMagnitude;
        // Debug.Log();
        float attackDistanceSqr = GetAttackDistanceSqr();
        if (distToTargetSqr > attackDistanceSqr)
        {
            CreatureState = ECreatureState.Idle;
            return;
        }

        float animDuration = 1;

        Skills.CurrentSkill = Skills.GetReadySkill();
        if (Skills.CurrentSkill.Activated == false)
        {
            CreatureState = ECreatureState.Cooltime;
            return;
        }

        LookAtTarget(Target);

        Skills.CurrentSkill.DoSkill();
        animDuration = Skills.CurrentSkill.SkillAnimDuration;

        float delay;
        // AttackSpeedRate가 기본값(1)과 다를 경우, 애니메이션 재생 시간을 조정한다.
        if (AttackSpeedRate != 1 && Skills.CurrentSkill == Skills.DefaultSkill)
            delay = animDuration / AttackSpeedRate;
        else
            delay = animDuration;

        StartWait(delay);
    }

    protected virtual void UpdateDead()
    {
    }

    #endregion

    #region Wait

    protected Coroutine _coWait;

    protected void StartWait(float seconds)
    {
        CancelWait();
        _coWait = StartCoroutine(CoWait(seconds));
    }

    IEnumerator CoWait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _coWait = null;
    }

    protected void CancelWait()
    {
        if (_coWait != null)
            StopCoroutine(_coWait);
        _coWait = null;
    }

    #endregion

    #region Battle

    protected virtual void OnDead()
    {
        Effects.Clear();
        CancelWait();
    }

    public override void OnDamage(InteractionObject Attacker, float damage)
    {
        base.OnDamage(Attacker, damage);

        if (Attacker.IsValid() == false)
            return;

        // 공격자를 Creature 타입으로 캐스팅
        Creature creatureAttacker = Attacker as Creature;
        if (creatureAttacker == null)
            return;

        //damageResult 
        bool isCritical = Util.CheckProbability(creatureAttacker.CriRate);
        EDamageResult damageResult = EDamageResult.Hit;
        if (damage < 0)
        {
            if (isCritical)
            {
                damageResult = EDamageResult.CriticalHeal;
                damage *= creatureAttacker.CriDamage;
            }
            else
            {
                damageResult = EDamageResult.Heal;
            }
        }
        else if (Util.CheckProbability(MissChance))
        {
            damageResult = EDamageResult.Miss;
            Managers.Object.ShowDamageFont(CenterPosition, damage, transform, damageResult);
            return;
        }
        else if (isCritical)
        {
            damage *= creatureAttacker.CriDamage;
            damageResult = EDamageResult.CriticalHit;
        }

        //데미지 감소 설정
        if (damageResult != EDamageResult.Heal)
        {
            damage -= ReduceDamage;
            damage -= damage * ReduceDamageRate;
        }


        Hp = Mathf.Clamp(Hp - damage, 0, MaxHp);
        Managers.Object.ShowDamageFont(CenterPosition, damage, transform, damageResult);

        float ratio = Hp / MaxHp;
        _hpBar.Refresh(ratio);

        //사망처리
        if (Hp == 0)
        {
            CreatureState = ECreatureState.Dead;
            return;
        }

        // 흡혈 
        {
            float value = (damage * creatureAttacker.LifeStealRate) * -1;
            if (value != 0)
                Attacker.OnDamage(this, value);
        }

        //가시 (반사데미지)
        {
            float value = (damage * ThornsDamageRate);
            if (value != 0)
                Attacker.OnDamage(this, value);
        }
    }

    protected void ChaseOrAttackTarget(float attackRangeSqr, float chaseRange)
    {
        Vector3 dir = (Target.CenterPosition - CenterPosition);
        float distToTargetSqr = dir.sqrMagnitude;
        float attackDistanceSqr = attackRangeSqr;

        if (distToTargetSqr <= attackDistanceSqr)
        {
            // 공격 범위 이내로 들어왔다면 공격.
            CreatureState = ECreatureState.Skill;
            return;
        }
        else
        {
            int maxDepth = 5;
            if (Managers.Object.HeroCamp.CampState == ECampState.CampMode)
                maxDepth = 50;
            
            // 공격 범위 밖이라면 추적.
            FindPathAndMoveToCellPos(Target.transform.position, maxDepth);
        }
    }

    #endregion

    #region Map

    public EFindPathResult FindPathAndMoveToCellPos(Vector3 destWorldPos, int maxDepth, bool forceMoveCloser = false)
    {
        Vector3Int destCellPos = Managers.Map.World2Cell(destWorldPos);
        return FindPathAndMoveToCellPos(destCellPos, maxDepth, forceMoveCloser);
    }

    private List<Vector3Int> _debugPath = new List<Vector3Int>();

    public EFindPathResult FindPathAndMoveToCellPos(Vector3Int destCellPos, int maxDepth, bool forceMoveCloser = false)
    {
        if (CellPos == destCellPos)
        {
            //Dragon 
            return EFindPathResult.SamePosition;
        }

        if (LerpCellPosCompleted == false)
            return EFindPathResult.Fail_LerpCell;

        // A*
        List<Vector3Int> path = Managers.Map.FindPath(this, CellPos, destCellPos, maxDepth, ExtraCells);
        if (path.Count < 2)
            return EFindPathResult.Fail_NoPath;

        _debugPath = path; // 여기서 경로를 _debugPath에 저장

        if (forceMoveCloser)
        {
            Vector3Int diff1 = CellPos - destCellPos;
            Vector3Int diff2 = path[1] - destCellPos;
            if (diff1.sqrMagnitude <= diff2.sqrMagnitude)
                return EFindPathResult.Fail_NoPath;
        }

        Vector3Int dirCellPos = path[1] - CellPos;
        Vector3Int nextPos = CellPos + dirCellPos;

        if (Managers.Map.MoveTo(this, nextPos) == false)
            return EFindPathResult.Fail_MoveTo;

        return EFindPathResult.Success;
    }

    public bool MoveToCellPos(Vector3Int destCellPos, int maxDepth = 2, bool forceMoveCloser = false)
    {
        //이동중에 CC기 맞은경우는 무조건 이동
        if (LerpCellPosCompleted == false && CreatureState != ECreatureState.OnDamaged)
            return false;

        return Managers.Map.MoveTo(this, destCellPos);
    }

    protected IEnumerator CoLerpToCellPos()
    {
        while (true)
        {
            if (ObjectType == EObjectType.Hero)
            {
                Hero hero = this as Hero;
                //1. drag 일때만 ratio 조절
                float ratio = 1;
                if (Managers.Game.JoystickState == EJoystickState.Drag)
                    ratio = CalcRatio(ref hero);

                LerpToCellPos(MoveSpeed * ratio, CreatureState != ECreatureState.OnDamaged);
            }
            else
            {
                // Debug.Log(MoveSpeed);
                LerpToCellPos(MoveSpeed, CreatureState != ECreatureState.OnDamaged);
            }

            yield return null;
        }
    }

    private float CalcRatio(ref Hero hero)
    {
        float div = 5f;

        Vector3 campPos = Managers.Object.HeroCamp.Destination.transform.position;
        Vector3Int campCellPos = Managers.Map.World2Cell(campPos);

        int dist = Mathf.Abs(CellPos.x - campCellPos.x) + Mathf.Abs(CellPos.y - campCellPos.y);

        float ratio = Math.Clamp(dist / div, 1, 3f);

        return ratio;
    }
}


#endregion

