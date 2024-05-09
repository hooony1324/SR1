public class UI_MapButton_Cheat : UI_Base
{
    UI_MapList_Cheat _mapListUI;
    enum Buttons
    {
        MapButton,
    }

    enum Texts
    {
        MapText,
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        GetButton((int)Buttons.MapButton).gameObject.BindEvent(OnClickMapButton);

        return true;
    }


    public void SetInfo(string mapName, UI_MapList_Cheat ui)
    {
        _mapListUI = ui;
        GetText((int)Texts.MapText).text = mapName;
    }

    void OnClickMapButton()
    {
        _mapListUI.CreateMap(GetText((int)Texts.MapText).text);
    }
}