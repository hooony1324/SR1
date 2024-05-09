using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Data;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_InventoryPopup : UI_Popup
{
    #region enum

    enum GameObjects
    {
        ItemList,
        AutoDismantleButton,
        LockedObj,
        UnlockedObj,
        
    }

    enum Toggles
    {
        EquipmentToggle,
        ConsumableToggle,
        CurrencyToggle,
    }

    enum Buttons
    {
        CloseButton,
        SortingButton,
        DismantleButton,
        DetailButton
    }

    enum Images
    {
        ItemSlotImage,
        ItemFrameImage,
        ItemImage,
        
        Stat1IconImage,
        Stat2IconImage,
        Stat3IconImage,
        
        Stat1UpImage,
        Stat2UpImage,
        Stat3UpImage,
        
        Stat1DownImage,
        Stat2DownImage,
        Stat3DownImage
        
    }

    enum Texts
    {
        EquipmentToggleNameText,
        ConsumableToggleNameText,
        ItemNameText,
        InventoryCountText,
        LevelText,
        BattlePowerText,
        Stat1ValueText,
        Stat2ValueText,
        Stat3ValueText,
        UpgradeCostText
    }

    #endregion

    List<UI_Inventory_SlotItem> _slotItems = new List<UI_Inventory_SlotItem>();
    private Toggle _equipmentToggle;
    private Toggle _consumableToggle;
    private Toggle _currencyToggle;

    Item _selectedItem;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindToggle(typeof(Toggles));
        BindText(typeof(Texts));
        BindImage(typeof(Images));
        BindButton(typeof(Buttons));

        _equipmentToggle = GetToggle((int)Toggles.EquipmentToggle);
        _consumableToggle = GetToggle((int)Toggles.ConsumableToggle);
        _currencyToggle = GetToggle((int)Toggles.CurrencyToggle);

        _equipmentToggle.gameObject.BindEvent(Refresh);
        _consumableToggle.gameObject.BindEvent(Refresh);
        _currencyToggle.gameObject.BindEvent(Refresh);

        GetObject((int)GameObjects.AutoDismantleButton).BindEvent(OnClickDismantle);
        GetButton((int)Buttons.SortingButton).gameObject.BindEvent(OnClickSortingButton);
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);
        GetButton((int)Buttons.DetailButton).gameObject.BindEvent(OnClickDetail);

        Transform parent = GetObject((int)GameObjects.ItemList).transform;
        parent.gameObject.DestroyChilds();

        for (int i = 0; i < InventoryManager.DEFAULT_INVENTORY_SLOT_COUNT; i++)
        {
            UI_Inventory_SlotItem item = Managers.UI.MakeSubItem<UI_Inventory_SlotItem>(parent);
            _slotItems.Add(item);
        }

        return true;
    }

    public void SetInfo()
    {
        Refresh();
    }

    void Refresh()
    {
        RefreshInventoryList();
        RefreshItemInfoData();
    }

    void RefreshItemInfoData()
    {
        if (_equipmentToggle.isOn == false)
            return;
        if(_selectedItem == null)
            return;

        //장비이름
        GetText((int)Texts.ItemNameText).text = _selectedItem.TemplateData.Name;
        //아이템 잠금
        //_selectedItem.IsLock
    }

    void RefreshInventoryList()
    {
        int MAX_ITEM_COUNT = InventoryManager.DEFAULT_INVENTORY_SLOT_COUNT;
        GetText((int)Texts.InventoryCountText).text =
            $"{Managers.Inventory.GetCountByGroupType(EItemGroupType.Equipment)} / {MAX_ITEM_COUNT}";
        GetObject((int)GameObjects.AutoDismantleButton).SetActive(true);

        if (_equipmentToggle.isOn)
        {
            List<Item> items = Managers.Inventory.GetItemsByGroupType(EItemGroupType.Equipment);

            for (int i = 0; i < MAX_ITEM_COUNT; i++)
            {
                if (i < items.Count)
                {
                    _slotItems[i].gameObject.SetActive(true);
                    _slotItems[i].SetInfo(items[i], this);
                }
                else
                {
                    _slotItems[i].gameObject.SetActive(true);
                    _slotItems[i].SetInfo(null, this);
                }
            }
        }
        else if (_consumableToggle.isOn)
        {
            List<Item> items = Managers.Inventory.GetItemsByGroupType(EItemGroupType.Currency);
            GetObject((int)GameObjects.AutoDismantleButton).SetActive(false);

            for (int i = 0; i < MAX_ITEM_COUNT; i++)
            {
                if (i < items.Count)
                {
                    switch (((CurrencyData)items[i].TemplateData).currencyType)
                    {
                        case ECurrencyType.Wood:
                        case ECurrencyType.Mineral:
                        case ECurrencyType.Meat:
                        case ECurrencyType.Gold:
                        case ECurrencyType.Dia:
                            _slotItems[i].gameObject.SetActive(false);
                            continue;
                    }

                    _slotItems[i].gameObject.SetActive(true);
                    _slotItems[i].SetInfo(items[i], this);
                }
                else
                {
                    _slotItems[i].gameObject.SetActive(true);
                    _slotItems[i].SetInfo(null, this);
                }
            }
        }
        else
        {
            List<Item> items = Managers.Inventory.GetItemsByGroupType(EItemGroupType.Consumable);
            GetObject((int)GameObjects.AutoDismantleButton).SetActive(false);

            for (int i = 0; i < MAX_ITEM_COUNT; i++)
            {
                if (i < items.Count)
                {
                    switch (((CurrencyData)items[i].TemplateData).currencyType)
                    {
                        case ECurrencyType.Wood:
                        case ECurrencyType.Mineral:
                        case ECurrencyType.Meat:
                        case ECurrencyType.Gold:
                        case ECurrencyType.Dia:
                            _slotItems[i].gameObject.SetActive(false);
                            continue;
                    }

                    _slotItems[i].gameObject.SetActive(true);
                    _slotItems[i].SetInfo(items[i], this);
                }
                else
                {
                    _slotItems[i].gameObject.SetActive(true);
                    _slotItems[i].SetInfo(null, this);
                }
            }
        }
    }

    void Refresh_SelectedItem()
    {
        string gradeString = "";
        switch (_selectedItem.TemplateData.Grade)
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
        GetImage((int)Images.ItemImage).sprite = Managers.Resource.Load<Sprite>(_selectedItem.TemplateData.SpriteName);
    }

    public void SelectItem(Item item)
    {
        if (item == null)
            return;

        _selectedItem = item;
        Refresh_SelectedItem();
    }

    void OnClickDismantle()
    {
        List<Item> items = Managers.Inventory.GetItemsByGroupType(EItemGroupType.Equipment);

        foreach (var item in items)
        {
            if ((int)item.TemplateData.Grade <= (int)EItemGrade.Rare)
                Managers.Inventory.DismantleItem(item.InstanceId);
        }

        Refresh();
    }

    void OnClickSortingButton()
    {
        Debug.Log("On Click Sorting Button");
    }

    void OnClickDetail()
    {
        
    }

    void OnClickCloseButton()
    {
        ClosePopupUI();
    }
}