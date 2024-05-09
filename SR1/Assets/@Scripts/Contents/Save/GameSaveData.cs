using System;
using System.Collections.Generic;
using UnityEngine;
using static Define;

[Serializable]
public class GameSaveData
{
    public int PlayerLevel = 1;
    public int PlayerExp = 0;
        
    public int MaxWood = 500;
    public int MaxMineral = 500;
    public int MaxMeat = 500;
    
    public int LeaderDataId = 0;

    public int HireCount = 0;
   
    public int ItemDbIdGenerator = 1;

    public int CurrentStageIndex = 0;
    public List< /*TemplateId*/int> UnlockedTrainingOptions = new List<int>();
    public List<GachaSaveData> GachaInfos = new List<GachaSaveData>();
    public List<ItemSaveData> Items = new List<ItemSaveData>();
    public List<QuestSaveData> AllQuests = new List<QuestSaveData>(); 
    public List<HeroSaveData> Heroes = new List<HeroSaveData>();
    public List<StorageSaveData> Storages = new List<StorageSaveData>();
    //TODO
    public Vector3Int LastCellPos; //TODO 재접할떄 마지막마을을 저장할지 좌표로 저장할지

    public int MaxTeamCount;

    //SettingData
    public bool IsOnAutoCamp;


}

[Serializable]
public class HeroSaveData
{
    public int TemplateId = 0;
    public int Level = 1;
    public int Exp = 0;
    public HeroOwningState OwningState = HeroOwningState.Unowned;
}

[Serializable]
public class ItemSaveData
{
    public int InstanceId;
    public int DbId;
    public int TemplateId;
    public int Count;
    public int EquipSlot; // 장착 + 인벤 + 창고
    //public int OwnerId;
    public int EnchantCount;
    public bool IsLock;
    public List<int> OptionIds = new List<int>();
}

[Serializable]
public class QuestSaveData
{
    public int TemplateId;
    public EQuestState State = EQuestState.Processing;
    public List<int> TaskProgressCount = new List<int>();//Task의 Count
    public List<EQuestState> TaskStates = new List<EQuestState>();//Task의 Count
    public DateTime NextResetTime;
    public int DailyScore;
    public int WeeklyScore;
}

[Serializable]
public class StorageSaveData
{
    public int TemplateId;
    public DateTime LastRewardTime;
    public int StoredResources;
}

[Serializable]
public class GachaSaveData
{
    public int GachaDataId;
    public bool IsPurchased;
    public int GachaExpCount;
}