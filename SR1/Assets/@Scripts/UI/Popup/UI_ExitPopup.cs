using UnityEngine;

public class UI_ExitPopup : UI_Popup
{
    enum GameObjects
    {
        CloseArea,
    }

    enum Texts
    {
        ExitConfirmationText,
        ExitText,
        CancelText,
    }

    enum Buttons
    {
        ExitButton,
        CancelButton,
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        GetObject((int)GameObjects.CloseArea).BindEvent(OnClickCloseArea);
        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnClickExitButton);
        GetButton((int)Buttons.CancelButton).gameObject.BindEvent(OnClickCancelButton);

        return true;
    }

    void OnClickCloseArea()
    {
        ClosePopupUI();
    }

    void OnClickExitButton()
    {
        Application.Quit();
    }

    void OnClickCancelButton()
    {
        ClosePopupUI();
    }
}
