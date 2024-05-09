using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_HireHeroesPopup : UI_Popup
{
    enum Texts
    {
        BonusCountText,

        //-- 순서바꾸지말기
        Hero1ExpText,
        Hero2ExpText,
        Hero3ExpText,
        Hero1LevelText,
        Hero2LevelText,
        Hero3LevelText,
        GachaExpCount1Text,
        GachaExpCount2Text,
        GachaExpCount3Text,
        //-- 순서바꾸지말기

        HireCostText,
        RefreshCostText,
    }

    enum Buttons
    {
        CloseButton,
        HireButton,
        RefreshButton,
    }

    enum Toggles
    {
        Multiplier1Toggle,
        Multiplier2Toggle,
        Multiplier3Toggle,
    }

    enum Images
    {
        BonusHeroImage,
        Hero1Image,
        Hero2Image,
        Hero3Image,
        SoldOut1Image,
        SoldOut2Image,
        SoldOut3Image,
        HireCostIconImage,
        RefreshCostIconImage
    }

    enum Sliders
    {
        BonusSlider,
        Hero1ExpSlider,
        Hero2ExpSlider,
        Hero3ExpSlider,
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        BindToggle(typeof(Toggles));
        BindImage(typeof(Images));
        BindSlider(typeof(Sliders));

        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);
        GetButton((int)Buttons.HireButton).gameObject.BindEvent(OnClickHireButton);
        GetButton((int)Buttons.RefreshButton).gameObject.BindEvent(OnClickRefreshButton);
        GetToggle((int)Toggles.Multiplier1Toggle).gameObject.BindEvent(OnClickMultiplier1Toggle);
        GetToggle((int)Toggles.Multiplier2Toggle).gameObject.BindEvent(OnClickMultiplier2Toggle);
        GetToggle((int)Toggles.Multiplier3Toggle).gameObject.BindEvent(OnClickMultiplier3Toggle);

        GetImage((int)Images.SoldOut1Image).gameObject.SetActive(false);
        GetImage((int)Images.SoldOut2Image).gameObject.SetActive(false);
        GetImage((int)Images.SoldOut3Image).gameObject.SetActive(false);

        return true;
    }

    bool _isOnMultiplier1 = true;
    bool _isOnMultiplier2 = false;
    bool _isOnMultiplier3 = false;

    //서버로부터 현재 뽑기 상태 내려 받아서 처리해야함
    public void SetInfo()
    {
        //현재 가챠 리스트
        //가챠 Refresh까지 남은 시간
        //이미 뽑힌 영웅

        if(Managers.Game.GachaList[0].GachaDataId == 0)
            RefreshGachaList();
        Refresh();
    }

    void Refresh()
    {
        if (_init == false)
            return;


        // 고용 보너스
        Slider bonusSlider = GetSlider((int)Sliders.BonusSlider);
        bonusSlider.maxValue = 20;
        bonusSlider.value = Managers.Game.HireCount;
        GetText((int)Texts.BonusCountText).text = $"{Managers.Game.HireCount}/20";

        //첫번째 영웅
        {
            int dataId = Managers.Game.GachaList[0].GachaDataId;
            HeroInfo heroInfo = Managers.Hero.GetHeroInfo(dataId);
            if (heroInfo == null)
                return;
            GetText((int)Texts.Hero1ExpText).text = $"{heroInfo.Exp}/{heroInfo.GetExpToNextLevel()}";
            GetText((int)Texts.Hero1LevelText).text = heroInfo.Level.ToString();
            GetText((int)Texts.GachaExpCount1Text).text =$"X{Managers.Data.HeroInfoDic[dataId].GachaExpCount}";
            GetImage((int)Images.Hero1Image).sprite = Managers.Resource.Load<Sprite>(Managers.Data.HeroInfoDic[dataId].IconImage);
            GetImage((int)Images.SoldOut1Image).gameObject.SetActive(Managers.Game.GachaList[0].IsPurchased);
            GetSlider((int)Sliders.Hero1ExpSlider).maxValue = heroInfo.GetExpToNextLevel();
            GetSlider((int)Sliders.Hero1ExpSlider).value = heroInfo.Exp;
        }
        
        //2
        {
            int dataId = Managers.Game.GachaList[1].GachaDataId;
            HeroInfo heroInfo = Managers.Hero.GetHeroInfo(dataId);
            if (heroInfo == null)
                return;
            dataId = Managers.Game.GachaList[1].GachaDataId;
            GetText((int)Texts.Hero2ExpText).text = $"{heroInfo.Exp}/{heroInfo.GetExpToNextLevel()}";
            GetText((int)Texts.Hero2LevelText).text = heroInfo.Level.ToString();
            GetText((int)Texts.GachaExpCount2Text).text = Managers.Data.HeroInfoDic[dataId].GachaExpCount.ToString();
            GetImage((int)Images.Hero2Image).sprite =
                Managers.Resource.Load<Sprite>(Managers.Data.HeroInfoDic[dataId].IconImage);
            GetImage((int)Images.SoldOut2Image).gameObject.SetActive(Managers.Game.GachaList[1].IsPurchased);
            GetSlider((int)Sliders.Hero3ExpSlider).maxValue = heroInfo.GetExpToNextLevel();
            GetSlider((int)Sliders.Hero3ExpSlider).value = heroInfo.Exp;
            
        }
        
        //3
        {
            int dataId = Managers.Game.GachaList[2].GachaDataId;
            HeroInfo heroInfo = Managers.Hero.GetHeroInfo(dataId);
            if (heroInfo == null)
                return;
            GetText((int)Texts.Hero3ExpText).text = heroInfo.Exp.ToString();
            GetText((int)Texts.Hero3LevelText).text = heroInfo.Level.ToString();
            GetText((int)Texts.GachaExpCount3Text).text = Managers.Data.HeroInfoDic[dataId].GachaExpCount.ToString();
            GetImage((int)Images.Hero3Image).sprite = Managers.Resource.Load<Sprite>(Managers.Data.HeroInfoDic[dataId].IconImage);
            GetImage((int)Images.SoldOut3Image).gameObject.SetActive(Managers.Game.GachaList[2].IsPurchased);
            GetSlider((int)Sliders.Hero3ExpSlider).maxValue = heroInfo.GetExpToNextLevel();
            GetSlider((int)Sliders.Hero3ExpSlider).value = heroInfo.Exp;
        }
        
        //cost
        Sprite gold = Managers.Resource.Load<Sprite>("Gold");
        GetImage((int)Images.HireCostIconImage).sprite = gold;
        GetImage((int)Images.RefreshCostIconImage).sprite = gold;
        GetText((int)Texts.HireCostText).text = $"{GACHA_COST}";
        GetText((int)Texts.RefreshCostText).text = $"{REFRESH_GACHA_COST}";
    }

    void OnClickCloseButton()
    {
        Managers.UI.ClosePopupUI(this);
    }

    void OnClickHireButton()
    {
        int multiple = 1;
        if (_isOnMultiplier1)
            multiple = 1;
        else if (_isOnMultiplier2)
            multiple = 2;
        else if (_isOnMultiplier3)
            multiple = 3;

        int cost = GACHA_COST * multiple;

        if (Managers.Inventory.SpendCurrency(ECurrencyType.Gold, cost) == false)
        {
            return;
        }

        int gachaNumber = Managers.Game.Gacha(multiple);

        if (gachaNumber < 0)
            return;



        //gachaNumber만 UI 애니메이션 추가하여 연출
        //임시로 그냥 Refresh때림
        Refresh();
    }

    void OnClickRefreshButton()
    {
        //재화 소모
        if (Managers.Inventory.SpendCurrency(ECurrencyType.Gold, REFRESH_GACHA_COST) == false)
            return;

        RefreshGachaList();

        Refresh();
    }

    void RefreshGachaList()
    {
        Managers.Game.RefreshGachaList();
    }

    void OnClickMultiplier1Toggle()
    {
        ResetMultiplierToggle();

        _isOnMultiplier1 = true;
    }

    void OnClickMultiplier2Toggle()
    {
        ResetMultiplierToggle();

        _isOnMultiplier2 = true;
    }

    void OnClickMultiplier3Toggle()
    {
        ResetMultiplierToggle();

        _isOnMultiplier3 = true;
    }

    void ResetMultiplierToggle()
    {
        _isOnMultiplier1 = false;
        _isOnMultiplier2 = false;
        _isOnMultiplier3 = false;
    }
}