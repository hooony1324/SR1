using System;
using System.Collections.Generic;
using UnityEngine;
using static Util;

public class Define
{

    public const char MAP_TOOL_WALL = '0';
    public const char MAP_TOOL_NONE = '1'; 
    public const char MAP_TOOL_SEMI_WALL = '2';	

    public const float EFFECT_SMALL_RADIUS = 2.5f;
    public const float EFFECT_NORMAL_RADIUS = 4.5f;
    public const float EFFECT_BIG_RADIUS = 5.5f;
    
    public const int HERO_DEFAULT_MOVE_DEPTH = 8;
    public const int MONSTER_DEFAULT_MOVE_DEPTH = 3;

    public const int PORTAL_DATA_ID = 500002;
    public const float MIX_DURATION = 0.4f;
    public const float MONSTER_RESPONE_TIME = 30;
    public const float ENV_RESPONE_TIME = 30;
    
    public static string MapName = "BaseMap";

    //가챠
    public const int GACHA_COST = 100;
    public const int REFRESH_GACHA_COST = 50;

    public const int WEEKLY_QUEST_RESET_DAY = 1;
    public const int QUEST_RESET_HOUR = 5;

    //Range
    public const int SCAN_RANGE = 12;

    public const int BASIC_DETECTION_RANGE = 7;
    public const int EXTRA_VERTICAL_DETECTION_RANGE = 9;

    public const int BASIC_DETECTION_RANGE_ON_IDLE = 12;
    public const int EXTRA_VERTICAL_DETECTION_RANGE_ON_IDLE = 13;
    
    public const int NPC_SCAN_RANGE = 10;

    public static readonly float[] ITEM_GRADE_PROB = new float[]
    {
        0,
        0.10f,   // Normal 확률
        0.15f,   // Rare 확률
        0.20f,   // Epic 확률
        0.55f,  // Legendary 확률
        //
        
        
        // 0.55f,   // Normal 확률
        // 0.20f,   // Rare 확률
        // 0.15f,   // Epic 확률
        // 0.10f,  // Legendary 확률
    };

    public const int DUNGEON_IDNEX = 99;
    // public static map
    public static readonly Dictionary<Type, Array> _enumDict = new Dictionary<Type, Array>();

    #region Enum

    public enum ECampState
    {
        Idle,
        Move,
        MoveToTarget,
        CampMode
    }

    public enum EFindRangeType
    {
        None,
        Circle,
        Cone,
        Rectangle,
        Single,
    }
    
    public enum EProjDetectType
    {
        Cell,
        Update
    }

    public enum ECameraState
    {
        Following,
        Targeting,
    }
    
    public enum ETrainingMainOption
    {
        None,
        UnlockPortal,
        MaxMembers,
        UnlockCamp,
        Critical,
    }
    
    public enum EDamageResult
    {
        None,
        Hit,
        CriticalHit,
        Miss,
        Heal,
        CriticalHeal
    }

    public enum EQuestPeriodType
    {
        Once, // 단발성
        Daily,
        Weekly,
        Infinite, // 무한으로
    }

    public enum EQuestCondition
    {
        None,
        Level,
        ItemLevel,

    }

    public enum EQuestObjectiveType
    {
        KillMonster,
        EarnMeat,
        SpendMeat,
        EarnWood,
        SpendWood,
        EarnMineral,
        SpendMineral,
        EarnGold,
        SpendGold,
        UseItem,
        Survival,
        ClearDungeon,
        Click,
    }

    public enum EQuestRewardType
    {
        Hero,
        Gold,
        Mineral,
        Meat,
        Wood,
        Item,
        DailyScore,
        WeeklyScore,
    }

    public enum EQuestState
    {
        None,
        Processing,
        Completed,
        Rewarded,
    }
    
    public enum EEquipSlotType
    {
        None,
        Red = 1,
        Pink = 2,
        Mint = 3,
        Yellow = 4,
        EquipMax,

        Inventory = 100,
        WareHouse = 200,
    }
    
    public enum EBroadcastEventType
    {
        None,
        KillMonster,
        HeroLevelUp,
        DungeonClear,
        ChangeInventory,
        ChangeTeam,
        PlayerLevelUp,
        UnlockTraining,
        ChangeCurrency,
        ChangeCampState,
        ChangeSetting,
        HeroDead,
    }
    
    public enum EItemGrade
    {
        None,
        Normal,
        Rare,
        Epic,
        Legendary
    }

