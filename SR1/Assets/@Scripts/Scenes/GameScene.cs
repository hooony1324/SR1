using System.Collections;
using UnityEngine;
using static Define;

public class GameScene : BaseScene
{
    [SerializeField] private float _detectionDelay = 0.2f;

    private Coroutine _detectionCoroutine;

    UI_Joystick _joystick;
    public UI_Joystick Joystick { get { return _joystick; } }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

#if UNITY_EDITOR
        gameObject.AddComponent<CaptureScreenShot>();
#endif
        Debug.Log("@>> GameScene Init()");
        SceneType = EScene.GameScene;

        int key = PlayerPrefs.GetInt("DROPDOWN_KEY", 0);
        if (key == 0)
        {
            MapName = "BaseMap";
        }
        else
        {
            MapName = "DevMap";
        }
        Managers.Map.LoadMap(MapName);

        Managers.Map.StageTransition.SetInfo();
        ObjectSpawnInfo startInfo = new ObjectSpawnInfo(
            "startInfo",
            0, 
            Managers.Game.LastCellPos.x, 
            Managers.Game.LastCellPos.y, 
            Managers.Map.Cell2World(Managers.Game.LastCellPos),
            EObjectType.Npc,
            ENpcType.StartPosition);
        
        //1. 저장된 마지막 위치 확인
        if(Managers.Game.LastCellPos == Vector3Int.zero)
            startInfo = Managers.Map.StageTransition.CurrentStage.StartSpawnInfo;
        
        #region Spawn
        
        //Camp
        HeroCamp camp = Managers.Object.Spawn<HeroCamp>(startInfo.CellPos);
        camp.SetCellPos(startInfo.CellPos, true);

        //카메라 위치 설정
        Managers.Game.Cam.transform.position = Managers.Object.HeroCamp.Position;
        Managers.Game.Cam.Target = Managers.Object.HeroCamp;
        
        //Hero
         foreach (var saveData in Managers.Hero.PickedHeroes)
         {
             if (Managers.Game.Leader == null)
             {
                 Hero hero = Managers.Hero.PickHero(saveData.TemplateId, startInfo.CellPos);
                 hero.MyLeader = null;
             }
             else
             {
                 Hero hero = Managers.Hero.PickHero(saveData.TemplateId, Vector3Int.zero);
                 hero.MyLeader = Managers.Game.Leader;
             }
         }

        #endregion

        //Event`
        Managers.Game.OnJoystickStateChanged -= HandleOnJoystickStateChanged;
        Managers.Game.OnJoystickStateChanged += HandleOnJoystickStateChanged;
        Managers.Game.OnBroadcastEvent -= HandleOnBroadcastEvent;
        Managers.Game.OnBroadcastEvent += HandleOnBroadcastEvent;
        
        if (_detectionCoroutine == null)
        {
            _detectionCoroutine = StartCoroutine(CoPerformDetection());
        }

        #region Scene UI
        Managers.UI.CacheAllPopups();

        _joystick = Managers.UI.ShowSceneUI<UI_Joystick>();
        
        UI_GameScene sceneUI = Managers.UI.ShowSceneUI<UI_GameScene>();
        sceneUI.GetComponent<Canvas>().sortingOrder = 1;
        Managers.UI.SceneUI = sceneUI;
        sceneUI.SetInfo();

        #endregion

        Managers.Game.Cam.SetCameraSize();
        StartCoroutine(CoSave());
        return true;
    }

    IEnumerator CoSave()
    {
        WaitForSeconds wait = new WaitForSeconds(1);
        while (true)
        {
            yield return wait;
            Managers.Game.SaveGame();
        }
    }

#if UNITY_EDITOR
    // 60001	도트뎀출혈 
    // 60002	도트뎀독 o
    // 60003	화상
    // 60004	도트힐 o
    // 60005	힐 o
    // 61001	공업버프
    // 61002	공업디버프
    // 61003	이속버프 o
    // 61004	이속디버프
    // 61005	공속버프 o
    // 61006	공속디버프
    // 61007	흡혈량버프 o
    // 61008	흡혈량디버프
    // 61009	받는피해버프
    // 61010	받는피해디버프
    // 61011	가시
    // 62001	넉백
    // 62002	에어본
    // 62003	땡기기
    // 62004	스턴 o
    // 62005	빙결

    //60009
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Managers.Game.Cam.SetCameraSize();

            // // List<InteractionObject> targets = Managers.Object.FindCircleRangeTargets(Managers.Game.Leader.Position, 5, EObjectType.Hero);
            // foreach (var hero in Managers.Object.Heroes)
            // {
            //     // int[] arr = new[] { 60009};
            //     List<int> list = new List<int>(){61001};
            //     hero.Effects.GenerateEffects(list, EEffectSpawnType.Skill, Managers.Game.Leader);
            //     
            //     // hero.CreatureState = ECreatureState.Dead;
            // }
        }
        
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Managers.Object.HeroCamp.OpenPortal();
            // foreach (var hero in Managers.Object.Heroes)
            // {
            //     // int[] arr = new[] { 62006 };
            //     // hero.Effects.GenerateEffects(arr, EEffectSpawnType.Skill);
            //     hero.CreatureState = ECreatureState.Idle;
            //
            // }
        }
        
        if (Input.GetKeyDown(KeyCode.F9))
        {
            UI_QuestPopup popup = Managers.UI.ShowPopupUI<UI_QuestPopup>();
            popup.SetInfo();
        }
    }
#endif

    public override void Clear()
    {
        Managers.Game.OnJoystickStateChanged -= HandleOnJoystickStateChanged;
    }

    //isAutoMode == true : 실행, False : 종료
    private IEnumerator CoPerformDetection()
    {
        WaitForSeconds wait = new WaitForSeconds(_detectionDelay);
        yield return wait;

        while (true)
        {
            Managers.Object.PerformMatching();
            yield return wait;
        }
    }

    private void OnDefeated()
    {
        Time.timeScale = 0;
        UI_GameOverPopup gameOverPopup = Managers.UI.ShowPopupUI<UI_GameOverPopup>();
        gameOverPopup.SetInfo((GoToTown) =>
        {
            Time.timeScale = 1;
            if (GoToTown == true)
            {
                Managers.Map.StageTransition.OnMapChanged(Managers.Map.StageTransition.TownStage.StageIndex);

                //teleport to Town
                foreach (var hero in Managers.Object.Heroes)
                {
                    hero.Rebirth();
                    Vector3 teleportPos = Managers.Map.StageTransition.TownStage.WaypointSpawnInfos[0].WorldPos;
                    Managers.Game.TeleportHeroes(teleportPos);
                }  
            }
            else
            {
                foreach (var hero in Managers.Object.Heroes)
                {
                    hero.Rebirth();
                }   
            }
        });
    }

    private bool IsDefeated()
    {
        foreach (var hero in Managers.Object.Heroes)
        {
            if (hero.CreatureState != ECreatureState.Dead)
                return false;
        }

        return true;
    }

    private void HandleOnJoystickStateChanged(EJoystickState joystickState)
    {
        switch (joystickState)
        {
            case EJoystickState.PointerDown:
                break;
            case EJoystickState.Drag:
                break;
            case EJoystickState.PointerUp:

                break;
            default:
                break;
        }
    }
    
    private void HandleOnBroadcastEvent(EBroadcastEventType eventType, ECurrencyType currencyType, int value)
    {
        switch (eventType)
        {
            case EBroadcastEventType.HeroDead:
                if (IsDefeated())
                {
                    OnDefeated();
                }

                break;
        }
    }
}