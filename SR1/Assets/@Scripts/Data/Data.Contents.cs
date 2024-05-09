using System;
using System.Collections.Generic;
using static Define;

namespace Data
{
    #region LevelData
    [Serializable]
    public class HeroLevelData
    {
        public int Level;
        public int Exp;
    }

    [Serializable]
    public class HeroLevelDataLoader : ILoader<int, HeroLevelData>
    {
        public List<HeroLevelData> levels = new List<HeroLevelData>();
        public Dictionary<int, HeroLevelData> MakeDict()
        {
            Dictionary<int, HeroLevelData> dict = new Dictionary<int, HeroLevelData>();
            foreach (HeroLevelData levelData in levels)
                dict.Add(levelData.Level, levelData);
            return dict;
        }
    }
    #endregion

    #region PlayerLevelData
    [Serializable]
    public class PlayerLevelData
    {
        public int Level;
        public int Exp;
    }

    [Serializable]
    public class PlayerLevelDataLoader : ILoader<int, PlayerLevelData>
    {
        public List<PlayerLevelData> levels = new List<PlayerLevelData>();
        public Dictionary<int, PlayerLevelData> MakeDict()
        {
            Dictionary<int, PlayerLevelData> dict = new Dictionary<int, PlayerLevelData>();
            foreach (PlayerLevelData levelData in levels)
                dict.Add(levelData.Level, levelData);
            return dict;
        }
    }
    #endregion
    
    #region CreatureData
    [Serializable]
    public class CreatureData
    {
        public int TemplateId;
        public string DescriptionTextID;
        public float ColliderOffsetX;
        public float ColliderOffsetY;
        public float ColliderRadius;
        public float MaxHp;
        public float UpMaxHpBonus;
        public float Atk;
        public float MissChance;
        public float AtkBonus;
        public float MoveSpeed;
        public float CriRate;
        public float CriDamage;
        public string IconImage;
        public string SkeletonDataID;
        public int DefaultSkillId;
        public int EnvSkillId;
        public int SkillAId;
        public int SkillBId;
    }

    [Serializable]
    public class CreatureDataLoader : ILoader<int, CreatureData>
    {
        public List<CreatureData> creatures = new List<CreatureData>();
        public Dictionary<int, CreatureData> MakeDict()
        {
            Dictionary<int, CreatureData> dict = new Dictionary<int, CreatureData>();
            foreach (CreatureData creature in creatures)
                dict.Add(creature.TemplateId, creature);
            return dict;
        }
    }
    #endregion
    
    #region HeroData
    [Serializable]
    public class HeroData : CreatureData
    {
    }

    [Serializable]
    public class HeroDataLoader : ILoader<int, HeroData>
    {
        public List<HeroData> creatures = new List<HeroData>();
        public Dictionary<int, HeroData> MakeDict()
        {
            Dictionary<int, HeroData> dict = new Dictionary<int, HeroData>();
            foreach (HeroData creature in creatures)
                dict.Add(creature.TemplateId, creature);
            return dict;
        }
    }
    #endregion

    #region StorageData
    [Serializable]
    public class StorageData
    {
        public int DataId;
        public string Name;
        public string SpriteName;
        public string Description;
        public int Level;
        public ECurrencyType currencyType;
        
        public int ProductionQuantity;
        public int ProductionSpeed;
        public int MaxCapacity;
        public int NextLevelExp;
        public int NextLevelDataID;
        public int MaxResource;// Max 자원량(저장량 아님)
    }

    [Serializable]
    public class StorageDataLoader : ILoader<int, StorageData>
    {
        public List<StorageData> creatures = new List<StorageData>();
        public Dictionary<int, StorageData> MakeDict()
        {
            Dictionary<int, StorageData> dict = new Dictionary<int, StorageData>();
            foreach (StorageData creature in creatures)
                dict.Add(creature.DataId, creature);
            return dict;
        }
    }
    #endregion
    
