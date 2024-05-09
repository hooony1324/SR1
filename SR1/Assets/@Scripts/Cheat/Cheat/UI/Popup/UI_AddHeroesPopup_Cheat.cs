using Data;
using System.Collections.Generic;

public class UI_AddHeroesPopup_Cheat : UI_Popup
{
    enum GameObjects
    {
        AddHeroesList,
        CloseArea,
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

    public void SetInfo()
    {
        Refresh();
    }

    void Refresh()
    {
        if (_init == false)
            return;


        foreach(KeyValuePair<int, HeroData> dict in Managers.Data.HeroDic)
        {
            UI_AddHeroes_HeroItem_Cheat item = Managers.UI.MakeSubItem<UI_AddHeroes_HeroItem_Cheat>(GetObject((int)GameObjects.AddHeroesList).transform);
            item.SetInfo(dict.Value);
        }
    }

    void OnClickCloseButton()
    {
        Managers.UI.ClosePopupUI();
    }

    void OnClickCloseArea()
    {
        Managers.UI.ClosePopupUI();
    }
}
