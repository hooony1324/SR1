using UnityEngine;

public class UI_EquipPopup_EquipmentItem : UI_SubItem
{
    enum GameObjects
    {
        EquippedEquipmentObject,
    }

    enum Images
    {
        EquipmentImage,
    }

    enum Buttons
    {
        EquipmentButton,
    }

    private ItemSaveData _itemSaveData;
    private Item _item;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindImage(typeof(Images));
        BindButton(typeof(Buttons));

        GetButton((int)Buttons.EquipmentButton).gameObject.BindEvent(OnClickEquipmentButton);
        GetButton((int)Buttons.EquipmentButton).gameObject.BindEvent(null, OnBeginDrag, Define.UIEvent.BeginDrag);
        GetButton((int)Buttons.EquipmentButton).gameObject.BindEvent(null, OnDrag, Define.UIEvent.Drag);
        GetButton((int)Buttons.EquipmentButton).gameObject.BindEvent(null, OnEndDrag, Define.UIEvent.EndDrag);

        Refresh();

        return true;
    }

    public void SetInfo(ItemSaveData itemData)
    {
        _itemSaveData = itemData;
        _item = Managers.Inventory.GetItem(_itemSaveData.InstanceId);
        Refresh();
    }

    void Refresh()
    {
        if (_init == false)
            return;

        if (_itemSaveData == null)
            return;

        GetImage((int)Images.EquipmentImage).sprite = Managers.Resource.Load<Sprite>(_item.TemplateData.SpriteName);

        if (_item.IsEquippedItem())
        {
            GetObject((int)GameObjects.EquippedEquipmentObject).SetActive(true);
        }
        else
        {
            GetObject((int)GameObjects.EquippedEquipmentObject).SetActive(false);
        }
    }

    void OnClickEquipmentButton()
    {
        var popup = Managers.UI.ShowPopupUI<UI_EquipmentInfoPopup>();
        popup.SetInfo(_itemSaveData);
    }
}
