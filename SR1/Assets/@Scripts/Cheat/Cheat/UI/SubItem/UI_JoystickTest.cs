using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_JoystickTest : UI_Scene
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
        gameObject.BindEvent(OnPointerDown, null, type: Define.UIEvent.PointerDown);
        gameObject.BindEvent(OnPointerUp, null, type: Define.UIEvent.PointerUp);
        gameObject.BindEvent(null, OnDrag, type: Define.UIEvent.Drag);

        // SetActiveJoystick(false);

        return true;
    }

    void Start()
    {
        _radius = _joystickBG.GetComponent<RectTransform>().sizeDelta.y / 5;

    }
    #region Event
    public void OnPointerDown()
    {
        // SetActiveJoystick(true);
        _joystickTouchPos = Input.mousePosition;
        (Managers.Scene.CurrentScene as ArtTestScene).JoystickState = Define.EJoystickState.PointerDown;

        if (Managers.Game.JoystickType == Define.EJoystickType.Flexible)
        {
            _handler.transform.position = Input.mousePosition;
            _joystickBG.transform.position = Input.mousePosition;
        }
        (Managers.Scene.CurrentScene as ArtTestScene).MoveDir = _moveDir;
        (Managers.Scene.CurrentScene as ArtTestScene).JoystickState = Define.EJoystickState.Drag;
    }

    public void OnDrag(BaseEventData baseEventData)
    {
        PointerEventData pointerEventData = baseEventData as PointerEventData;
        Vector2 dragePos = pointerEventData.position;

        _moveDir = Managers.Game.JoystickType == Define.EJoystickType.Fixed
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
            newPos = Managers.Game.JoystickType == Define.EJoystickType.Fixed
                ? _joystickOriginalPos + _moveDir * _radius
                : _joystickTouchPos + _moveDir * _radius;
        }

        _handler.transform.position = newPos;
        (Managers.Scene.CurrentScene as ArtTestScene).JoystickState = Define.EJoystickState.Drag;
        (Managers.Scene.CurrentScene as ArtTestScene).MoveDir = _moveDir;
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

    public void OnPointerUp()
    {
        _moveDir = Vector2.zero;
        _handler.transform.position = _joystickOriginalPos;
        _joystickBG.transform.position = _joystickOriginalPos;
        (Managers.Scene.CurrentScene as ArtTestScene).MoveDir = _moveDir;
        (Managers.Scene.CurrentScene as ArtTestScene).JoystickState = Define.EJoystickState.PointerUp;

        // SetActiveJoystick(false);
    }

    #endregion
}
