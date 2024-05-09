using Spine.Unity;
using UnityEngine;
using static Define;

public class UI_HeroesPopup_HeroItem : UI_SubItem
{
    enum GameObjects
    {
        PickedHeroObject,
        UnownedHeroObject,
    }
    enum Buttons
    {
        HeroButton,
    }

    enum Images
    {
        HeroImage,
        CheckImage,
    }

    enum Texts
    {
        ExpText,
        LevelText,
    }

    enum Sliders
    {
        HeroExpSlider,
    }

    int _heroTemplateId = -1;
    public int HeroTemplateId { get { return _heroTemplateId; } }

    UI_HeroesPopup _heroesPopupUI;
    private SkeletonAnimation _seletedAnim;
    
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));
        BindSlider(typeof(Sliders));
        BindImage(typeof(Images));

        GetButton((int)Buttons.HeroButton).gameObject.BindEvent(OnClickHeroButton);
        GetButton((int)Buttons.HeroButton).gameObject.BindEvent(null, OnBeginDrag, Define.UIEvent.BeginDrag);
        GetButton((int)Buttons.HeroButton).gameObject.BindEvent(null, OnDrag, Define.UIEvent.Drag);
        GetButton((int)Buttons.HeroButton).gameObject.BindEvent(null, OnEndDrag, Define.UIEvent.EndDrag);

        Refresh();

        return true;
    }

    public void SetInfo(int heroDataId, UI_HeroesPopup popup)
    {
        _heroTemplateId = heroDataId;
        _heroesPopupUI = popup;

        Refresh();
    }

    public void SetInfo()
    {
        Refresh();
    }

    void Refresh()
    {
        if (_init == false)
            return;

        if (_heroesPopupUI == null)
            return;

        HeroInfo heroInfo = Managers.Hero.GetHeroInfo(_heroTemplateId);
        if (heroInfo == null)
            return;
        
        // HeroImage
        GetImage((int)Images.HeroImage).sprite = Managers.Resource.Load<Sprite>(heroInfo.HeroData.IconImage);

        //레벨
        int level = heroInfo.Level;
        GetText((int)Texts.LevelText).text = $"{level}";
        int requireExp = (level - 1) / 10 * 5 + 5;
        GetText((int)Texts.ExpText).text = $"{heroInfo.Exp}/{requireExp}";
        float ratio = heroInfo.Exp / requireExp;
        GetSlider((int)Sliders.HeroExpSlider).value = ratio;
        
        if (heroInfo.OwningState == HeroOwningState.Unowned)
        {
            GetObject((int)GameObjects.PickedHeroObject).SetActive(false);
            GetObject((int)GameObjects.UnownedHeroObject).SetActive(true);
        }
        else if(heroInfo.OwningState == HeroOwningState.Picked)
        {
            GetObject((int)GameObjects.PickedHeroObject).SetActive(true);
            GetObject((int)GameObjects.UnownedHeroObject).SetActive(false);
        }
        else
        {
            GetObject((int)GameObjects.PickedHeroObject).SetActive(false);
            GetObject((int)GameObjects.UnownedHeroObject).SetActive(false);
        }
    }

    void OnClickHeroButton()
    {
        if (_heroTemplateId < 0)
            return;

        _heroesPopupUI.SetInfo(_heroTemplateId);
    }

    private void OnDisable()
    {
        _heroTemplateId = -1;
    }
   

}
