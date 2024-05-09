using System;
using System.Collections;
using UnityEngine;
using static Define;

public class HeroCamp : InteractionObject
{
    private Vector3 _moveDir { get; set; }
    public float Speed { get; set; }
    public Transform Pivot { get; protected set; }
    public Transform Destination { get; protected set; }
    private Npc _portal;
    private UI_CampProgressBar _progressBar;
    private const int CAMP_TIME = 3;
    private ECampState _campState = ECampState.Idle;

    IEnumerator _campAni;

    public ECampState CampState
    {
        get { return _campState; }
        set
        {
            {
                Debug.Log($"CampState : {value}");
                _campState = value;
                UpdateAnimation();
                Managers.Game.BroadcastEvent(EBroadcastEventType.ChangeCampState, value:(int)value);
            }
        }
    }

    protected override bool Init()
    {
        base.Init();

        ObjectType = EObjectType.Camp;
        gameObject.AddComponent<GridDrawer>();
        _progressBar = Util.FindChild<UI_CampProgressBar>(gameObject, recursive: true);
        _progressBar.SetInfo(this, CAMP_TIME);
        _progressBar.gameObject.SetActive(false);
        Speed = 7;

        #region Event

        Managers.Game.OnMoveDirChanged -= HandleOnMoveDirChanged;
        Managers.Game.OnMoveDirChanged += HandleOnMoveDirChanged;
        Managers.Game.OnJoystickStateChanged -= HandleOnJoystickStateChanged;
        Managers.Game.OnJoystickStateChanged += HandleOnJoystickStateChanged;

        #endregion

        #region Spine Animation

        SetSpineAnimation("ui_npc_camp_fire_SkeletonData", SortingLayers.NPC, gameObject.name);
        PlayAnimation(0, "idle_1", true);

        #endregion

        #region Collider

        CurrentCollider = gameObject.GetComponent<CircleCollider2D>();

        #endregion

        #region Direction

        Pivot = Util.FindChild<Transform>(gameObject, "Pivot", true);
        Destination = Util.FindChild<Transform>(Pivot.gameObject, "Destination", true);

        #endregion

        Managers.Game.OnBroadcastEvent += HandleOnBroadcast;
        return true;
    }

    private void Update()
    {
        Vector3 dir = _moveDir * (Time.deltaTime * Speed);
        Vector3 newPos = transform.position + dir;

        if (Managers.Map == null)
            return;
        if (Managers.Map.CanGo(this, newPos, ignoreObjects: true, ignoreSemiWall: true) == false)
            return;
        if (Managers.Game.Cam.State == ECameraState.Targeting)
            return;
        if(CampState == ECampState.MoveToTarget)
            return;

        transform.position = newPos;
    }

    private void UpdateAnimation()
    {
        switch (CampState)
        {
            case ECampState.Idle:
                PlayAnimation(0, "idle_1", true, isMix:false);
                _progressBar.gameObject.SetActive(false);
                StopAllCoroutines();

                if (Managers.Game.IsOnAutoCamp == false)
                {
                    break;
                }

                //마을인 경우에는 캠프모드 안들어감
/*                if (Managers.Map.StageTransition.CurrentStage.IsTownStage == false)
                {
                    StopAllCoroutines();
                    StartCoroutine(CoStartCampMode());
                }*/
                break;
            case ECampState.Move:
                StopAllCoroutines();
                PlayAnimation(0, "dead", false, isMix:false);
                _progressBar.gameObject.SetActive(false);

                // 투명 애니메이션으로 변환
                break;
            case ECampState.CampMode:
                //캠프모양으로 변환
                _campAni = CampAnimation();
                StartCoroutine(_campAni);
                break;
        }
    }
    
    public void ForceMove(Vector3 position)
    {
        transform.position = position;
    }

    public void StartCampMode()
    {
        CampState = ECampState.CampMode;
    }

    public void EndCampMode()
    {
        StopCoroutine(_campAni);
        CampState = ECampState.Idle;
    }

    IEnumerator CampAnimation()
    {
        PlayAnimation(0, "idle_2", true, isMix: false);

        yield return new WaitForSeconds(6f);

        PlayAnimation(0, "camp", true, isMix: false);
    }

    public void MoveToTarget(Vector3 destPos , Action action)
    {
        CampState = ECampState.MoveToTarget;
        StopAllCoroutines();
        StartCoroutine(CoMoveToTarget(destPos, action));
    }

