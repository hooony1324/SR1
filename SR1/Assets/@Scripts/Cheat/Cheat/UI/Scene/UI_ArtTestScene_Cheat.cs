using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class UI_ArtTestScene_Cheat : UI_Scene
{
    enum GameObjects
    {

    }

    enum Buttons
    {
        CheatButton,
    }

    enum Texts
    {

    }
    UI_ArtSceneCheatListPopup_Cheat _cheatListPopup;



    public List<string> MapList = new List<string>();
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));
        GetButton((int)Buttons.CheatButton).gameObject.BindEvent(OnClickCheatButton);

        var opHandle = Addressables.LoadResourceLocationsAsync("Maps", typeof(GameObject));
        opHandle.Completed += (op) =>
        {
            foreach (var result in op.Result)
            {
                MapList.Add(result.PrimaryKey);
                Debug.Log(result.PrimaryKey);
            }
        };

        return true;
    }

    void OnClickCheatButton()
    {
        if (_cheatListPopup == null)
        {
            _cheatListPopup = Managers.UI.ShowPopupUI<UI_ArtSceneCheatListPopup_Cheat>();
        }
        else
        {
            _cheatListPopup.gameObject.SetActive(true);
        }
    }
}
