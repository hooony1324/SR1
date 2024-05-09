using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static Define;

public class UI_GameScene : UI_Scene
{
    #region Enum
    enum GameObjects
    {
        TabButtonGroup,
        QuestProgressInfo,
        QuestCompleteInfo,
    }

    enum Images
    {
        QuestTargetImage,
        QuestRewardImage,

    }

    enum Buttons
    {
        GoldPlusButton,
        DiaPlusButton,
        OpenTabButton,
        CloseTabButton,
        //HeroesListButton,
        HeroesButton,
        SettingButton,
        InventoryButton,
        WorldMapButton,
        QuestButton,
        ChallengeButton,
        //ShopButton,
        //PortalButton,
        //CampButton,
        CheatButton,
        //QuestRewardButton,
        //MoveToQuestButton,
        StartCampModeButton,
        EndCampModeButton,
    }

    enum Texts
    {
        LevelText,
        BattlePowerText,
        GoldCountText,
        DiaCountText,
        MeatCountText,
        WoodCountText,
        MineralCountText,
        FpsText,
        ExpCountText,
        QuestDescriptionText,
        QuestProgressValueText,
        QuestRewardText,
        RewardCountText
    }

    enum Sliders
    {
        MeatSlider,
        WoodSlider,
        MineralSlider,
        PlayerExpSlider,
        QuestProgressSlider
    }

    #endregion

