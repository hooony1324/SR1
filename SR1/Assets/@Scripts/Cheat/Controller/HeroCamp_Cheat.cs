using UnityEngine;
using static Define;

public class HeroCamp_Cheat : HeroCamp
{
    private Vector3 _moveDir { get; set; }

    protected override bool Init()
    {
        base.Init();

        ObjectType = EObjectType.Camp;
        gameObject.AddComponent<GridDrawer>();

        #region Event
        Managers.Game.OnMoveDirChanged -= HandleOnMoveDirChanged;
        Managers.Game.OnMoveDirChanged += HandleOnMoveDirChanged;
        Managers.Game.OnJoystickStateChanged -= HandleOnJoystickStateChanged;
        Managers.Game.OnJoystickStateChanged += HandleOnJoystickStateChanged;
        #endregion

        #region Collider
        CurrentCollider = gameObject.GetComponent<CircleCollider2D>();
        //unity 2023에서 새로 추가된 기능. 그 밑 버전 사용불가 
        // CurrentCollider.includeLayers = 1 << (int)ELayer.Obstacle; 
        // CurrentCollider.excludeLayers = 1 << (int)ELayer.Boss |1 << (int)ELayer.Monster | 1 << (int)ELayer.Hero; 
        #endregion

        #region Direction
        Pivot = Util.FindChild<Transform>(gameObject, "Pivot", true);
        Destination = Util.FindChild<Transform>(gameObject, "Destination", true);
        #endregion

        return true;
    }

    private void Update()
    {
        Vector3 dir = _moveDir * (Time.deltaTime * Speed);
        Vector3 newPos = transform.position + dir;

        //if (Managers.Map == null)
        //    return;
        //if (Managers.Map.CanGo(newPos, ignoreObjects: true, ignoreSemiWall: true) == false)
        //    return;

        transform.position = newPos;
    }

    private void HandleOnMoveDirChanged(Vector2 dir)
    {
        _moveDir = dir;
        Debug.Log(dir);
        if (dir != Vector2.zero)
        {
            float angle = Mathf.Atan2(-dir.x, +dir.y) * 180 / Mathf.PI;
            Pivot.eulerAngles = new Vector3(0, 0, angle);
        }
    }
    #region Map

    private void HandleOnJoystickStateChanged(EJoystickState joystickState)
    {
        switch (joystickState)
        {
            case EJoystickState.PointerDown:
                Speed = Managers.Game.Leader.MoveSpeed;
                break;
            case EJoystickState.Drag:
                break;
            case EJoystickState.PointerUp:
                break;
            default:
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("OnCollisionEnter2D" + other.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("OnTriggerEnter2D" + other.gameObject);
    }
    #endregion
}
