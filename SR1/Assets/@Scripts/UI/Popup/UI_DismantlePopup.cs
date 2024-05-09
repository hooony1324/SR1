using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_DismantlePopup : UI_Popup
{
    enum GameObjects
    {
        CloseArea,
    }

    enum Buttons
    {
        ConfirmButton,
        CancelButton,
    }

    enum Images
    {
        Result1Image,
        Result2Image,
    }

    enum Texts
    {
        TitleText,
        DismantleDescriptionText,
        Result1CountText,
        Result2CountText,
        ConfirmText,
        CancelText,
    }


    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));
        BindImage(typeof(Images));
        BindText(typeof(Texts));

        GetButton((int)Buttons.CancelButton).gameObject.BindEvent(OnClickCancelButton);
        GetButton((int)Buttons.ConfirmButton).gameObject.BindEvent(OnClickConfirmButton);
        GetObject((int)GameObjects.CloseArea).BindEvent(OnClickCloseArea);

        return true;
    }


    public void SetInfo(/* 아이템 정보 받아오기 */)
    {

        Refresh();
    }

    void Refresh()
    {

    }

    void OnClickConfirmButton()
    {
        Debug.Log("On Click Confirm Button");
    }
    
    void OnClickCancelButton()
    {
        ClosePopupUI();
    }

    void OnClickCloseArea()
    {
        ClosePopupUI();
    }
}
