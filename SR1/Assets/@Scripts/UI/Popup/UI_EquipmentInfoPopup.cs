using Data;
using UnityEngine;
using static Define;

public class UI_EquipmentInfoPopup : UI_Popup
{
    #region Enum
    enum GameObjects
    {
        CloseArea,
        UnequippedButtonArea,
        EquippedButtonArea,
        
        Option1StatArea,
        Option2StatArea,
        Option3StatArea,
        
        OptionStatArea,
       
        NormalItemBG,
        RareItemBG,
        EpicItemBG,
        LegendItemBG
        
    }

    enum Images
    {
        EquipmentImage,
        Material1Image,
        Material2Image,
        //--순서 바꾸기 금지
        Option1Icon,
        Option1Outline,
        Option2Icon,
        Option2Outline,
        Option3Icon,
        Option3Outline,
        //--
        MainStatGradient
    }

    enum Texts
    {
        EquipmentText,
        EnchantCountText,
        EquipmentNameText,
        EquipmentLevelText,
        EquipmentScoreText,
        MainStatTypeText,
        MainStatValueText,
        
        //--순서 바꾸기 금지
        Option1StatTypeText,
        Option1StatValueText,
        Option2StatTypeText,
        Option2StatValueText,
        Option3StatTypeText,
        Option3StatValueText,
        //--
        Material1CountText,
        // Material2CountText
    }

    enum Buttons
    {
        EquipButton,
        UnequipButton,
        DismantleButton,
        UpgradeButton,
    }
    #endregion

