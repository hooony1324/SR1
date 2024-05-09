using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_DungeonEntrancePopup : UI_Popup
{
    enum GameObjects
    {
        CloseArea,
        BlockObj,
        Popup,
    }

    enum Buttons
    {
        DungeonEntranceButton,
        CancelButton,
    }

    enum Texts
    {
        DungeonEntranceText,
        EntranceText,
        CancelText,
    }

    private Action _OnEntrance;
    
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        GetObject((int)GameObjects.CloseArea).BindEvent(OnClickCloseArea);
        GetButton((int)Buttons.CancelButton).gameObject.BindEvent(OnClickCancelButton);
        GetButton((int)Buttons.DungeonEntranceButton).gameObject.BindEvent(OnClickDungeonEntranceButton);

        Refresh();

        return true;
    }

    public void SetInfo(Action action)
    {
        if (_init == false)
            return;

        _OnEntrance = action;
        Refresh();
    }

    void Refresh()
    {
        GetObject((int)GameObjects.BlockObj).SetActive(false);
        GetObject((int)GameObjects.Popup).SetActive(true);
    }

    void OnClickCloseArea()
    {
        ClosePopupUI();

    }

    void OnClickDungeonEntranceButton()
    {
        Debug.Log("On Click Dungeon Entrance Button");
        GetObject((int)GameObjects.BlockObj).SetActive(true);
        GetObject((int)GameObjects.Popup).SetActive(false);

        // Dragon 버그있어서 잠시 주석했어요
        // GameObject blackOutAni = Managers.Resource.Instantiate("BlackOutAnimation");
        // Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2f, Screen.height / 2f));
        // blackOutAni.transform.position = new Vector3(worldPos.x, worldPos.y, 0f);
        
        _OnEntrance?.Invoke();
        ClosePopupUI();
    }

    void OnClickCancelButton()
    {
        ClosePopupUI();
    }
}
