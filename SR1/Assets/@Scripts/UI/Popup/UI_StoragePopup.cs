using System;
using Data;
using UnityEngine;

public class UI_StoragePopup : UI_Popup
{
    enum GameObjects
    {
        CloseArea,
    }

    enum Images
    {
        StorageImage,
        CostTypeImage,
    }

    enum Buttons
    {
        UpgradeButton,
        CloseButton
    }

    enum Texts 
    { 
        StorageNameText,
        LevelText,
        ProductionAmountDescriptionText,
        ProductionAmountText,
        MaxProductionAmountDescriptionText,
        MaxProductionAmountText,
        ProductionSpeedDescriptionText,
        ProductionSpeedText,
        CostText,
    }

    private Storage _storage;

    private Action _onLevelUp;
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindImage(typeof(Images));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        GetButton((int)Buttons.UpgradeButton).gameObject.BindEvent(OnClickUpgradeButton);
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClickClose);

        return true;
    }

    public void SetInfo(Storage storage)
    {
        _storage = storage;
        // _onLevelUp = action;
        Refresh();
    }

    void Refresh()
    {
        if (_init == false)
            return;
        StorageData storageData = _storage.StorageData;

        GetText((int)Texts.StorageNameText).text = storageData.Name;

        Sprite storageImg = Managers.Resource.Load<Sprite>(storageData.SpriteName);
        GetImage((int)Images.StorageImage).sprite = storageImg;

        string level = storageData.Level.ToString();
        string nextLevel = (storageData.Level + 1).ToString();
        GetText((int)Texts.LevelText).text = $"Level {level}  >  {nextLevel}";
        
        GetText((int)Texts.ProductionAmountText).text = storageData.ProductionQuantity.ToString();
        GetText((int)Texts.MaxProductionAmountText).text = storageData.MaxCapacity.ToString();
        GetText((int)Texts.ProductionSpeedText).text = $"{storageData.ProductionQuantity}/분";

        Sprite costImg;
        switch (storageData.currencyType)
        {
            case Define.ECurrencyType.Wood:
                costImg = Managers.Resource.Load<Sprite>("FirewoodIC.sprite"); 
                break;
            case Define.ECurrencyType.Mineral:
                costImg = Managers.Resource.Load<Sprite>("MineralIC.sprite"); 
                break;
            case Define.ECurrencyType.Meat:
                costImg = Managers.Resource.Load<Sprite>("MeatIC.sprite");
                break;
            default:
                costImg = Managers.Resource.Load<Sprite>("MeatIC.sprite");
                break;
        }
        GetImage((int)Images.CostTypeImage).sprite = costImg;
            
        GetText((int)Texts.CostText).text = storageData.NextLevelExp.ToString();

        if (_storage.CanLevelUP() == false)
        {
            //disable
            // GetButton((int)Buttons.UpgradeButton).enabled = false;
        }

    }

    void OnClickUpgradeButton()
    {
        if (_storage.TryLevelUp())
        {
            //레벨업 완료
            Refresh();
            _onLevelUp?.Invoke();
        }
        else
        {
            // 레벨업 불가
        }
    }
    
    void OnClickClose()
    {
        ClosePopupUI();
    }
}
