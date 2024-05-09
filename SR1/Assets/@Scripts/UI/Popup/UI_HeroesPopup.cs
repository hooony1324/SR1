using Spine.Unity;
using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_HeroesPopup : UI_Popup
{
    enum GameObjects
    {
        PickedHeroesList,
        OwnedHeroesList,
        Content
    }

    enum Texts
    {
        AutoEquipText, //현지화 용
        OwnedHeroesCountText,
        NameText,
        LevelText,
        ExpText,
        DamageText,
        HpText,
        BattlePowerText,
        Skill1NameText,
        Skill2NameText,
        LevelUpCostText,
        PickedHeroesCountText,
        LevelUpText, //현지화 용
        EquipText, //현지화 용
        UnequipText, //현지화 용
        BattlePowerNameText,
        DamageNameText,
        HPNameText,
        MeatCountText
    }

    enum Images
    {
    }

    enum Buttons
    {
        EquipButton,
        UnequipButton,
        LevelUpButton,
        Skill1Button,
        Skill2Button,
        AutoEquipButton,
        CloseButton,
    }

    enum Sliders
    {
        ExpSlider,
    }

    [SerializeField] SkeletonGraphic _selectedHeroSpine;

    int _heroTemplateId = -1;

    const int MAX_ITEM_COUNT = 40;
    List<UI_HeroesPopup_HeroItem> _pickedHeroes = new List<UI_HeroesPopup_HeroItem>();
    List<UI_HeroesPopup_HeroItem> _unpickedHeroes = new List<UI_HeroesPopup_HeroItem>();

    private HeroInfo _heroInfo;

    private ScrollRect _scrollRect;
    
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        BindSlider(typeof(Sliders));

        Managers.Game.OnBroadcastEvent -= HandleOnBroadcast;
        Managers.Game.OnBroadcastEvent += HandleOnBroadcast;

        Transform parent = GetObject((int)GameObjects.PickedHeroesList).transform;
        for (int i = 0; i < MAX_ITEM_COUNT; i++)
        {
            UI_HeroesPopup_HeroItem item = Managers.UI.MakeSubItem<UI_HeroesPopup_HeroItem>(parent);
            _pickedHeroes.Add(item);
        }

        parent = GetObject((int)GameObjects.OwnedHeroesList).transform;
        for (int i = 0; i < MAX_ITEM_COUNT; i++)
        {
            UI_HeroesPopup_HeroItem item = Managers.UI.MakeSubItem<UI_HeroesPopup_HeroItem>(parent);
            _unpickedHeroes.Add(item);
        }

        GetButton((int)Buttons.EquipButton).gameObject.BindEvent(OnClickEquipButton);
        GetButton((int)Buttons.UnequipButton).gameObject.BindEvent(OnClickUnequipButton);
        GetButton((int)Buttons.LevelUpButton).gameObject.BindEvent(OnClickLevelUpButton);
        GetButton((int)Buttons.Skill1Button).gameObject.BindEvent(OnClickSkill1Button);
        GetButton((int)Buttons.Skill2Button).gameObject.BindEvent(OnClickSkill2Button);
        GetButton((int)Buttons.AutoEquipButton).gameObject.BindEvent(OnClickAutoEquipButton);
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);

        _selectedHeroSpine = Util.FindChild<SkeletonGraphic>(gameObject, "SelectedHeroSpine", true);

        Refresh();

        return true;
    }

    public void SetInfo(int heroTemplateId = 0)
    {
        _heroTemplateId = heroTemplateId;

        if (heroTemplateId == 0)
        {
            _heroTemplateId = Managers.Hero.PickedHeroes[0].TemplateId;
        }

        _heroInfo = Managers.Hero.GetHeroInfo(_heroTemplateId);
        Refresh();
    }

    void Refresh()
    {
        if (_init == false)
            return;

        if (_heroInfo == null)
            return;

        if (Managers.Data.HeroDic.ContainsKey(_heroTemplateId) == false)
            return;

        //스켈레톤
        _selectedHeroSpine.skeletonDataAsset =
            Managers.Resource.Load<SkeletonDataAsset>(_heroInfo.HeroData.SkeletonDataID);
        _selectedHeroSpine.Initialize(true);

        //재화
        GetText((int)Texts.MeatCountText).text = Managers.Inventory.GetCurrency(ECurrencyType.Meat).ToString();

        //이름
        GetText((int)Texts.NameText).text = Managers.GetText(_heroInfo.HeroInfoData.NameTextId);

        //스킬이름
        if (Managers.Data.SkillDic.TryGetValue(Managers.Data.HeroDic[_heroTemplateId].SkillAId, out SkillData skillAData))
            GetText((int)Texts.Skill1NameText).text = Managers.GetText(skillAData.NameTextId);
        if (Managers.Data.SkillDic.TryGetValue(Managers.Data.HeroDic[_heroTemplateId].SkillBId, out SkillData skillBData))
            GetText((int)Texts.Skill2NameText).text = Managers.GetText(skillBData.NameTextId);



        //전투정보
        GetText((int)Texts.BattlePowerNameText).text = "@@전투력";
        GetText((int)Texts.DamageNameText).text = "@@공격력";
        GetText((int)Texts.HPNameText).text = "@@체력";
        GetText((int)Texts.BattlePowerText).text = _heroInfo.CombatPower.ToString();
        GetText((int)Texts.DamageText).text = _heroInfo.Atk.ToString();
        GetText((int)Texts.HpText).text = _heroInfo.MaxHp.ToString();

        //레벨 , 경험치
        int level = _heroInfo.Level;
        GetText((int)Texts.LevelText).text = $"{_heroInfo.Level}";
        int requireExp = (level - 1) / 10 * 5 + 5;
        GetText((int)Texts.ExpText).text = $"{_heroInfo.Exp}/{requireExp}";
        int levelUpCost = level * 5;
        GetText((int)Texts.LevelUpCostText).text = $"{levelUpCost}";
        float ratio = _heroInfo.Exp / requireExp;
        GetSlider((int)Sliders.ExpSlider).value = ratio;

        //참전중인 영웅        
        GetText((int)Texts.PickedHeroesCountText).text =
            $"{Managers.Hero.PickedHeroes.Count} / {Managers.Game.MaxTeamCount}";

        //대기중인 영웅
        GetText((int)Texts.OwnedHeroesCountText).text =
            $"{Managers.Hero.OwnedHeroes.Count} / {Managers.Hero.AllHeroInfos.Keys.Count}";

        Refresh_EquipButton();

        Refresh_Hero(_pickedHeroes, Define.HeroOwningState.Picked);
        Refresh_Hero(_unpickedHeroes, Define.HeroOwningState.Owned);

        //TODO 만렙인 경우
        if(_heroInfo.IsMaxLevel())
            GetButton((int)Buttons.LevelUpButton).gameObject.SetActive((false));


        // 리프레시 버그 대응
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetObject((int)GameObjects.PickedHeroesList)
            .GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetObject((int)GameObjects.OwnedHeroesList)
            .GetComponent<RectTransform>());
    }

    void Refresh_Hero(List<UI_HeroesPopup_HeroItem> list, Define.HeroOwningState owningState)
    {
        List<HeroInfo> heroes;
        if (owningState == HeroOwningState.Picked)
            heroes = Managers.Hero.PickedHeroes;
        else
        {
            heroes = Managers.Hero.OwnedHeroes;
            heroes.AddRange(Managers.Hero.UnownedHeroes);
        }
        for (int i = 0; i < list.Count; i++)
        {
            if (i < heroes.Count)
            {
                HeroInfo hero = heroes[i];
                list[i].SetInfo(hero.TemplateId, this);
                list[i].gameObject.SetActive(true);
            }
            else
            {
                list[i].gameObject.SetActive(false);
            }
        }
    }

    void Refresh_SelectedHeroItem()
    {
        UI_HeroesPopup_HeroItem heroItem = _pickedHeroes.Find(hero => hero.HeroTemplateId == _heroTemplateId);
        if (heroItem != null)
        {
            heroItem.SetInfo();
        }
    }

    private Vector2 _lastScrollPos;

    public void SetRectPosition()
    {
        RectTransform rectTransform = GetObject((int)GameObjects.Content).GetComponent<RectTransform>();
        _lastScrollPos = rectTransform.anchoredPosition;
        rectTransform.anchoredPosition = Vector2.zero;
    }

    //TODO 기획논의필요
    public void SetRectLastPosition()
    {
        // RectTransform rectTransform = GetObject((int)GameObjects.Content).GetComponent<RectTransform>();
        // rectTransform.anchoredPosition = _lastScrollPos;
    }
    
    void Refresh_EquipButton()
    {
        if (_heroInfo == null)
            return;

        if (_heroInfo.OwningState == HeroOwningState.Picked)
        {
            GetButton((int)Buttons.EquipButton).gameObject.SetActive(false);
            GetButton((int)Buttons.UnequipButton).gameObject.SetActive(true);
        }
        else if (_heroInfo.OwningState == HeroOwningState.Owned)
        {
            GetButton((int)Buttons.EquipButton).gameObject.SetActive(true);
            GetButton((int)Buttons.UnequipButton).gameObject.SetActive(false);
        }
        else
        {
            GetButton((int)Buttons.EquipButton).gameObject.SetActive(false);
            GetButton((int)Buttons.UnequipButton).gameObject.SetActive(false);
        }
    }

    void OnClickEquipButton()
    {
        if (_heroInfo == null)
            return;

        if (_heroInfo.OwningState != HeroOwningState.Owned)
        {
            return;
        }

        if (Managers.Hero.CanPick() == false)
        {
            Managers.UI.ShowToast("@@최대인원 초과");
            return;
        }


        Managers.Hero.PickHero(_heroTemplateId, Vector3Int.zero);
        Refresh_SelectedHeroItem();
        Refresh_EquipButton();
        SetRectPosition();
    }

    void OnClickUnequipButton()
    {
        if (_heroInfo == null)
            return;

        if (_heroInfo.OwningState != HeroOwningState.Picked)
        {
            return;
        }

        if(Managers.Hero.PickedHeroes.Count < 2)
            return;

        Managers.Hero.UnpickHero(_heroTemplateId);
        Refresh_SelectedHeroItem();
        Refresh_EquipButton();
        SetRectLastPosition();
    }

    void OnClickLevelUpButton()
    {
        if (_heroInfo == null)
            return;

        if (_heroInfo.CanLevelUp() == false)
        {
            Managers.UI.ShowToast("@@ 카드가 부족합니다.");
            return;
        }

        int meat = _heroInfo.GetExpToNextLevel();

        if (Managers.Inventory.SpendCurrency(ECurrencyType.Meat, meat) == false)
        {
            Managers.UI.ShowToast("@@ 고기가 부족합니다.");
            return;
        }
        
        _heroInfo.TryLevelUp();

        Refresh();
    }

    void OnClickSkill1Button()
    {
        UI_SkillInfoPopup popup = Managers.UI.ShowPopupUI<UI_SkillInfoPopup>();
        popup.SetInfo(_heroInfo.HeroData.SkillAId,_heroInfo.ASkillDataId, _heroInfo.ASkillLevel);
    }

    void OnClickSkill2Button()
    {
        UI_SkillInfoPopup popup = Managers.UI.ShowPopupUI<UI_SkillInfoPopup>();
        popup.SetInfo(_heroInfo.HeroData.SkillBId,_heroInfo.BSkillDataId, _heroInfo.BSkillLevel);
    }

    void OnClickAutoEquipButton()
    {
        Debug.Log("OnClickAutoEquipButton");
    }

    void OnClickCloseButton()
    {
        ClosePopupUI();
    }

    private void HandleOnBroadcast(EBroadcastEventType type, ECurrencyType currencyType, int value)
    {
        switch (type)
        {
            case EBroadcastEventType.ChangeTeam:
                Refresh();
                break;
        }
    }

}