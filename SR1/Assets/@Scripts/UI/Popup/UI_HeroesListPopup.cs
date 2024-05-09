using System.Collections.Generic;

public class UI_HeroesListPopup : UI_Popup
{
    enum GameObjects
    {
        CloseArea,
        EquippedHeroesList,
        WaitingHeroesList,
        UnownedHeroesList,
    }

    enum Texts
    {
        EquippedHeroesCountText,
        WaitingHeroesCountText,
        UnownedHeroesCountText,
    }

    enum Buttons
    {
        CloseButton,
    }

    List<UI_HeroesList_HeroItem> _equippedHeroes = new List<UI_HeroesList_HeroItem>();
    List<UI_HeroesList_HeroItem> _waitingHeroes = new List<UI_HeroesList_HeroItem>();
    List<UI_HeroesList_HeroItem> _unownedHeroes = new List<UI_HeroesList_HeroItem>();

    const int MAX_ITEM_COUNT = 100;
    
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        GetObject((int)GameObjects.CloseArea).BindEvent(OnClickCloseArea);
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);

        {
            var parent = GetObject((int)GameObjects.EquippedHeroesList).transform;
            for (int i = 0; i < MAX_ITEM_COUNT; i++)
            {
                UI_HeroesList_HeroItem item = Managers.UI.MakeSubItem<UI_HeroesList_HeroItem>(parent);
                _equippedHeroes.Add(item);
            }
        }
        {
            var parent = GetObject((int)GameObjects.WaitingHeroesList).transform;
            for (int i = 0; i < MAX_ITEM_COUNT; i++)
            {
                UI_HeroesList_HeroItem item = Managers.UI.MakeSubItem<UI_HeroesList_HeroItem>(parent);
                _waitingHeroes.Add(item);
            }
        }
        {
            var parent = GetObject((int)GameObjects.UnownedHeroesList).transform;
            for (int i = 0; i < MAX_ITEM_COUNT; i++)
            {
                UI_HeroesList_HeroItem item = Managers.UI.MakeSubItem<UI_HeroesList_HeroItem>(parent);
                _unownedHeroes.Add(item);
            }
        }
        
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

        GetText((int)Texts.EquippedHeroesCountText).text = $"{Managers.Hero.PickedHeroes.Count} / ??";
        GetText((int)Texts.WaitingHeroesCountText).text = $"{Managers.Hero.OwnedHeroes.Count} / ??";
        GetText((int)Texts.UnownedHeroesCountText).text = $" / ";

        Refresh_Hero(_equippedHeroes, Define.HeroOwningState.Picked);
        Refresh_Hero(_waitingHeroes, Define.HeroOwningState.Owned);
        Refresh_Hero(_unownedHeroes, Define.HeroOwningState.Unowned);
    }

    void Refresh_Hero(List<UI_HeroesList_HeroItem> list, Define.HeroOwningState owningState)
    {
        List<HeroInfo> heroes = Managers.Hero.OwnedHeroes;

        for (int i = 0; i < list.Count; i++)
        {
            if (i < heroes.Count)
            {
                HeroInfo hero = heroes[i];
                list[i].SetInfo(hero.TemplateId);
                list[i].gameObject.SetActive(true);
            }
            else
            {
                list[i].gameObject.SetActive(false);
            }
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
}
