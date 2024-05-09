using UnityEngine;
using Data;
using static Define;

public class Creature_Cheat : InteractionObject
{ 
    private Vector3 _moveDir { get; set; }
    public Transform Pivot { get; private set; }

    CreatureData _creatureData;

    float _moveSpeed;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        (Managers.Scene.CurrentScene as ArtTestScene).OnMoveDirChanged -= HandleOnMoveDirChanged;
        (Managers.Scene.CurrentScene as ArtTestScene).OnMoveDirChanged += HandleOnMoveDirChanged;
        (Managers.Scene.CurrentScene as ArtTestScene).OnJoystickStateChanged -= HandleOnJoystickStateChanged;
        (Managers.Scene.CurrentScene as ArtTestScene).OnJoystickStateChanged += HandleOnJoystickStateChanged;

        return true;
    }

    private void Update()
    {
        Vector3 moveDir = _moveDir.normalized * _moveSpeed * Time.deltaTime;
        transform.Translate(moveDir);
        //transform.position += moveDir;
    }

    public void SetInfo(CreatureData data)
    {
        _creatureData = data;
        SetSpineAnimation(data.SkeletonDataID, 800, "SkeletonAnimation");

        _moveSpeed = _creatureData.MoveSpeed;
    }

    public void SetMoveSpeed(float speed)
    {
        _moveSpeed = speed;
    }

    protected virtual void UpdateIdle()
    {

    }

    public void SetAnimation(string animationName)
    {
        PlayAnimation(0, animationName, true);

    }

    protected virtual void UpdateMove() { }

    private void HandleOnJoystickStateChanged(EJoystickState joystickState)
    {
/*        switch (joystickState)
        {
            case EJoystickState.PointerDown:
                HeroMoveState = EHeroMoveState.ForceMove;
                Speed = Managers.Game.Leader.MoveSpeed.Value;
                break;
            case EJoystickState.Drag:
                HeroMoveState = EHeroMoveState.ForceMove;
                break;
            case EJoystickState.PointerUp:
                HeroMoveState = EHeroMoveState.None;
                break;
            default:
                break;
        }*/
    }

    private void HandleOnMoveDirChanged(Vector2 dir)
    {
        _moveDir = dir;

        if (dir.x < 0)
            LookLeft = true;
        else if(dir.x > 0)
            LookLeft = false;

        if (dir != Vector2.zero)
        {
            float angle = Mathf.Atan2(-dir.x, +dir.y) * 180 / Mathf.PI;
            //Pivot.eulerAngles = new Vector3(0, 0, angle);
        }
    }

    public void UpdateAnimation(ECreatureState state)
    {
        switch (state)
        {
            case ECreatureState.Idle:
                PlayAnimation(0, AnimName.IDLE, true);
                break;

            case ECreatureState.Move:
                PlayAnimation(0, AnimName.MOVE, true);
                break;
        }
    }
}
