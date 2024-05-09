using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data;
using UnityEngine;
using static Define;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class GameManager
{
    #region GameData

    GameSaveData _saveData = new GameSaveData();

    public GameSaveData SaveData
    {
        get { return _saveData; }
        set { _saveData = value; }
    }

    public int PlayerLevel
    {
        get { return _saveData.PlayerLevel; }
        private set { _saveData.PlayerLevel = value; }
    }

    public int PlayerExp
    {
        get { return _saveData.PlayerExp; }
        private set { _saveData.PlayerExp = value; }
    }

    public List< /*TemplateId*/int> UnlockedTrainings = new List<int>();

    public int MaxTeamCount
    {
        get { return _saveData.MaxTeamCount; }
        set { _saveData.MaxTeamCount = value; }
    }

    public bool IsOnAutoCamp
    {
        get { return _saveData.IsOnAutoCamp;}
        set
        {
            _saveData.IsOnAutoCamp = value;
            BroadcastEvent(EBroadcastEventType.ChangeSetting);
        }
    }

    #region 저장소
    public Dictionary<ECurrencyType, Storage> Storages = new Dictionary<ECurrencyType, Storage>();
    #endregion

    public int HireCount
    {
        get { return _saveData.HireCount; }
        set { _saveData.HireCount = value; }
    }

    public Vector3Int LastCellPos
    {
        get { return _saveData.LastCellPos; }
        set { _saveData.LastCellPos = value; }       
    }

    public void BroadcastEvent(EBroadcastEventType eventType, ECurrencyType currencyType = ECurrencyType.None, int value = 0)
    {
        switch (eventType)
        {
            case EBroadcastEventType.KillMonster:
                AddExp(1);
                break;
        }
        
        OnBroadcastEvent?.Invoke(eventType, currencyType, value);
        if (Managers.Scene.CurrentScene.SceneType == EScene.GameScene)
        {
            SaveGame();//임시
        }

    }

    public int GenerateItemDbId()
    {
        int itemDbId = _saveData.ItemDbIdGenerator;
        _saveData.ItemDbIdGenerator++;
        return itemDbId;
    }

    #endregion

    #region Player

    private Hero _leader;

    public Hero Leader
    {
        get => _leader;
        set { _leader = value; }
    }

    private Vector2 _moveDir;

    public Vector2 MoveDir
    {
        get => _moveDir;
        set
        {
            _moveDir = value;
            OnMoveDirChanged?.Invoke(_moveDir);
        }
    }

    private EJoystickState _joystickState;

    public EJoystickState JoystickState
    {
        get => _joystickState;
        set
        {
            _joystickState = value;
            OnJoystickStateChanged?.Invoke(_joystickState);
        }
    }

    #endregion

    public EJoystickType JoystickType = EJoystickType.Flexible;

    private CameraController _cam;

    public CameraController Cam
    {
        get
        {
            if (_cam == null)
            {
                _cam = Object.FindObjectOfType<CameraController>();
            }

            return _cam;
        }
    }
    
    public Npc TownPortal { get; set; }
    public Npc CampPortal { get; set; }

    #region Action

    public event Action<Vector2> OnMoveDirChanged;
    public event Action<EJoystickState> OnJoystickStateChanged;
    public event Action<EBroadcastEventType, ECurrencyType, int> OnBroadcastEvent;

    #endregion

    public void Init()
    {
        TotalGachaSpawnWeight();

        if (File.Exists(Path) == false)
            InitGame();
        else
            LoadGame();
    }

    #region Save & Load

    public string Path
    {
        get { return Application.persistentDataPath + "/SaveData.json"; }
    }

    public void InitGame()
    {
        #region Hero
        MaxTeamCount = 4;
        
        //영웅
        foreach (var heroId in Managers.Data.HeroDic.Keys)
        {
            SaveData.Heroes.Add(Managers.Hero.MakeHeroInfo(heroId));
        }

        #endregion

        #region Quest

        var quests = Managers.Data.QuestDic.Values.ToList();
        Debug.Log(quests.Count);
        foreach (QuestData questData in quests)
        {
            QuestSaveData saveData = new QuestSaveData()
            {
                TemplateId = questData.TemplateId,
                State = EQuestState.Processing,
                TaskProgressCount = new List<int>(),
                TaskStates = new List<EQuestState>(),
                NextResetTime = DateTime.Now,
            };

            for (int i = 0; i < questData.QuestTasks.Count; i++)
            {
                saveData.TaskProgressCount.Add(0);
                saveData.TaskStates.Add(EQuestState.None);
            }

            Debug.Log("SaveDataQuest");
            Managers.Quest.AddQuest(saveData);
        }

        #endregion

        #region Storage

        //gold
        StorageSaveData goldSaveData = new StorageSaveData()
        {
            TemplateId = 0,
            LastRewardTime = DateTime.Now,
            StoredResources = 0
        };
        Storage goldStorage = new Storage(goldSaveData);
        Storages.Add(ECurrencyType.Gold, goldStorage);

        //mineral
        StorageSaveData mineralSaveData = new StorageSaveData()
        {
            TemplateId = 11,
            LastRewardTime = DateTime.Now,
            StoredResources = 0
        };
        Storage mineralStorage = new Storage(mineralSaveData);
        Storages.Add(ECurrencyType.Mineral, mineralStorage);

        //wood
        StorageSaveData woodSaveData = new StorageSaveData()
        {
            TemplateId = 22,
            LastRewardTime = DateTime.Now,
            StoredResources = 0
        };
        Storage woodStorage = new Storage(woodSaveData);
        Storages.Add(ECurrencyType.Wood, woodStorage);
        #endregion

        #region Gacha
        for (int i = 0; i < 3; i++)
        {
            _gachaList[i] = new GachaSaveData();
        }
        RefreshGachaList();
        #endregion

        #region Inventory(Currency)

        foreach (var key in Managers.Data.CurrencyDic.Keys)
        {
            Managers.Inventory.MakeItem(key, 0);
        }

        #endregion

        //Settings
        IsOnAutoCamp = true;
        
        int startHeroId = 201006;
        Managers.Hero.AcquireHeroCard(startHeroId, 1);
        Managers.Hero.GetHeroInfo(startHeroId).OwningState = HeroOwningState.Picked;
        SaveGame();
    }

    public void SaveGame()
    {
        if(Managers.Object.HeroCamp != null)
            LastCellPos = Managers.Map.World2Cell(Managers.Object.HeroCamp.transform.position);
        
        //Hero
        SaveData.Heroes.Clear();
        foreach (var heroinfo in Managers.Hero.AllHeroInfos.Values)
        {
            SaveData.Heroes.Add(heroinfo.SaveData);
        }

        // Quest
        {
            SaveData.AllQuests.Clear();
            foreach (Quest quest in Managers.Quest.AllQuests.Values)
            {
                SaveData.AllQuests.Add(quest.SaveData);
            }
        }
       
        //storage
        {
            SaveData.Storages.Clear();
            foreach (var storages in Storages.Values)
            {
                SaveData.Storages.Add(storages.SaveData);
            }
        }
        
        // Item
        {
            SaveData.Items.Clear();
            foreach (var item in Managers.Inventory.AllItems)
                SaveData.Items.Add(item.SaveData);
        }
        
        //Gacha
        {
            SaveData.GachaInfos.Clear();
            foreach (var info in GachaList)
            {
                SaveData.GachaInfos.Add(info);
            }
        }
        
        //Training
        SaveData.UnlockedTrainingOptions.Clear();
        foreach (var templateId in UnlockedTrainings)
        {
            SaveData.UnlockedTrainingOptions.Add(templateId);
        }
        
        string jsonStr = JsonUtility.ToJson(Managers.Game.SaveData);
        File.WriteAllText(Path, jsonStr);
    }

    public void LoadGame()
    {
        string fileStr = File.ReadAllText(Path);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(fileStr);

        if (data != null)
            Managers.Game.SaveData = data;

        //HeroInfo

        Managers.Hero.AllHeroInfos.Clear();

        foreach (var saveData in data.Heroes)
        {
            Managers.Hero.AddHeroInfo(saveData);
        }

        Managers.Hero.AddUnknownHeroes();

        // Quest
        {
            Managers.Quest.Clear();

            foreach (QuestSaveData questSaveData in data.AllQuests)
            {
                Managers.Quest.AddQuest(questSaveData);
            }
            Managers.Quest.AddUnknownQuests();
        }

        //Storage Load
        Storages.Clear();

        foreach (var saveData in data.Storages)
        {
            Storage storage = new Storage(saveData);
            Storages.Add(storage.StorageData.currencyType, storage);
        }
        
        //Item
        {
            Managers.Inventory.Clear();

            for (int i = 0; i < data.Items.Count; i++)
            {
                Managers.Inventory.AddItem(data.Items[i]);
            }
        }
        
        //Gacha
        {
            for (int i = 0; i < data.GachaInfos.Count; i++)
            {
                _gachaList[i] = data.GachaInfos[i];
            }            
        }
        
        //Training
        {
            UnlockedTrainings.Clear();
            foreach (var templateId in data.UnlockedTrainingOptions)
            {
                UnlockedTrainings.Add(templateId);
            }
        }
        Debug.Log($"Save Game Loaded : {Path}");
    }

    #endregion

    #region Teleport

    public void TeleportHeroes(Vector3 position)
    {
        TeleportHeroes(Managers.Map.World2Cell(position));
    }
    
    public void TeleportHeroes(Vector3Int position)
    {
        foreach (var hero in Managers.Object.Heroes)
        {
            hero.Target = null;
            Vector3Int randCellPos = Managers.Game.GetNearbyPosition(hero, position);
            hero.transform.position = Managers.Map.Cell2World(randCellPos);
            Managers.Map.MoveTo(hero, randCellPos, true);
        }

        Managers.Object.HeroCamp.ForceMove(Leader.Position);
        Cam.transform.position = Leader.Position;
        // Managers.Map.StageTransition.OnMapChanged(Leader.Position);
    }

    #endregion

    #region Helper

    public Vector3Int GetNearbyPosition(BaseObject hero, Vector3Int pivot)
    {
        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

        queue.Enqueue(pivot);
        visited.Add(pivot);

        while (queue.Count > 0)
        {
            Vector3Int current = queue.Dequeue();
        
            if (Managers.Map.CanGo(hero, current))
                return current;

            List<Vector3Int> neighbors = new List<Vector3Int>
            {
                new Vector3Int(current.x - 1, current.y, current.z),
                new Vector3Int(current.x + 1, current.y, current.z),
                new Vector3Int(current.x, current.y - 1, current.z),
                new Vector3Int(current.x, current.y + 1, current.z)
            };
            
            neighbors.Shuffle();

            foreach (Vector3Int neighbor in neighbors)
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }

        return Managers.Game.Leader.CellPos;
    }

    #endregion

    #region Gacha

    float _totalSpawnWeight;
    public GachaSaveData[] _gachaList = new GachaSaveData[3];
    public GachaSaveData[] GachaList
    {
        get { return _gachaList; }
    }

    void TotalGachaSpawnWeight()
    {
        foreach (KeyValuePair<int, HeroInfoData> data in Managers.Data.HeroInfoDic)
        {
            _totalSpawnWeight += data.Value.GachaSpawnWeight;
        }
    }

    public void RefreshGachaList()
    {
        for (int i = 0; i < 3; i++)
        {
            float randomWeight = Random.Range(0f, _totalSpawnWeight);
            float sumWeight = 0f;
            foreach (KeyValuePair<int, HeroInfoData> data in Managers.Data.HeroInfoDic)
            {
                sumWeight += data.Value.GachaSpawnWeight;
                if (sumWeight >= randomWeight)
                {
                    if (_gachaList.Any(info => info.GachaDataId == data.Value.templateId))
                    {
                        i--;
                        break;
                    }

                    GachaSaveData gacha = new GachaSaveData
                    {
                        GachaDataId = data.Value.templateId,
                        IsPurchased = false,
                        GachaExpCount = data.Value.GachaExpCount
                    };

                    _gachaList[i] = gacha;
                    break;
                }
            }
        }
    }

    public int Gacha(int multiple)
    {
        float totalWeight = 0f;

        for (int i = 0; i < GachaList.Length; i++)
        {
            if (GachaList[i].IsPurchased == false)
            {
                totalWeight += Managers.Data.HeroInfoDic[GachaList[i].GachaDataId].GachaWeight;
            }
        }

        if (totalWeight <= 0f)
            return -1;

        float randomWeight = Random.Range(0, totalWeight);
        float sumWeight = 0f;

        for (int i = 0; i < GachaList.Length; i++)
        {
            if (GachaList[i].IsPurchased == false)
            {
                sumWeight += Managers.Data.HeroInfoDic[GachaList[i].GachaDataId].GachaWeight;
                Debug.Log(GachaList[i].GachaDataId);
            }

            if (randomWeight <= sumWeight)
            {
                int dataId = GachaList[i].GachaDataId;
                int gachaExpCount = GachaList[i].GachaExpCount * multiple;

                Managers.Hero.AcquireHeroCard(dataId, gachaExpCount);

                GachaList[i].IsPurchased = true;
                Managers.Game.HireCount += multiple;

                return i;
            }
        }

        return -1;
    }
    #endregion

    #region Player Level System

    public void AddExp(int amount)
    {
        if (IsMaxLevel())
            return;

        PlayerExp += amount;
        if(CanLevelUp())
            TryLevelUp();
    }

    public bool CanLevelUp()
    {
        return (GetExpToNextLevel() - PlayerExp <= 0);
    }

    private void TryLevelUp()
    {
        while (!IsMaxLevel())
        {
            if (CanLevelUp() == false)
            {
                break;
            }

            PlayerExp -= GetExpToNextLevel();
            PlayerLevel++;

            Managers.Game.BroadcastEvent(EBroadcastEventType.PlayerLevelUp, value:PlayerLevel);
        }
    }

    public float GetExpNormalized()
    {
        if (IsMaxLevel())
        {
            return 1f;
        }
        else
        {
            return (float)PlayerExp / GetExpToNextLevel();
        }
    }

    public int GetRemainsExp()
    {
        return GetExpToNextLevel() - PlayerExp;
    }

    private int GetExpToNextLevel()
    {
        PlayerLevelData playerLevelData;
        if (Managers.Data.PlayerLevelDic.TryGetValue(PlayerLevel, out playerLevelData))
        {
            return playerLevelData.Exp;
        }
        else
        {
            Debug.Log("Level invalid: " + PlayerLevel);
            return 100;
        }
    }

    public bool IsMaxLevel()
    {
        return PlayerLevel == Managers.Data.PlayerLevelDic.Count;
    }

    #endregion

    #region Training

    public void UnLockTraining(int templateId)
    {
        UnlockedTrainings.Add(templateId);
        BroadcastEvent(EBroadcastEventType.UnlockTraining, value:templateId);
    }

    public float GetTrainingStatModifier(ECalcStatType calcStatType, EStatModType type)
    {
        float result = 0;
        foreach (var templateId in UnlockedTrainings)
        {
            if (Managers.Data.TrainingDic.TryGetValue(templateId, out TrainingData trainingData) == false)
                continue;

            if(trainingData.CalcStatType == ECalcStatType.None || trainingData.OptionValue == 0)
                continue;
            
            if( trainingData.CalcStatType != calcStatType)
                continue;
            
            if(trainingData.StatModType != type)
                continue;
            
            switch (type)
            {
                case EStatModType.Add:
                    result += trainingData.OptionValue;
                    break;
                case EStatModType.PercentAdd:
                    result += trainingData.OptionValue;
                    break;
                case EStatModType.PercentMult:
                    result += trainingData.OptionValue;
                    break;
            }
        }

        return result;
    }
    #endregion
}