using UnityEngine;

public class UI_ArtSceneCheatListPopup_Cheat : UI_Popup
{
    enum GameObjects
    {
        CloseArea
    }

    enum Buttons
    {
        ChangeCharacterButton,
        ShowAnimationButton,
        MoveSpeedButton,
        ChangeMapButton,
    }

    UI_AnimationList_Cheat _animationListUI;
    UI_MapList_Cheat _mapListUI;
    UI_SelectCharacterPopup_Cheat SelectCharacterUI;
    UI_MoveSpeedItem_Cheat _moveSpeedItemUI;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));

        GetObject((int)GameObjects.CloseArea).BindEvent(OnClickCloseArea);
        
        GetButton((int)Buttons.ChangeCharacterButton).gameObject.BindEvent(OnClickChangeCharacterButton);
        GetButton((int)Buttons.ShowAnimationButton).gameObject.BindEvent(OnClickShowAnimationButton);
        GetButton((int)Buttons.MoveSpeedButton).gameObject.BindEvent(OnClickMoveSpeedButton);
        GetButton((int)Buttons.ChangeMapButton).gameObject.BindEvent(OnClickChangeMapButton);

        return true;
    }

    void Refresh()
    {

    }


    void OnClickChangeCharacterButton()
    {
        gameObject.SetActive(false);
        OffAllUI();

        if (SelectCharacterUI != null)
            SelectCharacterUI.gameObject.SetActive(true);
        else
            SelectCharacterUI = Managers.UI.ShowPopupUI<UI_SelectCharacterPopup_Cheat>();

    }

    void OnClickShowAnimationButton()
    {
        gameObject.SetActive(false);
        OffAllUI();

        if (_animationListUI != null)
        {
            _animationListUI.gameObject.SetActive(true);
            _animationListUI.SetInfo();
        }
        else
        {
            UI_AnimationList_Cheat item = Managers.UI.MakeSubItem<UI_AnimationList_Cheat>(Managers.UI.SceneUI.transform, pooling: false);
            item.transform.localPosition = new Vector3(300f, 0f);
            _animationListUI = item;

            item.SetInfo();
        }
    }

    void OnClickChangeMapButton()
    {
        gameObject.SetActive(false);
        OffAllUI();

        if (_mapListUI != null)
        {
            _mapListUI.gameObject.SetActive(true);
            _mapListUI.SetInfo();
        }
        else
        {
            UI_MapList_Cheat item = Managers.UI.MakeSubItem<UI_MapList_Cheat>(Managers.UI.SceneUI.transform, pooling: false);
            item.transform.localPosition = new Vector3(300f, 0f);
            _mapListUI = item;

            item.SetInfo();
        }
    }

    void OnClickMoveSpeedButton()
    {
        gameObject.SetActive(false);
        if(_moveSpeedItemUI != null)
        {
            _moveSpeedItemUI.gameObject.SetActive(true);
            return;
        }
        _moveSpeedItemUI = Managers.UI.MakeSubItem<UI_MoveSpeedItem_Cheat>(Managers.UI.SceneUI.transform, pooling: false);
        _moveSpeedItemUI.transform.localPosition = new Vector3(-150f, 800f);
    }

    void OffAllUI()
    {
        if (_animationListUI != null)
            _animationListUI.gameObject.SetActive(false);
        if (_mapListUI != null)
            _mapListUI.gameObject.SetActive(false);
    }

    void OnClickCloseArea()
    {
        gameObject.SetActive(false);
    }
}
