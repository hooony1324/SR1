using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_AutoDismantlePopup : UI_Popup
{
enum GameObjects
    {
        CloseArea,
    }

    enum Texts
    {
        TitleText,

    }

    enum Buttons
    {
        ConfirmButton,
        CancelButton,
    }

    enum Toggles
    {
        NormalToggle,
        RareToggle,
        EpicToggle,
        LegendaryToggle,
    }



    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        BindToggle(typeof(Toggles));

        GetObject((int)GameObjects.CloseArea).BindEvent(OnClickCloseArea);
        GetButton((int)Buttons.CancelButton).gameObject.BindEvent(OnClickCancelButton);
        GetButton((int)Buttons.ConfirmButton).gameObject.BindEvent(OnClickConfirmButton);

        return true;
    }


    public void SetInfo()
    {
        Refresh();
    }

    void Refresh()
    {

    }

    void OnClickCloseArea()
    {
        ClosePopupUI();
    }

    void OnClickCancelButton()
    {
        ClosePopupUI();
    }

    void OnClickConfirmButton()
    {
        Debug.Log("On Click Confirm Button");

        bool dismantleNormal = GetToggle((int)Toggles.NormalToggle).isOn;
        bool dismantleRare = GetToggle((int)Toggles.RareToggle).isOn;
        bool dismantleEpic = GetToggle((int)Toggles.EpicToggle).isOn;
        bool dismantleLegendary = GetToggle((int)Toggles.LegendaryToggle).isOn;

        ClosePopupUI();
    }
}
