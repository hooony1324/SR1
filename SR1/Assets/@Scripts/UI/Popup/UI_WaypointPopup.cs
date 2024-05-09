using System.Collections.Generic;
using UnityEngine;

public class UI_WaypointPopup : UI_Popup
{
    enum GameObjects
    {
        WaypointList
    }

    enum Buttons
    {
        CloseButton,
    }
    
    List<UI_StageItem> _items = new List<UI_StageItem>();
    const int MAX_ITEM_COUNT = 15 ;
    
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));
        
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);

        _items.Clear();

        GameObject parent = GetObject((int)GameObjects.WaypointList);
        for (int i = 0; i < MAX_ITEM_COUNT; i++)
        {
            UI_StageItem item = Managers.UI.MakeSubItem<UI_StageItem>(parent.transform);
            _items.Add(item);
        }
        
        Refresh();
        return true;
    }

    //서버로부터 현재 뽑기 상태 내려 받아서 처리해야함
    public void SetInfo()
    {
        Refresh();
    }

    void Refresh()
    {
        if (_init == false)
            return;

        List<Stage> stages = Managers.Map.StageTransition.Stages;

        for (int i = 0; i < _items.Count; i++)
        {
            if (i < stages.Count)
            {
                Stage stage = stages[i];
                _items[i].SetInfo(stage, () => Managers.UI.ClosePopupUI(this));
                _items[i].gameObject.SetActive(true);
            }
            else
            {
                _items[i].gameObject.SetActive(false);
            }
        }
    }

    void OnClickCloseButton()
    {
        Managers.UI.ClosePopupUI(this);
    }
}

