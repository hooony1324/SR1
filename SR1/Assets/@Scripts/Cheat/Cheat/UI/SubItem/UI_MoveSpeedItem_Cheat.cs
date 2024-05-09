public class UI_MoveSpeedItem_Cheat : UI_Base
{
    enum Sliders
    {
        MoveSpeedSlider,
    }

    enum Texts
    {
        MoveSpeedText,
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindSlider(typeof(Sliders));
        BindText(typeof(Texts));

        GetSlider((int)Sliders.MoveSpeedSlider).onValueChanged.AddListener((value) =>
        {
            OnMoveSpeedSliderValueChanged(value);
        });

        return true;
    }


    void Refresh()
    {

    }

    void OnMoveSpeedSliderValueChanged(float value)
    {
        GetText((int)Texts.MoveSpeedText).text = value.ToString("F2");
        (Managers.Scene.CurrentScene as ArtTestScene).SetMoveSpeed(value);
    }
}
