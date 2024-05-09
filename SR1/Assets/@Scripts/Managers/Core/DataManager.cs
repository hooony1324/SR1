using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager
{
    public Dictionary<int, Data.SkillData> SkillDic { get; private set; } = new Dictionary<int, Data.SkillData>();
    public Dictionary<int, Data.MonsterData> MonsterDic { get; private set; } = new Dictionary<int, Data.MonsterData>();
    public Dictionary<int, Data.HeroData> HeroDic { get; private set; } = new Dictionary<int, Data.HeroData>();
    public Dictionary<int, Data.HeroInfoData> HeroInfoDic { get; private set; } = new Dictionary<int, Data.HeroInfoData>();
    public Dictionary<int, Data.EnvData> EnvDic { get; private set; } = new Dictionary<int, Data.EnvData>();
    public Dictionary<int, Data.NpcData> NpcDic { get; private set; } = new Dictionary<int, Data.NpcData>();
    public Dictionary<int, Data.EffectData> EffectDic { get; private set; } = new Dictionary<int, Data.EffectData>();
    public Dictionary<int, Data.ProjectileData> ProjectileDic { get; private set; } = new Dictionary<int, Data.ProjectileData>();
    public Dictionary<int, Data.AoEData> AoEDic { get; private set; } = new Dictionary<int, Data.AoEData>();
    public Dictionary<int, Data.HeroLevelData> HeroLevelDic { get; private set; } = new Dictionary<int, Data.HeroLevelData>();
    public Dictionary<int, Data.PlayerLevelData> PlayerLevelDic { get; private set; } = new Dictionary<int, Data.PlayerLevelData>();
    public Dictionary<string, Data.TextData> TextDic { get; private set; } = new Dictionary<string, Data.TextData>();
    public Dictionary<int, Data.StorageData> StorageDic { get; private set; } = new Dictionary<int, Data.StorageData>();

    public Dictionary<int, Data.EquipmentData> EquipmentDic { get; private set; } = new Dictionary<int, Data.EquipmentData>();
    public Dictionary<int, Data.EquipmentOptionData> EquipmentOptionDic { get; private set; } = new Dictionary<int, Data.EquipmentOptionData>();
    public Dictionary<int, Data.ConsumableData> ConsumableDic { get; private set; } = new Dictionary<int, Data.ConsumableData>();
    public Dictionary<int, Data.CurrencyData> CurrencyDic { get; private set; } = new Dictionary<int, Data.CurrencyData>();
    public Dictionary<int, Data.ItemData> ItemDic { get; private set; } = new Dictionary<int, Data.ItemData>();
    public Dictionary<int, Data.DropTableData> DropTableDic { get; private set; } = new Dictionary<int, Data.DropTableData>();
    
    public Dictionary<int, Data.QuestTaskData> QuestTaskDic { get; private set; } = new Dictionary<int, Data.QuestTaskData>();
    public Dictionary<int, Data.QuestData> QuestDic { get; private set; } = new Dictionary<int, Data.QuestData>();
    
    public Dictionary<int, Data.TrainingData> TrainingDic { get; private set; } = new Dictionary<int, Data.TrainingData>();

    public void Init()
    {
        MonsterDic = LoadJson<Data.MonsterDataLoader, int, Data.MonsterData>("MonsterData").MakeDict();
        HeroDic = LoadJson<Data.HeroDataLoader, int, Data.HeroData>("HeroData").MakeDict();
        HeroInfoDic = LoadJson<Data.HeroInfoDataLoader, int, Data.HeroInfoData>("HeroInfoData").MakeDict();
        EnvDic = LoadJson<Data.EnvDataLoader, int, Data.EnvData>("EnvData").MakeDict();
        NpcDic = LoadJson<Data.NpcDataLoader, int, Data.NpcData>("NpcData").MakeDict();
        EffectDic = LoadJson<Data.EffectDataLoader, int, Data.EffectData>("EffectData").MakeDict();
        ProjectileDic = LoadJson<Data.ProjectileDataLoader, int, Data.ProjectileData>("ProjectileData").MakeDict();
        SkillDic = LoadJson<Data.SkillDataLoader, int, Data.SkillData>("SkillData").MakeDict();
        AoEDic = LoadJson<Data.AoEDataLoader, int, Data.AoEData>("AoEData").MakeDict();
        HeroLevelDic = LoadJson<Data.HeroLevelDataLoader, int, Data.HeroLevelData>("HeroLevelData").MakeDict();
        PlayerLevelDic = LoadJson<Data.PlayerLevelDataLoader, int, Data.PlayerLevelData>("PlayerLevelData").MakeDict();
        TextDic = LoadJson<Data.TextDataLoader, string, Data.TextData>("TextData").MakeDict();
        StorageDic = LoadJson<Data.StorageDataLoader, int, Data.StorageData>("StorageData").MakeDict();
        QuestDic = LoadJson<Data.QuestDataLoader, int, Data.QuestData>("QuestData").MakeDict();
        EquipmentOptionDic = LoadJson<Data.EquipmentOptionDataLoader, int, Data.EquipmentOptionData>("EquipmentOptionData").MakeDict();
        
        EquipmentDic = LoadJson<Data.ItemDataLoader<Data.EquipmentData>, int, Data.EquipmentData>("Item_EquipmentData").MakeDict();
        ConsumableDic = LoadJson<Data.ItemDataLoader<Data.ConsumableData>, int, Data.ConsumableData>("Item_ConsumableData").MakeDict();
        CurrencyDic = LoadJson<Data.ItemDataLoader<Data.CurrencyData>, int, Data.CurrencyData>("Item_CurrencyData").MakeDict();
        DropTableDic = LoadJson<Data.DropTableDataLoader, int, Data.DropTableData>("DropTableData").MakeDict();
        
        TrainingDic = LoadJson<Data.TrainingDataLoader, int, Data.TrainingData>("TrainingData").MakeDict();
        
        ItemDic.Clear();

        foreach (var item in EquipmentDic)
            ItemDic.Add(item.Key, item.Value);

        foreach (var item in ConsumableDic)
            ItemDic.Add(item.Key, item.Value);
        
        foreach (var item in CurrencyDic)
            ItemDic.Add(item.Key, item.Value);
    }

    private Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        Debug.Log($"{path}");
		TextAsset textAsset = Managers.Resource.Load<TextAsset>($"{path}");
        return JsonConvert.DeserializeObject<Loader>(textAsset.text);
	}

    #region HeroInfoData
    [Serializable]
    public class HeroInfoData
    {
        public int DataId;
        public string DescriptionTextId;
        public string Rarity;
        public float HireSpawnWeight;
        public float GachaWeight;
        public string IconImage;
    }

    [Serializable]
    public class HeroInfoDataLoader : ILoader<int, HeroInfoData>
    {
        public List<HeroInfoData> heroInfo = new List<HeroInfoData>();
        public Dictionary<int, HeroInfoData> MakeDict()
        {
            Dictionary<int, HeroInfoData> dict = new Dictionary<int, HeroInfoData>();
            foreach (HeroInfoData info in heroInfo)
                dict.Add(info.DataId, info);
            return dict;
        }
    }
    #endregion
}