    public enum EItemGroupType
    {
        None,
        Equipment,
        Consumable,
        Currency
    }

    public enum EItemType
    {
        None,
        Equipment,
        Potion,
        Scroll
    }

    public enum EItemSubType
    {
        None,
        PinkRune,
        RedRune,
        YellowRune,
        MintRune,

        EnchantWeapon,
        EnchantArmor,

        HealthPotion,
        ManaPotion,
    }
    
    public enum ECurrencyType
    {
        None,
        Wood,
        Mineral,
        Meat,
        Gold,
        Fragments,
        Dia,
        Ruby,
        ForestMarble
        
    }
    
    public enum HeroOwningState
    {
        Unowned,
        Owned,
        Picked,
    }
    
    public enum EAoEType
    {
        ConeShape,
        CircleShape,
        SingleTarget,
        CircleTrigger,
    }

    public enum ENpcType
    {
        None,
        StartPosition,
        Guild,
        Portal,
        Waypoint,
        BlackSmith,
        Training,
        TreasureBox,
        Quest,
        GoldStorage,
        WoodStorage,
        MineralStorage,
        Exchange,
        RuneStone,
    }

    public enum EEffectSize
    {
        CircleSmall,
        CircleNormal,
        CircleBig,
        ConeSmall,
        ConeNormal,
        ConeBig,
        Single,
    }
    
    public enum EFindPathResult
    {
        Fail_LerpCell,
        Fail_NoPath,
        Fail_MoveTo,
        Success,
        SamePosition,//같은좌표에서 길찾기했을때
    }

    public enum ECellCollisionType
    {
        None,
        SemiWall,
        Wall,
    }

    public enum EEffectSpawnType
    {
        Skill,// 지속시간이 있는 기본적인 이펙트 
        External , // 외부(장판스킬)에서 이펙트를 관리(지속시간에 영향을 받지않음)
    }
    
    public enum EEffectClearType
    {
        TimeOut,// 시간초과로 인한 Effect 종료
        ClearSkill,// 정화 스킬로 인한 Effect 종료
        TriggerOutAoE,// AoE스킬을 벗어난 종료
        EndOfCC ,// 에어본, 넉백이 끝난 경우 호출되는 종료
        Disable
    }

    public enum EStatModType
    {
        Add,
        PercentAdd,
        PercentMult,
    }

    public enum ESkillSlot
    {
        Default,
        Env,
        A,
        B
    }

    public enum ESkillReqLevel
    {
        A_Level_1 = 1,
        A_Level_2 = 10,
        A_Level_3 = 15,
        B_Level_1 = 5,
        B_Level_2 = 20,
        B_Level_3 = 30,
    }
    public enum ECalcStatType  
    {
        None,
        Default,
        SourceHp,//상대방의 체력
        SourceAtk,//상대방의 공격력
        
        Hp,
        MaxHp,
        Critical,
        CriticalDamage,
        ReduceDamageRate,
        ReduceDamage,
        LifeStealRate,
        ThornsDamageRate,
        
        AttackSpeedRate,
        MissChance,
        Atk,
        MoveSpeed,
        CooldownReduction,
        Thorns,
        
        Count,
    }

    public enum EEffectType
    {
        Instant,
        Buff,
        Debuff,
        Dot,
        Infinite,
        Knockback,
        Airborne,
        Freeze,
        Stun,
        Pull,
    }


    public enum ELanguage
    {
        Korean,
        English,
        French,
        SimplifiedChinese,
        TraditionalChinese,
        Japanese
    }
    
    public enum EHeroMoveState
    {
        None,
        TargetMonster,
        CollectEnv,
        ReturnToCamp,
        ForceMove,
        ForcePath,
    }

    public enum EIndicatorType
    {
        None,
        Cone,
        Rectangle,
    }
    
    public enum ELayer
    {
        Default = 0,
        TransparentFX = 1,
        IgnoreRaycast = 2,
        Dummy1 = 3,
        Water = 4,
        UI = 5,
        Hero = 6,
        Monster = 7,
        Boss = 8,
        //
        Env = 11,
        Obstacle = 12,
        //
        Projectile = 20,
    }

    public enum EBuildType
    {
        Remote, // 실제디바이스 에서 aws에서 로드
        Editor_Local, // 에디터에서 로컬에서 로드
        Editor_Remote // 애다터 aws에서 로드
    }