    #region HeroInfoData
    [Serializable]
    public class HeroInfoData
    {
        public int templateId;
        public string NameTextId;
        public string DescriptionTextId;
        public string Rarity;
        public float GachaSpawnWeight;
        public float GachaWeight;
        public int GachaExpCount;
        public string IconImage;
    }

    [Serializable]
    public class HeroInfoDataLoader:ILoader<int, HeroInfoData>
    {
        public List<HeroInfoData> heroInfo = new List<HeroInfoData>();
        public Dictionary<int, HeroInfoData> MakeDict()
        {
            Dictionary<int, HeroInfoData> dict = new Dictionary<int, HeroInfoData>();
            foreach (HeroInfoData info in heroInfo)
                dict.Add(info.templateId, info);
            return dict;
        }
    }
    #endregion

    #region MonsterData
    [Serializable]
    public class MonsterData : CreatureData
    {
        public int DropItemId;
        public int Size;
    }

    [Serializable]
    public class MonsterDataLoader : ILoader<int, MonsterData>
    {
        public List<MonsterData> creatures = new List<MonsterData>();
        public Dictionary<int, MonsterData> MakeDict()
        {
            Dictionary<int, MonsterData> dict = new Dictionary<int, MonsterData>();
            foreach (MonsterData creature in creatures)
                dict.Add(creature.TemplateId, creature);
            return dict;
        }
    }
    #endregion
    
    #region SkillData
    [Serializable]
    public class SkillData
    {
        public int TempleteId;
        public string Name;
        public string NameTextId;
        public string ClassName;
        public string Description;
        public string DescriptionTextId;
        public int ProjectileId;
        public string IconLabel;
        public string AnimName;
        public float CoolTime;
        public float Duration;
        public string CastingAnimname;
        public string CastingSound;
        public float SkillRange;
        public int TargetCount;
        public List<int> EffectIds = new List<int>();
        public int NextLevelId;
        public int AoEId;
        public EEffectSize EffectSize;

    }
    [Serializable]
    public class SkillDataLoader : ILoader<int, SkillData>
    {
        public List<SkillData> skills = new List<SkillData>();

        public Dictionary<int, SkillData> MakeDict()
        {
            Dictionary<int, SkillData> dict = new Dictionary<int, SkillData>();
            foreach (SkillData skill in skills)
                dict.Add(skill.TempleteId, skill);
            return dict;
        }
    }
    #endregion
    
    #region ProjectileData
    //HitSound	ProjRange	ProjSpeed

    [Serializable]
    public class ProjectileData
    {
        public int DataId;
        public string Name;
        public EProjetionMotion ProjectileMotion;
        public string SpriteName;
        public string SpineName;
        public float Duration;
        public float HitSound;
        public float ProjRange;
        public float ProjSpeed;
        public int PenetrateCount;
    }
    [Serializable]
    public class ProjectileDataLoader : ILoader<int, ProjectileData>
    {
        public List<ProjectileData> projectiles = new List<ProjectileData>();

        public Dictionary<int, ProjectileData> MakeDict()
        {
            Dictionary<int, ProjectileData> dict = new Dictionary<int, ProjectileData>();
            foreach (ProjectileData projectile in projectiles)
                dict.Add(projectile.DataId, projectile);
            return dict;
        }
    }
    #endregion

    #region Env
    [Serializable]
    public class EnvData
    {
        public int DataId;
        public string DescriptionTextID;
        public string PrefabLabel;
        public float MaxHp;
        public float RegenTime;
        public String SkeletonDataID;
        public int DropItemId;
        public string SpriteName;
        public EEnvType EnvType;
    }

    [Serializable]
    public class EnvDataLoader : ILoader<int, EnvData>
    {
        public List<EnvData> creatures = new List<EnvData>();
        public Dictionary<int, EnvData> MakeDict()
        {
            Dictionary<int, EnvData> dict = new Dictionary<int, EnvData>();
            foreach (EnvData creature in creatures)
                dict.Add(creature.DataId, creature);
            return dict;
        }
    }
    #endregion

