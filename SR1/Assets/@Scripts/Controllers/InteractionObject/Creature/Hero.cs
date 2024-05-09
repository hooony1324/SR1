using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Hero : Creature
{
    private Hero _myLeader;

    public Hero MyLeader
    {
        get => _myLeader;
        set
        {
            _myLeader = value;
            if (_myLeader)
            {
                // Indicator.gameObject.SetActive(false);
                IsLeader = false;
            }
            else
            {
                // Indicator.gameObject.SetActive(false);
                IsLeader = true;
                Managers.Game.Leader = this;
            }
        }
    }

    private HeroInfo _heroInfo { get; set; }
    public bool IsLeader = false;

    public bool NeedArange { get; set; }

    [SerializeField] private EHeroMoveState _heroMoveState = EHeroMoveState.None;

    public EHeroMoveState HeroMoveState
    {
        get => _heroMoveState;
        set
        {
            if (_heroMoveState == value)
                return;

            _heroMoveState = value;
            switch (value)
            {
                case EHeroMoveState.CollectEnv:
                    NeedArange = true;
                    break;
                case EHeroMoveState.TargetMonster:
                    NeedArange = true;
                    break;
                case EHeroMoveState.ForceMove:
                    Target = null;
                    NeedArange = true;
                    break;
            }
        }
    }

    private Transform HeroCampDest
    {
        get
        {
            HeroCamp heroCamp = Managers.Object.HeroCamp;
            if (HeroMoveState == EHeroMoveState.ReturnToCamp)
                return heroCamp.Pivot;

            return heroCamp.Destination;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (Managers.Game == null)
            return;

        Managers.Game.OnBroadcastEvent -= HandleOnBroadcast;
        Managers.Game.OnJoystickStateChanged -= HandleOnJoystickStateChanged;
    }

    protected override bool Init()
    {
        base.Init();
        Managers.Game.OnJoystickStateChanged -= HandleOnJoystickStateChanged;
        Managers.Game.OnJoystickStateChanged += HandleOnJoystickStateChanged;
        return true;
    }

    public override void SetInfo(int templateId)
    {
        ObjectType = EObjectType.Hero;
        CreatureState = ECreatureState.Idle;
        _heroInfo = Managers.Hero.GetHeroInfo(templateId);
        Managers.Game.OnBroadcastEvent -= HandleOnBroadcast;
        Managers.Game.OnBroadcastEvent += HandleOnBroadcast;

        base.SetInfo(templateId);
    }

    protected override void InitCreatureStat()
    {
        base.InitCreatureStat();
    }

    protected override float CalculateFinalStat(float baseValue, ECalcStatType calcStatType)
    {
        // base.CalculateStat();

        float finalValue = baseValue;
        finalValue += Effects.GetStatModifier(calcStatType, EStatModType.Add)
                      + Managers.Inventory.GetStatModifier(calcStatType, EStatModType.Add)
                      + Managers.Game.GetTrainingStatModifier(calcStatType, EStatModType.Add);

        finalValue *= 1 + Effects.GetStatModifier(calcStatType, EStatModType.PercentAdd)
                        + Managers.Inventory.GetStatModifier(calcStatType, EStatModType.PercentAdd)
                        + Managers.Game.GetTrainingStatModifier(calcStatType, EStatModType.PercentAdd);

        finalValue *= 1 + Effects.GetStatModifier(calcStatType, EStatModType.PercentMult);
        //장비는 퍼센트 멀티 없음

        return finalValue;
    }

    protected override void SetSkill()
    {
        //공통스킬
        Skills.AddSkill(CreatureData.DefaultSkillId, ESkillSlot.Default);
        Skills.AddSkill(CreatureData.EnvSkillId, ESkillSlot.Env);

        Skills.AddSkill(_heroInfo.ASkillDataId, ESkillSlot.A);
        Skills.AddSkill(_heroInfo.BSkillDataId, ESkillSlot.B);
    }

    protected void UpdateSkillSlot()
    {
        Skills.UpdateSkill(_heroInfo.ASkillDataId, ESkillSlot.A);
        Skills.UpdateSkill(_heroInfo.BSkillDataId, ESkillSlot.B);
    }

    protected override void UpdateAnimation()
    {
        base.UpdateAnimation();
        switch (CreatureState)
        {
            case ECreatureState.Idle:
                HeroMoveState = EHeroMoveState.None;
                break;
            case ECreatureState.Skill:
                HeroMoveState = EHeroMoveState.None;
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

    protected override void OnDead()
    {
        base.OnDead();
        Managers.Game.BroadcastEvent(EBroadcastEventType.HeroDead, value: CreatureData.TemplateId);
        StartCoroutine(CoRebirthTimer());
    }

    #region AI

    public float StopDistance { get; private set; } = 2.3f;

    protected override void UpdateIdle()
    {
        Skills.CurrentSkill = Skills.GetReadySkill();

        // 0. 이동 상태라면 강제 변경
        if (HeroMoveState == EHeroMoveState.ForceMove)
        {
            Target = null;
            CreatureState = ECreatureState.Move;
            return;
        }

        //NPC로 이동중일때
        if (Managers.Object.HeroCamp.CampState == ECampState.MoveToTarget)
        {
            CreatureState = ECreatureState.Move;
            HeroMoveState = EHeroMoveState.ReturnToCamp;
        }

        if (Target.IsValid())
        {
            if (Target.ObjectType == EObjectType.Monster)
            {
                CreatureState = ECreatureState.Move;
                HeroMoveState = EHeroMoveState.TargetMonster;
                return;
            }

            if (Target.ObjectType == EObjectType.Env)
            {
                CreatureState = ECreatureState.Move;
                HeroMoveState = EHeroMoveState.CollectEnv;
                return;
            }
        }
        else
        {
            Target = null;
        }

        // 3. Camp 주변으로 모이기
        if (NeedArange)
        {
            CreatureState = ECreatureState.Move;
            HeroMoveState = EHeroMoveState.ReturnToCamp;
            return;
        }
    }

    protected override void UpdateMove()
    {
        // 너무 멀어서 강제 조정.
        if (HeroMoveState == EHeroMoveState.ForcePath)
        {
            MoveByForcePath();
            return;
        }

        // if (CheckHeroCampDistanceAndForcePath())
        //     return;

        // 0. 누르고 있다면, 강제 이동
        if (HeroMoveState == EHeroMoveState.ForceMove)
        {
            EFindPathResult result = FindPathAndMoveToCellPos(HeroCampDest.position, HERO_DEFAULT_MOVE_DEPTH);
            return;
        }

        // 1. 주변 몬스터 서치
        if (HeroMoveState == EHeroMoveState.TargetMonster)
        {
            // 몬스터 죽었으면 포기.
            if (Target.IsValid() == false)
            {
                HeroMoveState = EHeroMoveState.None;
                CreatureState = ECreatureState.Move;
                return;
            }

            ChaseOrAttackTarget(GetAttackDistanceSqr(), SCAN_RANGE);
            return;
        }

        // 2. 주변 Env 채굴
        if (HeroMoveState == EHeroMoveState.CollectEnv)
        {
            // Env 이미 채집했으면 포기.
            if (Target.IsValid() == false)
            {
                HeroMoveState = EHeroMoveState.None;
                CreatureState = ECreatureState.Move;
                return;
            }

            ChaseOrAttackTarget(GetAttackDistanceSqr(), SCAN_RANGE);
            return;
        }

        // 3. Camp 주변으로 모이기
        if (HeroMoveState == EHeroMoveState.ReturnToCamp)
        {
            Vector3 destPos = HeroCampDest.position;
            EFindPathResult res = FindPathAndMoveToCellPos(destPos, HERO_DEFAULT_MOVE_DEPTH);

            if (res == EFindPathResult.SamePosition)
            {
                HeroMoveState = EHeroMoveState.None;
                // Dragon 스르륵 거리는거 방지하기위해 막아둠 임시
                // CreatureState = ECreatureState.Idle;
                NeedArange = false;
                return;
            }

            if (res == EFindPathResult.Success)
                return;

            // 실패 사유 검사.
            BaseObject obj = Managers.Map.GetObject(destPos);
            if (obj.IsValid())
            {
                // 내가 그 자리를 차지하고 있다면
                if (obj == this)
                {
                    HeroMoveState = EHeroMoveState.None;
                    NeedArange = false;
                    return;
                }

                // 다른 영웅이 멈춰있다면.
                Hero hero = obj as Hero;
                if (hero != null && hero.CreatureState == ECreatureState.Idle)
                {
                    Vector3Int destCellPos = Managers.Map.World2Cell(destPos);
                    if ((destCellPos - CellPos).sqrMagnitude > 4)
                        return;

                    HeroMoveState = EHeroMoveState.None;
                    // Dragon 스르륵 거리는거 방지하기위해 막아둠 임시
                    // CreatureState = ECreatureState.Idle;
                    NeedArange = false;
                    return;
                }
            }
        }

        // 4. 기타 (누르다 뗐을 때)
        if (LerpCellPosCompleted)
            CreatureState = ECreatureState.Idle;
    }

    Queue<Vector3Int> _forcePath = new Queue<Vector3Int>();

    bool CheckHeroCampDistanceAndForcePath()
    {
        // 너무 멀어서 못 간다.
        Vector3 destPos = HeroCampDest.position;
        Vector3Int destCellPos = Managers.Map.World2Cell(destPos);
        if ((CellPos - destCellPos).magnitude <= SCAN_RANGE)
            return false;

        if (Managers.Map.CanGo(this, destCellPos, ignoreObjects: true) == false)
            return false;

        List<Vector3Int> path = Managers.Map.FindPath(this, CellPos, destCellPos, 100);
        if (path.Count < 2)
            return false;

        HeroMoveState = EHeroMoveState.ForcePath;

        _forcePath.Clear();
        foreach (var p in path)
        {
            _forcePath.Enqueue(p);
        }

        _forcePath.Dequeue();

        return true;
    }

    void MoveByForcePath()
    {
        if (_forcePath.Count == 0)
        {
            HeroMoveState = EHeroMoveState.None;
            return;
        }

        Vector3Int cellPos = _forcePath.Peek();

        if (MoveToCellPos(cellPos, 2))
        {
            _forcePath.Dequeue();
            return;
        }

        // 실패 사유가 영웅이라면.
        Hero hero = Managers.Map.GetObject(cellPos) as Hero;
        if (hero != null && hero.CreatureState == ECreatureState.Idle)
        {
            HeroMoveState = EHeroMoveState.None;
            return;
        }
    }

    protected override void UpdateSkill()
    {
        base.UpdateSkill();
        if (HeroMoveState == EHeroMoveState.ForceMove)
        {
            CreatureState = ECreatureState.Move;
            return;
        }

        if (Target.IsValid() == false)
        {
            CreatureState = ECreatureState.Idle;
            return;
        }
    }

    protected override void UpdateDead()
    {
        base.OnDead();
    }

    #endregion

    private IEnumerator CoRebirthTimer()
    {
        float remains = 5;
        float tickTimer = 0f;
        while (true)
        {
            tickTimer += Time.deltaTime;
            float ratio = tickTimer / remains;
            _hpBar.Refresh(ratio);

            if (tickTimer >= remains)
            {
                Rebirth();
                break;
            }

            yield return null;
        }
    }

    private void HandleOnBroadcast(EBroadcastEventType eventType, ECurrencyType currencyType, int value)
    {
        switch (eventType)
        {
            case EBroadcastEventType.HeroLevelUp:
                CalculateStat();
                UpdateSkillSlot();
                break;
            case EBroadcastEventType.DungeonClear:
                break;
            case EBroadcastEventType.ChangeInventory:
            case EBroadcastEventType.ChangeTeam:
                CalculateStat();
                break;
            case EBroadcastEventType.ChangeCampState:
                switch ((ECampState)value)
                {
                    case ECampState.Idle:
                        NeedArange = false;
                        break;
                    case ECampState.CampMode:
                    case ECampState.Move:

                        break;
                    case ECampState.MoveToTarget:
                        // if(Vector3.Distance(Position, Managers.Object.HeroCamp.Position) >= 2)
                        NeedArange = true;
                        break;
                }

                break;
        }
    }

    private void HandleOnJoystickStateChanged(Define.EJoystickState joystickState)
    {
        switch (joystickState)
        {
            case Define.EJoystickState.PointerDown:
                HeroMoveState = EHeroMoveState.ForceMove;
                break;
            case Define.EJoystickState.Drag:
                HeroMoveState = EHeroMoveState.ForceMove;
                break;
            case Define.EJoystickState.PointerUp:
                HeroMoveState = EHeroMoveState.None;
                break;
            default:
                break;
        }
    }

    public void Rebirth()
    {
        Hp = MaxHp;
        _hurtFlash.Init();
        Skills.Clear();
        CreatureState = ECreatureState.Idle;
        _hpBar.Refresh(1);

        // 캠프와의 거리가 너무 멀면 텔레포트
    }
}