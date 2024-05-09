using UnityEngine;

public class UI_SetHeroesPopup : UI_Popup
{
    enum GameObjects
    {
        CloseArea,
        HeroesList,
    }

    enum Buttons
    {
        CloseButton,
        AutoEquipButton,
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));

        GetObject((int)GameObjects.CloseArea).BindEvent(OnClickCloseArea);
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);
        GetButton((int)Buttons.AutoEquipButton).gameObject.BindEvent(OnClickAutoEquipButton);

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

        Extension.DestroyChilds(GetObject((int)GameObjects.HeroesList));

        foreach (var hero in Managers.Hero.PickedHeroes)
        {
            UI_SetHeroes_HeroItem item = Managers.UI.MakeSubItem<UI_SetHeroes_HeroItem>(GetObject((int)GameObjects.HeroesList).transform);
            item.SetInfo(hero.TemplateId, true);
        }
        
        foreach (var hero in Managers.Hero.AllHeroInfos.Values)
        {
            UI_SetHeroes_HeroItem item = Managers.UI.MakeSubItem<UI_SetHeroes_HeroItem>(GetObject((int)GameObjects.HeroesList).transform);
            item.SetInfo(hero.TemplateId, false);   
        }
    }

    void OnClickCloseArea()
    {
        Managers.UI.ClosePopupUI(this);
    }

    void OnClickCloseButton()
    {
        Managers.UI.ClosePopupUI(this);
    }

    void OnClickAutoEquipButton()
    {
        Debug.Log("On Click AutoEquipButton");
    }
}
