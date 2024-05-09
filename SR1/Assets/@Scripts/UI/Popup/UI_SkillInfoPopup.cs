using UnityEngine;

public class UI_SkillInfoPopup : UI_Popup
{
    enum GameObjects
    {
        CloseArea,
    }

    enum Texts
    {
        SkillNameText,
        BasicAbilityText,
        SecondAbilityText,
        ThirdAbilityText,
    }

    enum Images
    {
        SecondAbilityLockImage,
        ThirdAbilityLockImage,
    }

    //baseSkillId
    //currentSkillId
    private int _baseSkillId = -1;
    private int _currentSkillId = -1;
    private int _skillLevel = 0;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        BindImage(typeof(Images));

        GetObject((int)GameObjects.CloseArea).BindEvent(OnClickCloseArea);

        Refresh();

        return true;
    }

    public void SetInfo(int baseSkillId, int currentSkillId, int skillLevel)
    {
        _baseSkillId = baseSkillId;
        _currentSkillId = currentSkillId;
        _skillLevel = skillLevel;

        Refresh();
    }

    void Refresh()
    {
        if (_init == false)
            return;
        if (_currentSkillId == -1)
            return;

        GetText((int)Texts.SkillNameText).text = Managers.GetText(Managers.Data.SkillDic[_baseSkillId].NameTextId);
        GetText((int)Texts.BasicAbilityText).text = "잠금";
        GetText((int)Texts.SecondAbilityText).text = "잠금";
        GetText((int)Texts.ThirdAbilityText).text = "잠금";

        if (_skillLevel == 1)
        {
            GetText((int)Texts.BasicAbilityText).text = Managers.GetText(Managers.Data.SkillDic[_baseSkillId].DescriptionTextId);
        }
        else if (_skillLevel == 2)
        {
            GetText((int)Texts.BasicAbilityText).text = Managers.GetText(Managers.Data.SkillDic[_baseSkillId].DescriptionTextId);
            GetText((int)Texts.SecondAbilityText).text = Managers.GetText(Managers.Data.SkillDic[_currentSkillId].DescriptionTextId);
        }
        else if (_skillLevel >= 3)
        {
            int secondSkillId = Managers.Data.SkillDic[_baseSkillId].NextLevelId;
            GetText((int)Texts.BasicAbilityText).text = Managers.GetText(Managers.Data.SkillDic[_baseSkillId].DescriptionTextId);
            GetText((int)Texts.SecondAbilityText).text = Managers.GetText(Managers.Data.SkillDic[secondSkillId].DescriptionTextId);
            GetText((int)Texts.ThirdAbilityText).text = Managers.GetText(Managers.Data.SkillDic[_currentSkillId].DescriptionTextId);
        }
    }

    void OnClickCloseArea()
    {
        _skillLevel = 0;
        Managers.UI.ClosePopupUI(this);
    }
}