using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_RevivalPopup : UI_Popup
{
    enum Texts
    {
        AdCountText,
    }

    enum Buttons
    {
        ShowAdButton,
        PurchaseJewelButton,
        CloseButton,
    }

    enum Sliders
    {
        CountTimeSlider,
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        BindSlider(typeof(Sliders));

        GetButton((int)Buttons.PurchaseJewelButton).gameObject.BindEvent(OnClickPurchaseJewelButton);
        GetButton((int)Buttons.ShowAdButton).gameObject.BindEvent(OnClickShowAdButton);
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);

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

    }

    void OnClickShowAdButton()
    {
        Debug.Log("On Click Show Ad Button");
    }

    void OnClickPurchaseJewelButton()
    {
        Debug.Log("On Click Purchase Jewel Button");
    }

    void OnClickCloseButton()
    {
        ClosePopupUI();
        Managers.UI.ShowPopupUI<UI_GameOverPopup>();
    }
}
