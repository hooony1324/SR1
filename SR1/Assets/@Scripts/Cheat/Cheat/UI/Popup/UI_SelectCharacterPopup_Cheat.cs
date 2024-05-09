using Data;
using System.Collections.Generic;

public class UI_SelectCharacterPopup_Cheat : UI_Popup
{
    enum GameObjects
    {
        CloseArea,
        AnimationList,
    }

    enum Buttons
    {
        CloseButton,
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));

        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);
        GetObject((int)GameObjects.CloseArea).BindEvent(OnClickCloseArea);

        Refresh();

        return true;
    }


    void Refresh()
    {
        if (_init == false)
            return;

        foreach (KeyValuePair<int, MonsterData> dict in Managers.Data.MonsterDic)
        {
            UI_SelectCharacterItem_Cheat item = Managers.UI.MakeSubItem<UI_SelectCharacterItem_Cheat>(GetObject((int)GameObjects.AnimationList).transform);
            item.SetInfo(dict.Value, this);
        }

        foreach (KeyValuePair<int, HeroData> dict in Managers.Data.HeroDic)
        {
            UI_SelectCharacterItem_Cheat item = Managers.UI.MakeSubItem<UI_SelectCharacterItem_Cheat>(GetObject((int)GameObjects.AnimationList).transform);
            item.SetInfo(dict.Value, this);
        }
    }

    public void SetAnimation(CreatureData creatureData)
    {
        (Managers.Scene.CurrentScene as ArtTestScene).SetCharacter(creatureData);
    }

    void OnClickCloseArea()
    {
        Managers.UI.ClosePopupUI(this);
    }

    void OnClickCloseButton()
    {
        Managers.UI.ClosePopupUI(this);
    }
}