    #region Item
    
	[Serializable]
	public class BaseData
	{
		public int DataId;
		public string DescriptionTextID;
		public string SpriteName;
	}

	[Serializable]
	public class ItemData : BaseData
	{
		public string Name;
		public EItemGroupType ItemGroupType;
		public EItemType Type;
		public EItemSubType SubType;
		public EItemGrade Grade;
		public int MaxStack;
	}

	[Serializable]
	public class EquipmentData : ItemData
	{
        public int ItemLevel;
        public ECalcStatType CalcStatType;
        public EStatModType StatModType;
        public int BonusValue;
        public int SubOptionCount;
	}

	[Serializable]
	public class ConsumableData : ItemData
	{
		public double Value;
		public int CoolTime;
	}
    
	[Serializable]
	public class CurrencyData : ItemData
	{
		public ECurrencyType currencyType;
	}
	
	[Serializable]
	public class ItemDataLoader<T> : ILoader<int, T> where T : BaseData
	{
		public List<T> items = new List<T>();

		public Dictionary<int, T> MakeDict()
		{
			Dictionary<int, T> dict = new Dictionary<int, T>();
			foreach (T item in items)
				dict.Add(item.DataId, item);

			return dict;
		}
	}
	#endregion

    #region ItemOption

    [Serializable]
    public class EquipmentOptionData
    {
        public int DataId;
        public string Name;
        public int ItemLevel;
        public EItemGrade OptionGrade;
       
        public ECalcStatType CalcStatType;
        public EStatModType StatModType;
        public float OptionValue;
    }

    [Serializable]
    public class EquipmentOptionDataLoader : ILoader<int, EquipmentOptionData>
    {
        public List<EquipmentOptionData> datas = new List<EquipmentOptionData>();
        public Dictionary<int, EquipmentOptionData> MakeDict()
        {
            Dictionary<int, EquipmentOptionData> dict = new Dictionary<int, EquipmentOptionData>();
            foreach (EquipmentOptionData data in datas)
                dict.Add(data.DataId, data);
            return dict;
        }
    }

    #endregion
    
	#region DropTable
	[Serializable]
	public class RewardData
	{
		public int ItemTemplateId;
		public int Probability; // 100분율
		public int Count;
	}

	[Serializable]
	public class DropTableData
	{
		public int TemplateId;
		public int RewardExp;
		public List<RewardData> Rewards = new List<RewardData>();
	}

	[Serializable]
	public class DropTableDataLoader : ILoader<int, DropTableData>
	{
		public List<DropTableData> dropTables = new List<DropTableData>();

		public Dictionary<int, DropTableData> MakeDict()
		{
			Dictionary<int, DropTableData> dict = new Dictionary<int, DropTableData>();

			foreach (DropTableData tempData in dropTables)
			{
				dict.Add(tempData.TemplateId, tempData);
			}

			return dict;
		}
	}


	#endregion

    #region NPC
    [Serializable]
    public class NpcData
    {
        public int DataId;
        public string Name;
        public string DescriptionTextID;
        public ENpcType NpcType;
        public string PrefabLabel;
        public string IconSpriteName;
        public string SkeletonDataID;
        public int QuestDataId;
        public int QuestTaskDataId;
    }

    [Serializable]
    public class NpcDataLoader : ILoader<int, NpcData>
    {
        public List<NpcData> creatures = new List<NpcData>();
        public Dictionary<int, NpcData> MakeDict()
        {
            Dictionary<int, NpcData> dict = new Dictionary<int, NpcData>();
            foreach (NpcData creature in creatures)
                dict.Add(creature.DataId, creature);
            return dict;
        }
    }
    #endregion

    #region EffectData
    [Serializable]
    public class EffectData
    {
        public int DataId;
        public string Name;
        public string DescriptionTextID;
        public string PrefabName;
        public string SkeletonDataID;
        public string IconLabel;
        public string SoundLabel;
        public float Amount;
        public float PercentAdd;
        public float PercentMult;
        public float TickTime;
        public float TickCount;
        public EEffectType EffectType;
        public ECalcStatType calcStatType;
    }

