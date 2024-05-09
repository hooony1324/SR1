using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public class UI_Joystick : UI_Scene
{
    private GameObject _handler;
    private GameObject _joystickBG;
    private Vector2 _moveDir { get; set; }
    private Vector2 _joystickTouchPos;
    private Vector2 _joystickOriginalPos;
    private float _radius;

    private enum GameObjects
    {
        JoystickBG,
        JoystickCursor,
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
       
        _joystickBG = GetObject((int)GameObjects.JoystickBG);
        _handler = GetObject((int)GameObjects.JoystickCursor);
        _joystickOriginalPos = _joystickBG.transform.position;
        gameObject.BindEvent(OnPointerDown, null, type: UIEvent.PointerDown);
        gameObject.BindEvent(OnPointerUp, null,  type: UIEvent.PointerUp);
        gameObject.BindEvent(null, OnDrag, type: UIEvent.Drag);

        // SetActiveJoystick(false);
        GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        GetComponent<Canvas>().worldCamera = Camera.main;

        return true;
    }

    void Start()
    {
        _radius = _joystickBG.GetComponent<RectTransform>().sizeDelta.y / 5;
        Managers.Game.OnBroadcastEvent -= HandleOnBroadcast;
        Managers.Game.OnBroadcastEvent += HandleOnBroadcast;
    }
    
    private void HandleOnBroadcast(EBroadcastEventType type, ECurrencyType currencyType, int value)
    {
        switch (type)
        {
            case EBroadcastEventType.ChangeCampState:
                if ((ECampState)value == ECampState.CampMode)
                {
                    SetActiveJoystick(false);
                }
                else
                {
                    SetActiveJoystick(true);
                }
                break;
        }
    }
    
    #region Event
    private void OnPointerDown()
    {
        if(Managers.Object.HeroCamp.CampState == ECampState.CampMode)
            return;
        
        _joystickTouchPos = Input.mousePosition;
        Managers.Game.JoystickState = EJoystickState.PointerDown;

        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Managers.Game.JoystickType == EJoystickType.Flexible)
        {
            _handler.transform.position = mouseWorldPos;
            _joystickBG.transform.position = mouseWorldPos;



        }
        (Managers.UI.SceneUI as UI_GameScene).HideUIOnMove();
        Managers.Game.MoveDir = _moveDir;
        Managers.Game.JoystickState = EJoystickState.Drag;
    }

    private void OnDrag(BaseEventData baseEventData)
    {
        if(Managers.Object.HeroCamp.CampState == ECampState.CampMode)
            return;
        
        PointerEventData pointerEventData = baseEventData as PointerEventData;
        Vector2 dragePos = pointerEventData.position;

        _moveDir = Managers.Game.JoystickType == EJoystickType.Fixed
            ? (dragePos - _joystickOriginalPos).normalized
            : (dragePos - _joystickTouchPos).normalized;

        // 조이스틱이 반지름 안에 있는 경우
        float joystickDist = (dragePos - _joystickOriginalPos).sqrMagnitude;

        Vector3 newPos;
        // 조이스틱이 반지름 안에 있는 경우
        if (joystickDist < _radius)
        {
            newPos = _joystickTouchPos + _moveDir * joystickDist;
        }
        else // 조이스틱이 반지름 밖에 있는 경우
        {
            newPos = Managers.Game.JoystickType == EJoystickType.Fixed
                ? _joystickOriginalPos + _moveDir * _radius
                : _joystickTouchPos + _moveDir * _radius;
        }

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(newPos);

        _handler.transform.position = worldPos;


        Managers.Game.JoystickState = EJoystickState.Drag;
        Managers.Game.MoveDir = _moveDir;
    }

    private void OnPointerUp()
    {
        if(Managers.Object.HeroCamp.CampState == ECampState.CampMode)
            return;
        
        _moveDir = Vector2.zero;

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(_joystickOriginalPos);

        _handler.transform.position = worldPos;
        _joystickBG.transform.position = worldPos;
        Managers.Game.MoveDir = _moveDir;
        Managers.Game.JoystickState = EJoystickState.PointerUp;

        // SetActiveJoystick(false);
    }

    private void SetActiveJoystick(bool isActive)
    {
        if (isActive == true)
        {
            _handler.GetComponent<Image>().DOFade(1, 0.5f);
            _joystickBG.GetComponent<Image>().DOFade(1, 0.5f);
        }
        else
        {
            _handler.GetComponent<Image>().DOFade(0, 0.5f);
            _joystickBG.GetComponent<Image>().DOFade(0, 0.5f);
        }
    }
    #endregion
    
    
}
