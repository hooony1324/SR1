using Data;
using UnityEngine;
using static Define;

public class UI_Inventory_SlotItem : UI_SubItem
{
    enum GameObjects
    {
        EmptySlot,
    }

    enum Images
    {
        ItemSlotImage,
        ItemFrameImage,
        ItemImage,
        EquippedImage,
        SelectedSlotImage,
        LockedImage,
        NewImage,
    }

    enum Texts
    {
        ItemCountText,
    }

    private Item _item;

    UI_InventoryPopup _inventoryPopupUI;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        BindObject(typeof(GameObjects));
        BindImage(typeof(Images));
        BindText(typeof(Texts));

        gameObject.BindEvent(OnClickObject);
        gameObject.BindEvent(null, OnBeginDrag, Define.UIEvent.BeginDrag);
        gameObject.BindEvent(null, OnDrag, Define.UIEvent.Drag);
        gameObject.BindEvent(null, OnEndDrag, Define.UIEvent.EndDrag);

        GetImage((int)Images.EquippedImage).gameObject.SetActive(false);
        GetImage((int)Images.SelectedSlotImage).gameObject.SetActive(false);
        GetImage((int)Images.LockedImage).gameObject.SetActive(false);
        GetImage((int)Images.NewImage).gameObject.SetActive(false);


        return true;
    }

    public void SetInfo(Item item, UI_InventoryPopup popup)
    {
        _item = item;
        _inventoryPopupUI = popup;

        Refresh();
    }

    void Refresh()
    {
        if (_item == null || _inventoryPopupUI == null)
        {
            GetObject((int)GameObjects.EmptySlot).SetActive(true);

            GetText((int)Texts.ItemCountText).gameObject.SetActive(false);
            GetImage((int)Images.ItemSlotImage).gameObject.SetActive(false);
            return;
        }
        else
        {
            GetObject((int)GameObjects.EmptySlot).SetActive(false);
            GetImage((int)Images.ItemSlotImage).gameObject.SetActive(true);
        }
        GetImage((int)Images.ItemImage).sprite = Managers.Resource.Load<Sprite>(_item.TemplateData.SpriteName);
        GetImage((int)Images.ItemImage).gameObject.SetActive(true);

        GetText((int)Texts.ItemCountText).gameObject.SetActive(false);
        if (_item.TemplateData.ItemGroupType == Define.EItemGroupType.Currency)
        {
            // 소모형 아이템만 숫자 보여줌
            GetText((int)Texts.ItemCountText).gameObject.SetActive(true);
            GetText((int)Texts.ItemCountText).text = _item.Count.ToString();
        }

        SelectBg(_item.TemplateData.Grade);
    }

    void OnClickObject()
    {
        //TODO Open popup?
        _inventoryPopupUI.SelectItem(_item);
    }
    
    void SelectBg(Define.EItemGrade grade)
    {
        string gradeString = "";
        switch (grade)
        {
            case Define.EItemGrade.None:
                return;

            case Define.EItemGrade.Normal:
                gradeString = "Normal";
                break;
            case Define.EItemGrade.Rare:
                gradeString = "Rare";
                break;
            case Define.EItemGrade.Epic:
                gradeString = "Epic";
                break;
            case Define.EItemGrade.Legendary:
                gradeString = "Legendary";
                break;
        }
        GetImage((int)Images.ItemSlotImage).sprite = Managers.Resource.Load<Sprite>($"{gradeString}ItemSlot");
        GetImage((int)Images.ItemFrameImage).sprite = Managers.Resource.Load<Sprite>($"{gradeString}ItemFrame");
    }
}