    private ItemSaveData _itemSaveData;
    private Equipment _item;
 
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));
        BindImage(typeof(Images));
        BindText(typeof(Texts));
        // BindButton(typeof(Buttons));

        GetObject((int)GameObjects.CloseArea).BindEvent(OnClickCloseArea);
        GetButton((int)Buttons.EquipButton).gameObject.BindEvent(OnClickEquipButton);
        GetButton((int)Buttons.UnequipButton).gameObject.BindEvent(OnClickUnequipButton);
        GetButton((int)Buttons.DismantleButton).gameObject.BindEvent(OnClickDismantleButton);
        GetButton((int)Buttons.UpgradeButton).gameObject.BindEvent(OnClickUpgradeButton);

        Refresh();

        return true;
    }

    public void SetInfo(ItemSaveData itemData)
    {
        _itemSaveData = itemData;
        _item = Managers.Inventory.GetItem(_itemSaveData.InstanceId) as Equipment;
        Refresh();
    }

    void Refresh()
    {
        if (_init == false)
            return;

        if (_itemSaveData == null)
            return;

        if (Managers.Data.EquipmentDic.TryGetValue(_itemSaveData.TemplateId, out EquipmentData equipData) == false)
        {
            return;
        }

        GetImage((int)Images.EquipmentImage).sprite = Managers.Resource.Load<Sprite>(equipData.SpriteName);

        GetText((int)Texts.EquipmentNameText).text = equipData.Name;
        GetText((int)Texts.EnchantCountText).text = _item.EnchantCount.ToString();
        
        //dragon 임시로만듬
        int score = equipData.ItemLevel + (equipData.BonusValue) * equipData.SubOptionCount;
        GetText((int)Texts.EquipmentLevelText).text = equipData.ItemLevel.ToString();
        GetText((int)Texts.EquipmentScoreText).text = score.ToString();

        // main option
        GetText((int)Texts.MainStatTypeText).text = equipData.CalcStatType.ToString();
        GetText((int)Texts.MainStatValueText).text = Util.ParseEquipOptionValue(_item.MainOption.CalcStatType,_item.MainOption.StatModType, _item.MainOption.OptionValue);
        GetImage((int)Images.MainStatGradient).color = Util.GetOutlineColor(_item.TemplateData.Grade);
        SelectBg(_item.TemplateData.Grade);


        //subOption
        for (int i = 0; i < 3; i++)
        {
            int objectTypeIndex = (int)GameObjects.Option1StatArea;
            if (i < _itemSaveData.OptionIds.Count)
            {
                int id = _itemSaveData.OptionIds[i];
                if(Managers.Data.EquipmentOptionDic.TryGetValue(id, out EquipmentOptionData optionData))
                {
                    GetObject(objectTypeIndex + i).gameObject.SetActive(true);            

                    int typeTextIndex = (int)Texts.Option1StatTypeText + i * 2;
                    int valueTextIndex = typeTextIndex + 1; 
                    int typeIconline = (int)Images.Option1Icon + i * 2;
                    int typeOutlineIndex = typeIconline + 1;

                    GetText(typeTextIndex).text = optionData.CalcStatType.ToString();
                    GetText(typeTextIndex).color = Util.GetTextColor(optionData.OptionGrade);

                    GetText(valueTextIndex).text = Util.ParseEquipOptionValue(optionData.CalcStatType,optionData.StatModType, optionData.OptionValue);
                    GetText(valueTextIndex).color = Util.GetTextColor(optionData.OptionGrade);

                    GetImage(typeIconline).color = Util.GetTextColor(optionData.OptionGrade);
                    GetImage(typeOutlineIndex).color = Util.GetOutlineColor(optionData.OptionGrade);
                    
                }
            }
            else
            {   
                //안쓰는 옵션 hide
                GetObject(objectTypeIndex + i).gameObject.SetActive(false);            
            }
        }

        if (_itemSaveData.OptionIds.Count == 0)
        {
            GetObject((int)GameObjects.OptionStatArea).SetActive(false);
        }

        if (_item.IsEquippedItem())
        {
            GetButton((int)Buttons.EquipButton).gameObject.SetActive(false);
            GetButton((int)Buttons.UnequipButton).gameObject.SetActive(true);
        }
        else
        {
            GetButton((int)Buttons.EquipButton).gameObject.SetActive(true);
            GetButton((int)Buttons.UnequipButton).gameObject.SetActive(false);
        }
        
        //TODO 장비강화 데이터 필요
        Sprite spr = Managers.Resource.Load<Sprite>("rune_pink_1.sprite");
        GetImage((int)Images.Material1Image).sprite = spr;
        GetText((int)Texts.Material1CountText).text = _item.CalculateRequiredMaterials().ToString();
        if (Managers.Inventory.CheckCurrency(ECurrencyType.Fragments, _item.CalculateRequiredMaterials()) == false)
        {
            GetText((int)Texts.Material1CountText).color = Color.red;
        }
        else
        {
            GetText((int)Texts.Material1CountText).color = Color.white;
        }
    }

    void OnClickCloseArea()
    {
        Managers.UI.ClosePopupUI(this);
    }

    void OnClickEquipButton()
    {
        Managers.Inventory.EquipItem(_item.InstanceId);
        Refresh();
    }

    void OnClickUnequipButton()
    {
        Managers.Inventory.UnEquipItem(_item.InstanceId);
        Refresh();
    }

    void OnClickDismantleButton()
    {
        Managers.Inventory.DismantleItem(_item.InstanceId);
        ClosePopupUI();
    }

    void OnClickUpgradeButton()
    {
        if (Managers.Inventory.SpendCurrency(ECurrencyType.Fragments, _item.CalculateRequiredMaterials()))
        {
            Managers.Inventory.EnchantItem(_item.InstanceId);
            Refresh();
        }

    }

    void SelectBg(EItemGrade grade)
    {
        GetObject((int)GameObjects.NormalItemBG).SetActive(false);
        GetObject((int)GameObjects.RareItemBG).SetActive(false);
        GetObject((int)GameObjects.EpicItemBG).SetActive(false);
        GetObject((int)GameObjects.LegendItemBG).SetActive(false);

        switch (grade)
        {
            case EItemGrade.Normal:
                GetObject((int)GameObjects.NormalItemBG).SetActive(true);
                break;
            case EItemGrade.Rare:
                GetObject((int)GameObjects.RareItemBG).SetActive(true);
                break;
            case EItemGrade.Epic:
                GetObject((int)GameObjects.EpicItemBG).SetActive(true);
                break;
            case EItemGrade.Legendary:
                GetObject((int)GameObjects.LegendItemBG).SetActive(true);
                break;
            default:
                GetObject((int)GameObjects.NormalItemBG).SetActive(true);
                break;
        }
    }
}
