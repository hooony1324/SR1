using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_EquipPopup : UI_Popup
{
    #region enum
    enum GameObjects
    {
        EquipmentList,
        RedStoneSlot,
        OrangeStoneSlot,
        PinkStoneSlot,
        MintStoneSlot,
        CloseButton
    }

    enum Images
    {
        RedStoneImage,
        OrangeStoneImage,
        PinkStoneImage,
        MintStoneImage,
    }

    enum Texts
    {
        LevelText,
        BattlePowerText,
        BattlePowerIncreaseText
    }

    #endregion

    List<UI_EquipPopup_EquipmentItem> _slotItems = new List<UI_EquipPopup_EquipmentItem>();
    const int MAX_ITEM_COUNT = 100;


    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindImage(typeof(Images));
        BindText(typeof(Texts));
        
        GetObject((int)GameObjects.RedStoneSlot).BindEvent(OnClickRedStoneSlot);
        GetObject((int)GameObjects.OrangeStoneSlot).BindEvent(OnClickOrangeStoneSlot);
        GetObject((int)GameObjects.PinkStoneSlot).BindEvent(OnClickPinkStoneSlot);
        GetObject((int)GameObjects.MintStoneSlot).BindEvent(OnClickMintStoneSlot);
        GetObject((int)GameObjects.CloseButton).BindEvent(Close);

        Transform parent = GetObject((int)GameObjects.EquipmentList).transform;
        parent.gameObject.DestroyChilds();

        for (int i = 0; i < MAX_ITEM_COUNT; i++)
        {
            //클래스 이름이 너무 길어요ㅠ
            UI_EquipPopup_EquipmentItem item = Managers.UI.MakeSubItem<UI_EquipPopup_EquipmentItem>(parent);
            _slotItems.Add(item);
        }
        
        return true;
    }

    public void SetInfo()
    {
        Managers.Game.OnBroadcastEvent -= HandleOnBroadcastEvent;
        Managers.Game.OnBroadcastEvent += HandleOnBroadcastEvent;
        Refresh();
    }

    void Refresh()
    {
        GetText((int)Texts.LevelText).text = $"Lv.{Managers.Game.PlayerLevel}";
        GetText((int)Texts.BattlePowerText).text = $"Lv.{Managers.Hero.GetBattlePower()}";
        GetText((int)Texts.BattlePowerIncreaseText).text = $"(+{Managers.Inventory.GetEquippedItemScore()})";
        
        RefreshList();

        RefreshSlots();
    }

    void RefreshSlots()
    {
        UpdateSlotImage(EEquipSlotType.Red, Images.RedStoneImage);
        UpdateSlotImage(EEquipSlotType.Yellow, Images.OrangeStoneImage);
        UpdateSlotImage(EEquipSlotType.Pink, Images.PinkStoneImage);
        UpdateSlotImage(EEquipSlotType.Mint, Images.MintStoneImage);
    }

    void UpdateSlotImage(EEquipSlotType slotType, Images imageEnum)
    {
        Item item = Managers.Inventory.GetEquippedItem(slotType);
        Sprite itemSprite = null;

        if (item != null)
        {
            itemSprite = Managers.Resource.Load<Sprite>(item.TemplateData.SpriteName);
        }
        else
        {
            // TODO 여기에서 "empty" 스프라이트를 로드하는 코드를 추가
        }

        GetImage((int)imageEnum).sprite = itemSprite;
    }
    
    void RefreshList()
    {
        List<Item> items = Managers.Inventory.GetItemsByGroupType(EItemGroupType.Equipment);

        for (int i = 0; i < MAX_ITEM_COUNT; i++)
        {
            if (i < items.Count)
            {
                _slotItems[i].gameObject.SetActive(true);
                _slotItems[i].SetInfo(items[i].SaveData);
            }
            else
            {
                _slotItems[i].gameObject.SetActive(false);
            }
        }
    }

    void OnClickRedStoneSlot()
    {
        Item item = Managers.Inventory.GetEquippedItem(EEquipSlotType.Red);
        if(item == null)
            return;
        var popup = Managers.UI.ShowPopupUI<UI_EquipmentInfoPopup>();
        popup.SetInfo(item.SaveData);
    }

    void OnClickOrangeStoneSlot()
    {
        Item orangeStone = Managers.Inventory.GetEquippedItem(EEquipSlotType.Yellow);
        if (orangeStone == null)
            return;
        var popup = Managers.UI.ShowPopupUI<UI_EquipmentInfoPopup>();
        popup.SetInfo(orangeStone.SaveData);
    }

    void OnClickPinkStoneSlot()
    {
        Item pink = Managers.Inventory.GetEquippedItem(EEquipSlotType.Pink);
        if (pink == null)
            return;
        var popup = Managers.UI.ShowPopupUI<UI_EquipmentInfoPopup>();
        popup.SetInfo(pink.SaveData);
    }

    void OnClickMintStoneSlot()
    {
        Item mint = Managers.Inventory.GetEquippedItem(EEquipSlotType.Mint);
        if (mint == null)
            return;
        var popup = Managers.UI.ShowPopupUI<UI_EquipmentInfoPopup>();
        popup.SetInfo(mint.SaveData);
    }

    void Close()
    {
        Managers.Game.OnBroadcastEvent -= HandleOnBroadcastEvent;
        ClosePopupUI();
    }
    
    private void HandleOnBroadcastEvent(EBroadcastEventType eventType, ECurrencyType currencyType, int value)
    {
        switch (eventType)
        {
            case EBroadcastEventType.ChangeInventory:
            case EBroadcastEventType.ChangeCurrency:
                Refresh();
                break;
        }
    }
}