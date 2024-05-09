using Spine;
using System.Collections.Generic;

public class UI_AnimationList_Cheat : UI_Base
{
    enum Buttons
    {
        CloseButton,
    }

    List<UI_AnimationButton_Cheat> _animationButtonList = new List<UI_AnimationButton_Cheat>();

    bool _isRefresh = false;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));

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
        if (_isRefresh)
            return;

        SkeletonData skeletonData = (Managers.Scene.CurrentScene as ArtTestScene).Creature_Cheat.SkeletonAnim.skeletonDataAsset.GetSkeletonData(false);

        _animationButtonList = new List<UI_AnimationButton_Cheat>();
        foreach (Spine.Animation Animation in skeletonData.Animations)
        {
            UI_AnimationButton_Cheat item = Managers.UI.MakeSubItem<UI_AnimationButton_Cheat>(transform);
            item.SetInfo(Animation.Name);
            item.gameObject.SetActive(true);
            _animationButtonList.Add(item);
        }
        _isRefresh = true;
    }

    private void OnDisable()
    {
        //gameObject.DestroyChilds();
        foreach(var button in _animationButtonList)
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
