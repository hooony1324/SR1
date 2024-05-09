public class UI_ChallengePopup : UI_Popup
{
    enum GameObjects
    {
        ChallengeList,
    }

    enum Texts
    {
        TitleText,
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
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);

        Refresh();

        return false;
    }

    public void SetInfo()
    {

    }

    void Refresh()
    {

    }

    void OnClickCloseButton()
    {
        ClosePopupUI();
    }
}
