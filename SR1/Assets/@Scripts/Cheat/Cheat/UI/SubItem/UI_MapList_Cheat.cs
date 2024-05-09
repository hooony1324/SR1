using System.Collections.Generic;

public class UI_MapList_Cheat : UI_Base
{
    enum Buttons
    {
        CloseButton,
    }
    List<UI_MapButton_Cheat> _mapButtonList = new List<UI_MapButton_Cheat>();

    bool _isRefresh = false;
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        Refresh();

        BindButton(typeof(Buttons));
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);

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
        if (_isRefresh)
            return;

        _mapButtonList = new List<UI_MapButton_Cheat>();

        for (int i = 0; i < (Managers.UI.SceneUI as UI_ArtTestScene_Cheat).MapList.Count; i++)
        {
            UI_MapButton_Cheat item = Managers.UI.MakeSubItem<UI_MapButton_Cheat>(transform);
            item.SetInfo((Managers.UI.SceneUI as UI_ArtTestScene_Cheat).MapList[i], this);
            item.gameObject.SetActive(true);
            _mapButtonList.Add(item);
        }
        _isRefresh = true;
    }

    public void CreateMap(string mapName)
    {
        (Managers.Scene.CurrentScene as ArtTestScene).CreateMap(mapName);
    }

    private void OnDisable()
    {
        //gameObject.DestroyChilds();
        foreach (var button in _mapButtonList)
        {
            Managers.Resource.Destroy(button.gameObject);
        }
        _isRefresh = false;
    }

    void OnClickCloseButton()
    {
        gameObject.SetActive(false);
    }
}
