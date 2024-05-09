using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GameOverPopup : UI_Popup
{
    enum Buttons
    {
        CloseButton,
        RebirthButton,
    }

    private Action<bool> OnClose;
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));

        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);
        GetButton((int)Buttons.RebirthButton).gameObject.BindEvent(OnClickRebirthButton);

        Refresh();

        return true;
    }

    public void SetInfo(Action<bool> action)
    {
        OnClose = action;

        Refresh();
    }

    void Refresh()
    {
        if (_init == false)
            return;

    }

    void OnClickCloseButton()
    {
        //true : 마을로 이동
        //false : 광고 후 부활
        OnClose?.Invoke(true);
        ClosePopupUI();
    }

    void OnClickRebirthButton()
    {
        //true : 마을로 이동
        //false : 광고 후 부활
        OnClose?.Invoke(false);
        ClosePopupUI();
    }
}
