using UnityEngine;

public class UI_Popup : UI_Base
{
    public Canvas UICanvas;
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        UICanvas = Managers.UI.SetCanvas(gameObject, true);
        PopupOpenAnimation(gameObject);
        return true;
    }
    
    public virtual void ClosePopupUI()
    {
        Managers.UI.ClosePopupUI(this);
    }

}