    [Serializable]
    public class EffectDataLoader : ILoader<int, EffectData>
    {
        public List<EffectData> effects = new List<EffectData>();
        public Dictionary<int, EffectData> MakeDict()
        {
            Dictionary<int, EffectData> dict = new Dictionary<int, EffectData>();
            foreach (EffectData effect in effects)
                dict.Add(effect.DataId, effect);
            return dict;
        }
    }
    #endregion
    
    #region AoEData
    [Serializable]
    public class AoEData
    {
        public int TemplateId;
        public string Name;
        public EAoEType Type;
        public string SkeletonDataID;
        public string PrefabName;
        public string SoundLabel;
        public float Duration;
        public List<int> AllyEffects = new List<int>();
        public List<int> EnemyEffects = new List<int>();
        public string AnimName;
        public EFindRangeType FindRangeType;
    }

    [Serializable]
    public class AoEDataLoader : ILoader<int, AoEData>
    {
        public List<AoEData> aoes = new List<AoEData>();
        public Dictionary<int, AoEData> MakeDict()
        {
            Dictionary<int, AoEData> dict = new Dictionary<int, AoEData>();
            foreach (AoEData aoe in aoes)
                dict.Add(aoe.TemplateId, aoe);
            return dict;
        }
    }
    #endregion

    #region TextData
    [Serializable]
    public class TextData
    {
        public string DataId;
        public string KOR;
    }
    [Serializable]
    public class TextDataLoader:ILoader<string, TextData>
    {
        public List<TextData> texts = new List<TextData>();
        public Dictionary<string, TextData> MakeDict()
        {
            Dictionary<string, TextData> dict = new Dictionary<string, TextData>();
            foreach (TextData text in texts)
                dict.Add(text.DataId, text);
            return dict;
        }
    }
    #endregion

    #region QuestData

    [Serializable]
    public class QuestData
    {
	    public int TemplateId;
	    public string Name;
	    public EQuestPeriodType QuestPeriodType;
	    public List<QuestTaskData> QuestTasks = new List<QuestTaskData>();
    }
    
    [Serializable]
    public class QuestTaskData
    {
        public int TemplateId;
        public string DescriptionTextId;
	    public EQuestObjectiveType ObjectiveType;
        public string ObjectiveIcon;
	    public int ObjectiveDataId;
	    public int ObjectiveCount;
        public EQuestRewardType RewardType;
        public int RewardDataId;
        public int RewardCount;    
        public string RewardIcon;
    }
    
    [Serializable]
    public class QuestDataLoader : ILoader<int, QuestData>
    {
	    public List<QuestData> quests = new List<QuestData>();
	    public Dictionary<int, QuestData> MakeDict()
	    {
		    Dictionary<int, QuestData> dict = new Dictionary<int, QuestData>();
		    foreach (QuestData quest in quests)
			    dict.Add(quest.TemplateId, quest);
		    return dict;
	    }
    }

    #endregion
    
    #region Training Data
    [Serializable]
    public class TrainingData
    {
        public int TemplateId;
        public int RequiredLevel;
        
        public string NameTextId;
        public string DescTextId;
        public string IconName;
        
        public ECalcStatType CalcStatType;
        public EStatModType StatModType;

        public float OptionValue;
        public bool isMainOption;
        public ETrainingMainOption TrainingMainOption;

        public ECurrencyType currencyType;
        public int Price;
    }

    [Serializable]
    public class TrainingDataLoader : ILoader<int, TrainingData>
    {
        public List<TrainingData> datas = new List<TrainingData>();
        public Dictionary<int, TrainingData> MakeDict()
        {
            Dictionary<int, TrainingData> dict = new Dictionary<int, TrainingData>();
            foreach (TrainingData data in datas)
                dict.Add(data.TemplateId, data);
            return dict;
        }
    }
    #endregion

}