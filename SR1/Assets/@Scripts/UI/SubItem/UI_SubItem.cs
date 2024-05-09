using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SubItem : UI_Base
{
    [SerializeField]
    protected ScrollRect _parentScrollRect;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        _parentScrollRect = Util.FindAncestor<ScrollRect>(gameObject);
        return true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _parentScrollRect.OnBeginDrag(eventData);
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        _parentScrollRect.OnDrag(eventData);
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        _parentScrollRect.OnEndDrag(eventData);
    }
}