    Quest _mainQuest;
    QuestTask _questTask;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));
        BindImage(typeof(Images));
        BindSlider(typeof(Sliders));

        GetButton((int)Buttons.GoldPlusButton).gameObject.BindEvent(OnClickGoldPlusButton);
        GetButton((int)Buttons.DiaPlusButton).gameObject.BindEvent(OnClickDiaPlusButton);
        GetButton((int)Buttons.SettingButton).gameObject.BindEvent(OnClickSettingButton);
        //GetButton((int)Buttons.QuestButton).gameObject.BindEvent(OnClickQuestButton);
        //GetButton((int)Buttons.CampButton).gameObject.BindEvent(OnClickCampButton);
        GetButton((int)Buttons.HeroesButton).gameObject.BindEvent(OnClickHeroesButton);
        GetButton((int)Buttons.InventoryButton).gameObject.BindEvent(OnClickInventoryButton);
        GetButton((int)Buttons.WorldMapButton).gameObject.BindEvent(OnClickWorldmapButton);
        GetButton((int)Buttons.ChallengeButton).gameObject.BindEvent(OnClickChallengeButton);
        //GetButton((int)Buttons.ShopButton).gameObject.BindEvent(OnClickShopButton);
        GetButton((int)Buttons.OpenTabButton).gameObject.BindEvent(OnClickTabButton);
        GetButton((int)Buttons.CloseTabButton).gameObject.BindEvent(OnClickTabButton);
        GetButton((int)Buttons.CloseTabButton).gameObject.SetActive(false);

        //GetButton((int)Buttons.QuestRewardButton).gameObject.BindEvent(OnClickReward);
        GetObject((int)GameObjects.QuestCompleteInfo).BindEvent(OnClickReward);
        //GetButton((int)Buttons.MoveToQuestButton).gameObject.BindEvent(OnClickMoveToQuest);
        GetObject((int)GameObjects.QuestProgressInfo).BindEvent(OnClickMoveToQuest);
        
        GetButton((int)Buttons.CheatButton).gameObject.BindEvent(OnClickCheatButton);

        GetButton((int)Buttons.StartCampModeButton).gameObject.BindEvent(OnClickStartCampModeButton);
        GetButton((int)Buttons.EndCampModeButton).gameObject.BindEvent(OnClickEndCampModeButton);
        GetButton((int)Buttons.EndCampModeButton).gameObject.SetActive(false);

        GetSlider((int)Sliders.MineralSlider).gameObject.BindEvent(() =>
        {
            Managers.Inventory.EarnCurrency(ECurrencyType.Mineral, 10);
        });
        GetSlider((int)Sliders.MeatSlider).gameObject.BindEvent(() =>
        {
            Managers.Inventory.EarnCurrency(ECurrencyType.Meat, 10);
        });
        GetSlider((int)Sliders.WoodSlider).gameObject.BindEvent(() =>
        {
            Managers.Inventory.EarnCurrency(ECurrencyType.Wood, 10);
        });
        GetSlider((int)Sliders.PlayerExpSlider).gameObject.BindEvent(() =>
        {
            Managers.Game.AddExp(1);
            Refresh();
        });

        GetObject((int)GameObjects.TabButtonGroup).SetActive(false);
        Refresh();

        return true;
    }

    private float elapsedTime = 0.0f;
    private float updateInterval = 0.3f;

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= updateInterval)
        {
            float fps = 1.0f / Time.deltaTime;
            float ms = Time.deltaTime * 1000.0f;
            string text = string.Format("{0:N1} FPS ({1:N1}ms)", fps, ms);
            GetText((int)Texts.FpsText).text = text;

            elapsedTime = 0;
        }
    }

    public void SetInfo()
    {
        Managers.Game.OnBroadcastEvent += HandleOnBroadcastEvent;
        Refresh();
    }

    void Refresh()
    {
        if (_init == false)
            return;

        float expNormalized = Managers.Game.GetExpNormalized();

        GetText((int)Texts.GoldCountText).text = Managers.Inventory.GetCurrency(ECurrencyType.Gold).ToString();
        GetText((int)Texts.DiaCountText).text = Managers.Inventory.GetCurrency(ECurrencyType.Dia).ToString();
        GetText((int)Texts.MeatCountText).text = Managers.Inventory.GetCurrency(ECurrencyType.Meat).ToString();
        GetText((int)Texts.WoodCountText).text = Managers.Inventory.GetCurrency(ECurrencyType.Wood).ToString();
        GetText((int)Texts.MineralCountText).text = Managers.Inventory.GetCurrency(ECurrencyType.Mineral).ToString();
        GetText((int)Texts.LevelText).text = Managers.Game.PlayerLevel.ToString();

        int battlePower = (int)Managers.Hero.GetBattlePower();
        GetText((int)Texts.BattlePowerText).text = battlePower.ToString();

        GetSlider((int)Sliders.MineralSlider).value = (float)Managers.Inventory.GetCurrency(ECurrencyType.Mineral) / Managers.Inventory.GetMax(ECurrencyType.Mineral);
        GetSlider((int)Sliders.MeatSlider).value = (float)Managers.Inventory.GetCurrency(ECurrencyType.Meat) / Managers.Inventory.GetMax(ECurrencyType.Meat);
        GetSlider((int)Sliders.WoodSlider).value = (float)Managers.Inventory.GetCurrency(ECurrencyType.Wood) / Managers.Inventory.GetMax(ECurrencyType.Wood);
        GetSlider((int)Sliders.PlayerExpSlider).value = expNormalized;

        RefreshQuest();

    }

    void RefreshQuest()
    {
        //Quest
        _mainQuest = Managers.Quest.MainQuest;
        _questTask = _mainQuest.GetCurrentTask();

        if (_mainQuest.State == EQuestState.Rewarded)
        {
            GetObject((int)GameObjects.QuestCompleteInfo).SetActive(true);
            GetObject((int)GameObjects.QuestProgressInfo).SetActive(false);
            GetText((int)Texts.RewardCountText).text = "";
            GetText((int)Texts.QuestRewardText).text = "@@모든퀘스트 완료";
            Sprite iconSprite = Managers.Resource.Load<Sprite>(_questTask.TaskData.RewardIcon);
            GetImage((int)Images.QuestRewardImage).sprite = iconSprite;
            //GetButton((int)Buttons.QuestRewardButton).gameObject.SetActive(false);
            return;
        }

        if (_questTask.TaskState == EQuestState.Completed)
        {
            GetObject((int)GameObjects.QuestCompleteInfo).SetActive(true);
            GetObject((int)GameObjects.QuestProgressInfo).SetActive(false);
            GetText((int)Texts.RewardCountText).text = _questTask.TaskData.RewardCount.ToString();
            //GetButton((int)Buttons.QuestRewardButton).gameObject.SetActive(true);
            GetText((int)Texts.QuestRewardText).text = "@@보상받기";
            Sprite iconSprite = Managers.Resource.Load<Sprite>(_questTask.TaskData.RewardIcon);
            GetImage((int)Images.QuestRewardImage).sprite = iconSprite;
        }
        else// EQuestState.Processing
        {
            GetObject((int)GameObjects.QuestCompleteInfo).SetActive(false);
            GetObject((int)GameObjects.QuestProgressInfo).SetActive(true);

            Sprite iconSprite = Managers.Resource.Load<Sprite>(_questTask.TaskData.ObjectiveIcon);
            GetImage((int)Images.QuestTargetImage).sprite = iconSprite;

            GetText((int)Texts.QuestDescriptionText).text = _questTask.TaskData.DescriptionTextId;
            GetSlider((int)Sliders.QuestProgressSlider).minValue = 0;
            GetSlider((int)Sliders.QuestProgressSlider).maxValue = _questTask.TaskData.ObjectiveCount;
            GetSlider((int)Sliders.QuestProgressSlider).value = _questTask.Count;
            GetText((int)Texts.QuestProgressValueText).text = $"{_questTask.Count} / {_questTask.TaskData.ObjectiveCount}";
        }
    }

    public void HideUIOnMove()
    {
        GetObject((int)GameObjects.TabButtonGroup).SetActive(false);
    }

    #region ClickEvent

    void OnClickGoldPlusButton()
    {
        //test
        Managers.Inventory.EarnCurrency(ECurrencyType.Gold, 20);
        Debug.Log("On Click GoldPlusButton");
    }

    void OnClickDiaPlusButton()
    {
        Managers.Inventory.EarnCurrency(ECurrencyType.Dia, 20);
        Debug.Log("On Click DiaPlusButton");
    }

    void OnClickSetHeroesButton()
    {
        UI_SetHeroesPopup popup = Managers.UI.ShowPopupUI<UI_SetHeroesPopup>();
        popup.SetInfo();
    }

    void OnClickSettingButton()
    {
        UI_SettingPopup popup = Managers.UI.ShowPopupUI<UI_SettingPopup>();
        popup.SetInfo();
    }

    void OnClickQuestButton()
    {
        Debug.Log("On Click QuestButton");
        UI_QuestPopup popup = Managers.UI.ShowPopupUI<UI_QuestPopup>();
        popup.SetInfo();
    }

    void OnClickCampButton()
    {
        Debug.Log("On Click CampButton");
    }

    void OnClickPortalButton()
    {
        Managers.Object.HeroCamp.OpenPortal();
        Debug.Log("On Click PortalButton");
    }

    void OnClickCheatButton()
    {
        //UI_AddMonsterPopup_Cheat popup =  Managers.UI.ShowPopupUI<UI_AddMonsterPopup_Cheat>();
        Managers.UI.ShowPopupUI<UI_CheatListPopup_Cheat>();
        //popup.SetInfo();
    }

    void OnClickHeroesButton()
    {
        UI_HeroesPopup popup = Managers.UI.ShowPopupUI<UI_HeroesPopup>();
        popup.SetInfo();
    }

    void OnClickInventoryButton()
    {
        UI_InventoryPopup popup = Managers.UI.ShowPopupUI<UI_InventoryPopup>();
        popup.SetInfo();
    }

    void OnClickWorldmapButton()
    {
        UI_WorldmapPopup popup = Managers.UI.ShowPopupUI<UI_WorldmapPopup>();
        popup.SetInfo();
    }

    void OnClickChallengeButton()
    {
        UI_ChallengePopup popup = Managers.UI.ShowPopupUI<UI_ChallengePopup>();
    }

    void OnClickShopButton()
    {
        Debug.Log("On Click Shop Toggle");
    }

    void OnClickTabButton()
    {
        bool isActive = GetObject((int)GameObjects.TabButtonGroup).activeSelf;
        GetObject((int)GameObjects.TabButtonGroup).SetActive(!isActive);

        if (isActive)
        {
            GetButton((int)Buttons.OpenTabButton).gameObject.SetActive(true);
            GetButton((int)Buttons.CloseTabButton).gameObject.SetActive(false);
        }
        else
        {
            GetButton((int)Buttons.OpenTabButton).gameObject.SetActive(false);
            GetButton((int)Buttons.CloseTabButton).gameObject.SetActive(true);
        }
    }

    void OnClickReward()
    {
        //Quest
        _questTask.GiveReward();
        Refresh();

    }

    void OnClickMoveToQuest()
    {
        Quest mainQuest = Managers.Quest.MainQuest;
        QuestTask questTask = mainQuest.GetCurrentTask();

        // Npc를 찾는 경우
        if (questTask.TaskData.RewardType == EQuestRewardType.Hero)
        {
            FindAndTargetNpc(_questTask.TaskData.TemplateId);
            return;
        }

        switch (questTask.TaskData.ObjectiveType)
        {
            case EQuestObjectiveType.ClearDungeon:
                FindAndTargetNpc(questTask.TaskData.TemplateId);
                break;

            case EQuestObjectiveType.KillMonster:
            case EQuestObjectiveType.EarnMeat:
                FindAndTargetMonster(questTask.TaskData.ObjectiveDataId);
                break;

            case EQuestObjectiveType.EarnWood:
                FindAndTargetEnvironment(EEnvType.Wood);
                break;

            case EQuestObjectiveType.EarnMineral:
                FindAndTargetEnvironment(EEnvType.Mineral);
                break;

            default:
                //TODO
                Debug.Log("Cannot Find Target");
                break;
        }
    }

    #endregion

    void FindAndTargetNpc(int templateId)
    {
        var target = Managers.Object.Npcs.FirstOrDefault(x => x.Data.QuestTaskDataId == templateId);
        if (target != null)
            Managers.Game.Cam.TargetingCamera(target);
    }

    void FindAndTargetMonster(int templateId)
    {
        var target = Managers.Object.Monsters
            .Where(x => x.TemplateId == templateId)
            .OrderBy(monster => (Managers.Object.HeroCamp.Position - monster.Position).sqrMagnitude)
            .FirstOrDefault();

        if (target != null)
            Managers.Game.Cam.TargetingCamera(target);
    }

    void FindAndTargetEnvironment(EEnvType envType)
    {
        var target = Managers.Object.Envs
            .Where(x => x.EnvData.EnvType == envType)
            .OrderBy(env => (Managers.Object.HeroCamp.Position - env.Position).sqrMagnitude)
            .FirstOrDefault();

        if (target != null)
            Managers.Game.Cam.TargetingCamera(target);
    }

    void HandleOnBroadcastEvent(EBroadcastEventType type, ECurrencyType currencyType, int value)
    {
        switch (type)
        {
            case EBroadcastEventType.ChangeCurrency:
            case EBroadcastEventType.HeroLevelUp:
            case EBroadcastEventType.ChangeInventory:
            case EBroadcastEventType.ChangeTeam:
            case EBroadcastEventType.KillMonster:
                Refresh();
                break;
        }
    }

    void OnClickStartCampModeButton()
    {
        Managers.Object.HeroCamp.StartCampMode();
        GetButton((int)Buttons.StartCampModeButton).gameObject.SetActive(false);
        GetButton((int)Buttons.EndCampModeButton).gameObject.SetActive(true);
    }

    void OnClickEndCampModeButton()
    {
        Managers.Object.HeroCamp.EndCampMode();
        GetButton((int)Buttons.StartCampModeButton).gameObject.SetActive(true);
        GetButton((int)Buttons.EndCampModeButton).gameObject.SetActive(false);
    }
}