    public enum ESizeUnits
    {
        Byte, 
        KB, 
        MB, 
        GB
    }
    
    public enum EAnimationState
    {
        Attack,
        End,
    }
    
    public enum EJoystickState
    {
        PointerDown,
        Drag,
        PointerUp
    }

    public enum ESkillType
    {
        None,
        NormalAttack,
        AreaSkill,
        ComboSkill,
        ProjectileSkill,
        SingleTargetSkill,
        SupportSkill,
        PassiveSkill
    }
    public enum ECreatureState
    {
        Idle,
        Cooltime,
        Skill,
        Move,
        OnDamaged,
        Dead
    }
    
    public enum EEnvState
    {
        Idle,
        OnDamaged,
        Dead
    }

    public enum EObjectType
    {
        None,
        Hero,
        Monster,
        Camp,
        Env,
        ItemHolder,
        Npc,
        Projectile,
    }

    public enum EEnvType
    {
        Wood,
        Mineral
    }

    public enum EJoystickType
    {
        Fixed,
        Flexible
    }
    public enum EScene
    {
        Unknown,
        TitleScene,
        LobbyScene,
        GameScene,
        ArtTestScene,
    }

    public enum ESound
    {
        Bgm,
        SubBgm,
        Effect,
        Max,
    }

    public enum UIEvent
    {
        Click,
        Preseed,
        PointerDown,
        PointerUp,
        BeginDrag,
        Drag,
        EndDrag,
    }
    public enum EToastColor 
    {
        Black,
        Red,
        Purple,
        Magenta,
        Blue,
        Green,
        Yellow,
        Orange
    }

    public enum EToastPosition 
    {
        TopLeft,
        TopCenter,
        TopRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }
    
    public enum EStorageState
    {
        None,
        Transferring,
    }

    public enum EProjetionMotion
    {
        Straight,
        Parabola
    }

    #endregion

}

public static class SortingLayers
{
    public const int SPELL_INDICATOR = 200;
    public const int HERO = 300;
    public const int NPC = 300;
    public const int MONSTER = 300;
    public const int BOSS = 300;
    public const int GATHERING_RESOURCES = 300;
    public const int PROJECTILE = 310;
    public const int DROP_ITEM = 310;
    public const int SKILL_EFFECT = 315;
    public const int DAMAGE_FONT = 410;
}

public static class AnimName
{
    public const string ATTACK = "attack";
    public const string IDLE = "idle";
    public const string MOVE = "move";
    public const string DAMAGED = "hit";
    public const string DEAD = "dead";
    public const string EVENT_ATTACK_A ="event_attack";
    public const string EVENT_ATTACK_B ="event_attack";
    public const string EVENT_SKILL_A = "event_attack";    
    public const string EVENT_SKILL_B = "event_attack";
}

public static class EquipmentUIColors
{
    #region 장비 이름 색상
    public static readonly Color COMMON_NAME = HexToColor("3E4C68");
    public static readonly Color RARE_NAME = HexToColor("1D75E2");
    public static readonly Color EPIC_NAME = HexToColor("73438E");
    public static readonly Color LEGEND_NAME = HexToColor("C2590E");
    #endregion
    #region 테두리 색상
    public static readonly Color COMMON_OUTLINE = HexToColor("949DB3");
    public static readonly Color RARE_OUTLINE = HexToColor("6C9BF2");
    public static readonly Color EPIC_OUTLINE = HexToColor("A876C4");
    public static readonly Color LEGEND_OUTLINE = HexToColor("F19451");
    #endregion
    // #region 배경색상
    // public static readonly Color EpicBg = HexToColor("D094FF");
    // public static readonly Color LegendaryBg = HexToColor("F8BE56");
    // public static readonly Color MythBg = HexToColor("FF7F6E");
    // #endregion
}

public static class MoveDir
{
    public static Vector2 BOTTOM = new Vector2(0f, -1f);
    public static Vector2 BOTTOM_LEFT = new Vector2(-0.894f, -0.447f);
    public static Vector2 BOTTOM_RIGHT = new Vector2(0.894f, -0.447f);
    public static Vector2 TOP = new Vector2(0f, 1f);
    public static Vector2 TOP_LEFT = new Vector2(-0.894f, 0.447f);
    public static Vector2 TOP_RIGHT = new Vector2(0.894f, 0.447f);
    public static Vector2 LEFT = new Vector2(-1f, 0f);
    public static Vector2 RIGHT = new Vector2(1f, 0f);
}
