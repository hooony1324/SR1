using System;
using Data;
using UnityEngine;
using static Define;

public class Storage
{
    public StorageSaveData SaveData { get; set; }

    public int TemplateId
    {
        get { return SaveData.TemplateId; }
        set { SaveData.TemplateId = value; }
    }

    public DateTime LastRewardTime
    {
        get { return SaveData.LastRewardTime; }
        set { SaveData.LastRewardTime = value; }
    }

    public int StoredResources
    {
        get { return SaveData.StoredResources; }
        set { SaveData.StoredResources = value; }
    }
    
    public StorageData StorageData { get; private set; } = new StorageData();

    public Storage(StorageSaveData saveData)
    {
        SaveData = saveData;
        TemplateId = saveData.TemplateId;
        StoredResources = saveData.StoredResources;
        LastRewardTime = saveData.LastRewardTime;
        
        if(Managers.Data.StorageDic.TryGetValue(saveData.TemplateId, out StorageData data))
            StorageData = data;
    }
    
    public int GetStoredQuantity()
    {
        return StoredResources;
    }

    public float GetStorageRatio()
    {
        float ratio = 0;
        ratio = StoredResources / StorageData.MaxCapacity;
        return ratio;
    }

    private DateTime GetRewardUpdateTime()
    {
        return LastRewardTime;
    }
    
    public void AddStoredQuantity(int quantity)
    {
        StoredResources = Mathf.Clamp(StoredResources + quantity, 0, StorageData.MaxCapacity);
        // Debug.Log($"UpdateStoredQuantity : type : {(StorageData.currencyType)}, quantity : {quantity}");
    }
    
    #region Level System

    public int GetLevel()
    {
        return StorageData.Level;
    }

    public bool CanLevelUP()
    {
        if (IsMaxLevel())
            return false;

        return Managers.Inventory.CheckCurrency(StorageData.currencyType, GetExpToNextLevel());
    }

    public bool TryLevelUp()
    {
        if (IsMaxLevel())
            return false;

        if (Managers.Inventory.CheckCurrency(StorageData.currencyType, GetExpToNextLevel()))
        {
            //레벨업 완료
            UpdateStorageData(StorageData.currencyType, StorageData.NextLevelDataID);
            return true;
        }

        return false;
    }

    private void UpdateStorageData(ECurrencyType currencyType, int nextLevelDataID)
    {
        TemplateId = nextLevelDataID;

        if (Managers.Data.StorageDic.TryGetValue(nextLevelDataID, out StorageData data) == true)
        {
            StorageData = data;
        }
    }

    private bool IsMaxLevel()
    {
        return StorageData.NextLevelDataID == 0;
    }

    private int GetExpToNextLevel()
    {
        return StorageData.NextLevelExp;
    }

    #endregion
}