    IEnumerator CoMoveToTarget(Vector3 destPos, Action action)
    {
        while (Vector3.Distance(destPos, transform.position) > 0.5f)
        {
            Vector3 moveDir = (destPos - transform.position).normalized;
            Vector3 dir = moveDir * (Time.deltaTime * Speed);
            Vector3 newPos = transform.position + dir;
            transform.position = newPos;
            yield return null;
        }
        
        action?.Invoke();

        // yield return new WaitForSeconds(1.5f);

        // if (Managers.Map.CanGo(null, transform.position, ignoreObjects: false) == false)
        Hero nearest = FindNearestHero();
        if (nearest.IsValid())
            transform.position = nearest.Position;
        
        CampState = ECampState.Idle;

    }

    public void OpenPortal()
    {
        if (Managers.Map.StageTransition.CurrentStage.IsTownStage)
        {
            return;
        }

        //기존에 있던 포탈 제거
        if (_portal.IsValid())
        {
            Managers.Object.Despawn(_portal);
        }

        _portal = Managers.Object.Spawn<Npc>(Managers.Game.Leader.Position, PORTAL_DATA_ID);
        _portal.name = "CampPortal";
        _portal.SetInfo(PORTAL_DATA_ID);

        PortalInteraction pi = _portal.Interaction as PortalInteraction;
        if (pi != null)
        {
            pi.ConnectPortal(Managers.Game.TownPortal, false);
        }
    }
    
    private void HandleOnMoveDirChanged(Vector2 dir)
    {
        _moveDir = dir;
        if (dir != Vector2.zero)
        {
            float angle = Mathf.Atan2(-dir.x, +dir.y) * 180 / Mathf.PI;

            if (angle > 15f && angle <= 75f)
                _moveDir = MoveDir.TOP_LEFT;
            else if (angle > 75f && angle <= 105f)
                _moveDir = MoveDir.LEFT;
            else if (angle > 105f && angle <= 160f)
                _moveDir = MoveDir.BOTTOM_LEFT;
            else if (angle > 160f || angle <= -160f)
                _moveDir = MoveDir.BOTTOM;
            else if (angle < -15f && angle >= -75f)
                _moveDir = MoveDir.TOP_RIGHT;
            else if (angle < -75f && angle >= -105f)
                _moveDir = MoveDir.RIGHT;
            else if (angle < -105f && angle >= -160f)
                _moveDir = MoveDir.BOTTOM_RIGHT;
            else
                _moveDir = MoveDir.TOP;

            Debug.Log($"{_moveDir}, {angle}");
            //_moveDir = _moveDir.normalized;

            Pivot.eulerAngles = new Vector3(0, 0, angle); //      
        }
    }
    
    private Hero FindNearestHero()
    {
        float minDistance = float.PositiveInfinity;
        Hero nearestHero = null;
        foreach (Hero hero in Managers.Object.Heroes)
        {
            float distance = Vector3.SqrMagnitude(Position - hero.Position);
            if (distance <= minDistance)
            {
                minDistance = distance;
                nearestHero = hero;
            }
        }

        return nearestHero;
    }

    private void HandleOnBroadcast(EBroadcastEventType type, ECurrencyType currencyType, int value)
    {
        if (type != EBroadcastEventType.ChangeSetting) return;

        CampState = ECampState.Idle;
    }

    #region Map

    public EHeroMoveState HeroMoveState { get; set; } = EHeroMoveState.None;

    private void HandleOnJoystickStateChanged(EJoystickState joystickState)
    {
        switch (joystickState)
        {
            case EJoystickState.PointerDown:
                Hero nearest = FindNearestHero();
                if (nearest.IsValid())
                    transform.position = nearest.Position;
                HeroMoveState = EHeroMoveState.ForceMove;
                Speed = Managers.Game.Leader.MoveSpeed * 0.87f;
                CampState = ECampState.Move;
                break;
            case EJoystickState.Drag:
                HeroMoveState = EHeroMoveState.ForceMove;
                break;
            case EJoystickState.PointerUp:
                HeroMoveState = EHeroMoveState.None;
                CampState = ECampState.Idle;

                if (Managers.Map.CanGo(null, transform.position, ignoreObjects: false))
                    return;

                //HeroCamp가 Semiwall등 영웅이 못가는 지역에 있으면 가장가까운 영웅에게 이동
                Hero nearestHero = FindNearestHero();
                if (nearestHero.IsValid())
                    transform.position = nearestHero.Position;
                
                break;
        }
    }

    #endregion
}