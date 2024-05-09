using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Hero_Cheat : Hero
{
    private Hero _myLeader;

    private HeroInfo _heroInfo { get; set; }

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
    }

    protected override bool Init()
    {
        base.Init();
        Debug.Log("init");
        ObjectType = Define.EObjectType.Hero;

        //event
        // Managers.Game.OnMoveDirChanged -= HandleOnMoveDirChanged;
        // Managers.Game.OnMoveDirChanged += HandleOnMoveDirChanged;
        Managers.Game.OnJoystickStateChanged -= HandleOnJoystickStateChanged;
        Managers.Game.OnJoystickStateChanged += HandleOnJoystickStateChanged;
        return true;
    }

    public override void SetInfo(int templateId)
    {
        _heroInfo = Managers.Hero.GetHeroInfo(templateId); 
        base.SetInfo(templateId);
    }

    protected override void InitCreatureStat()
    {
        base.InitCreatureStat();

    }

    protected override void SetSkill()
    {
        //공통스킬
        Skills.AddSkill(CreatureData.DefaultSkillId, ESkillSlot.Default);
        Skills.AddSkill(CreatureData.EnvSkillId, ESkillSlot.Env);

        Skills.AddSkill(_heroInfo.ASkillDataId, ESkillSlot.A);
        Skills.AddSkill(_heroInfo.BSkillDataId, ESkillSlot.B);
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

    #region AI

    protected override void UpdateIdle()
    {
        // 0. 이동 상태라면 강제 변경
        if (HeroMoveState == EHeroMoveState.ForceMove)
        {
            CreatureState = ECreatureState.Move;
            return;
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
        if (transform.position.x > HeroCampDest.position.x)
            LookLeft = true;
        else
            LookLeft = false;

        transform.position = Managers.Object.HeroCamp.transform.position;
        // 너무 멀어서 강제 조정.
/*        if (HeroMoveState == EHeroMoveState.ForcePath)
        {
            //MoveByForcePath();
            return;
        }

        if (CheckHeroCampDistanceAndForcePath())
            return;

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

            ChaseOrAttackTarget(GetAttackDistanceSqr(), HERO_SCAN_RANGE);
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

            ChaseOrAttackTarget(GetAttackDistanceSqr(), HERO_SCAN_RANGE);
            return;
        }

        // 3. Camp 주변으로 모이기
        if (HeroMoveState == EHeroMoveState.ReturnToCamp)
        {
            Vector3 destPos = HeroCampDest.position;
            if (FindPathAndMoveToCellPos(destPos, HERO_DEFAULT_MOVE_DEPTH) == EFindPathResult.Success)
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
                    HeroMoveState = EHeroMoveState.None;
                    CreatureState = ECreatureState.Idle;

                    NeedArange = false;
                    return;
                }
            }
        }

        // 4. 기타 (누르다 뗐을 때)
        if (LerpCellPosCompleted)
            CreatureState = ECreatureState.Idle;*/
    }

    Queue<Vector3Int> _forcePath = new Queue<Vector3Int>();
    bool CheckHeroCampDistanceAndForcePath()
    {
        // 너무 멀어서 못 간다.
        Vector3 destPos = HeroCampDest.position;
        Vector3Int destCellPos = Managers.Map.World2Cell(destPos);
        if ((CellPos - destCellPos).magnitude <= 10)
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
            CreatureState = ECreatureState.Move;
            return;
        }
    }

    protected override void UpdateDead()
    {

    }

    #endregion

    #region Level System
    private void HandleOnLevelUp(int level)
    {
        Debug.Log($"{gameObject.name} Level UP -> {level} ");

        InitCreatureStat();
        // TODO 스킬 레벨업 확인, 
        UpdateSkillSlot();

        //Character 레벨업

    }
    #endregion

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

    private void OnCollisionStay2D(Collision2D other)
    {
        Creature creature = other.gameObject.GetComponent<Creature>();
        if (creature == null)
            return;

        switch (HeroMoveState)
        {
            case EHeroMoveState.TargetMonster:
                if (creature.ObjectType == EObjectType.Hero)
                {
                    // Vector2 dirToOther = creature.CenterPosition - CenterPosition;
                    // Vector2 rightDir = Quaternion.Euler(0, 0, -90) * MovementInput;
                    // float dot = Vector2.Dot(dirToOther, rightDir);
                    // if (dot > 0)
                    // {
                    //     creature.Rigid.AddForce(rightDir.normalized * 250f, ForceMode2D.Force);
                    // }
                    // else
                    // {
                    //     Vector2 leftDir = Quaternion.Euler(0, 0, 90) * MovementInput;
                    //     creature.Rigid.AddForce(leftDir.normalized * 250f, ForceMode2D.Force);
                    // }
                }
                break;
            case EHeroMoveState.ForceMove:
                if (creature.ObjectType == EObjectType.Monster)
                {
                    //진행방향의 양 옆으로 밈
                    // var pushDir = ( creature.CenterPosition - CenterPosition).normalized;                    
                    // creature.Rigid.AddForce(pushDir * 2500f, ForceMode2D.Impulse);
                }
                break;
        }
    }
}
