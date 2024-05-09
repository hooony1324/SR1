using Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ArtTestScene : BaseScene
{
    private Vector2 _moveDir;
    Creature_Cheat _creature;
    public Creature_Cheat Creature_Cheat { get { return _creature; } }

    GameObject _currentMap;
    Dictionary<string, GameObject> _mapDict = new Dictionary<string, GameObject>();

    public Vector2 MoveDir
    {
        get => _moveDir;
        set
        {
            _moveDir = value;
            OnMoveDirChanged?.Invoke(_moveDir);
        }
    }

    private EJoystickState _joystickState;

    public EJoystickState JoystickState
    {
        get => _joystickState;
        set
        {
            _joystickState = value;
            OnJoystickStateChanged?.Invoke(_joystickState);
            HandleOnJoystickStateChanged(_joystickState);
        }
    }

    public event Action<Vector2> OnMoveDirChanged;
    public event Action<EJoystickState> OnJoystickStateChanged;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        SceneType = EScene.ArtTestScene;

        _creature = Managers.Resource.Instantiate("CreatureCheatPrefab").GetOrAddComponent<Creature_Cheat>();
        _creature.SetInfo(Managers.Data.HeroDic[201006]);

        Managers.UI.ShowSceneUI<UI_JoystickTest>();
        Managers.UI.ShowSceneUI<UI_ArtTestScene_Cheat>().GetComponent<Canvas>().sortingOrder = 1;

        OnJoystickStateChanged -= HandleOnJoystickStateChanged;
        OnJoystickStateChanged += HandleOnJoystickStateChanged;

        Managers.Game.Cam.transform.position = _creature.Position;
        Managers.Game.Cam.Target = _creature;

        CreateMap("BaseMap");

        return true;
    }

    public void SetCharacter(CreatureData creatureData)
    {
        _creature.SetInfo(creatureData);
    }

    public void SetMoveSpeed(float speed)
    {
        _creature.SetMoveSpeed(speed);
    }

    private void HandleOnJoystickStateChanged(EJoystickState joystickState)
    {
        switch (joystickState)
        {
            case EJoystickState.PointerDown:
                _creature.UpdateAnimation(ECreatureState.Move);
                break;
            case EJoystickState.Drag:
                break;
            case EJoystickState.PointerUp:
                _creature.UpdateAnimation(ECreatureState.Idle);
                break;
            default:
                break;
        }
    }

    public override void Clear()
    {
        Managers.Game.OnJoystickStateChanged -= HandleOnJoystickStateChanged;
    }

    public void CreateMap(string mapName)
    {
        if (_currentMap != null)
            _currentMap.gameObject.SetActive(false);

        GameObject map;
        if (_mapDict.TryGetValue(mapName, out map))
        {
            map.gameObject.SetActive(true);
            _currentMap = map;
        }
        else
        {
            Debug.Log(mapName);
            map = Managers.Resource.Instantiate(mapName);
            _currentMap = map;
            _mapDict.Add(mapName, map);
        }

        Transform centerPos = _currentMap.transform.Find("CenterPosition");
        if (centerPos != null)
            Creature_Cheat.transform.position = centerPos.position;
        else
            Creature_Cheat.transform.position = Vector3.zero;
    }
}
