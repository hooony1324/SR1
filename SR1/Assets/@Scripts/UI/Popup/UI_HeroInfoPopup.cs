using Data;
using UnityEngine;
using UnityEngine.UI;

public class UI_HeroInfoPopup : UI_Popup
{
    enum GameObjects
    {
        CloseArea,
        HeroAbilityList,
        ExpSlider
    }

    enum Buttons
    {
        CloseButton,
        LevelUpButton,
        Skill1Button,
        Skill2Button,
    }

    enum Texts
    {
        NameText,
        ExpText,
        LevelText,
        BattlePowerText,
        DamageText,
        HpText,
        Skill1NameText,
        Skill2NameText,
        MeatCountText,
    }

    enum Images
    {
        HeroIconImage,
    }

    int _heroTemplateId = -1;
    private HeroInfo _heroInfo;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));
        BindImage(typeof(Images));

        GetObject((int)GameObjects.CloseArea).BindEvent(OnClickCloseArea);
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);
        GetButton((int)Buttons.LevelUpButton).gameObject.BindEvent(OnClickLevelUpButton);
        GetButton((int)Buttons.Skill1Button).gameObject.BindEvent(OnClickSkill1Button);
        GetButton((int)Buttons.Skill2Button).gameObject.BindEvent(OnClickSkill2Button);

        Refresh();

        return true;
    }

    public void SetInfo(int heroTemplateId)
    {
        _heroTemplateId = heroTemplateId;
        _heroInfo = Managers.Hero.GetHeroInfo(heroTemplateId);
        Refresh();
    }

    void Refresh()
    {
        if (_init == false)
            return;

        if (_heroTemplateId < 0)
            return;
        
        if(_heroInfo == null)
            return;
        HeroData data = _heroInfo.HeroData;

        
        GetImage((int)Images.HeroIconImage).sprite = Managers.Resource.Load<Sprite>(data.IconImage);
        GetText((int)Texts.NameText).text = data.DescriptionTextID;

        #region Level

        GetText((int)Texts.LevelText).text = _heroInfo.Level.ToString();
        GetText((int)Texts.ExpText).text = $"{_heroInfo.Exp} / {_heroInfo.GetExpToNextLevel()}";

        int skillId = data.SkillAId;
        string skillNameId = Managers.Data.SkillDic[skillId].NameTextId;
        GetText((int)Texts.Skill1NameText).text = Managers.GetText(skillNameId); 

        skillId = data.SkillBId;
        skillNameId = Managers.Data.SkillDic[skillId].NameTextId;
        GetText((int)Texts.Skill2NameText).text = Managers.GetText((skillNameId));

        // TODO 레벨에 따른 능력치 적용후 계산되게 해야함
        float atk = data.Atk;
        float hp = data.MaxHp;
        GetText((int)Texts.BattlePowerText).text = (hp + atk * 5).ToString("F0");
        GetText((int)Texts.DamageText).text = atk.ToString("F0");
        GetText((int)Texts.HpText).text = hp.ToString("F0");

        Slider expSlider = GetObject((int)GameObjects.ExpSlider).GetComponent<Slider>();
        expSlider.value = _heroInfo.GetExpNormalized();

        if (_heroInfo.CanLevelUp() == true)
        {
            Debug.Log("레벨업 버튼 활성화");
            GetButton((int)Buttons.LevelUpButton).interactable = true;
        }
        else
        {
            Debug.Log("레벨업 버튼 비활성화");
            GetButton((int)Buttons.LevelUpButton).interactable = false;
        }
        #endregion

        UI_HeroInfo_AbilityItem item = Managers.UI.MakeSubItem<UI_HeroInfo_AbilityItem>(GetObject((int)GameObjects.HeroAbilityList).transform);

    }

    void OnClickCloseArea()
    {
        Managers.UI.ClosePopupUI(this);
    }

    void OnClickCloseButton()
    {
        Managers.UI.ClosePopupUI(this);
    }

    void OnClickLevelUpButton()
    {
        if (_heroInfo.CanLevelUp() == false)
            return;

        int meat = _heroInfo.Level * 5;

        if (Managers.Inventory.SpendCurrency(Define.ECurrencyType.Meat, meat) == false)
            return;

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
}
