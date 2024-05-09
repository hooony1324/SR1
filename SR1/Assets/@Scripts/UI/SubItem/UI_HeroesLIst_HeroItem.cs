using UnityEngine;

public class UI_HeroesList_HeroItem : UI_SubItem
{
    enum Buttons
    {
        HeroButton,
    }

    enum Images
    {
        HeroImage,
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

    int _heroDataId = -1;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));
        BindText(typeof(Texts));
        BindSlider(typeof(Sliders));
        BindImage(typeof(Images));

        GetButton((int)Buttons.HeroButton).gameObject.BindEvent(OnClickHeroButton);

        Refresh();

        return true;
    }

    public void SetInfo(int heroDataId)
    {
        _heroDataId = heroDataId;

        Refresh();
    }

    void Refresh()
    {
        if (_init == false)
            return;

        if (_heroDataId < 0)
            return;

        GetImage((int)Images.HeroImage).sprite = Managers.Resource.Load<Sprite>(Managers.Data.HeroDic[_heroDataId].IconImage);
    }

    void OnClickHeroButton()
    {
        UI_HeroInfoPopup popup = Managers.UI.ShowPopupUI<UI_HeroInfoPopup>();
        popup.SetInfo(_heroDataId);
    }

    private void OnDisable()
    {
        _heroDataId = -1;
    }
}
