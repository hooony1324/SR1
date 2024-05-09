using Data;
using UnityEngine;
using static Define;

public class HeroInfo
{
    public HeroSaveData SaveData { get; set; }

    public int TemplateId
    {
        get { return SaveData.TemplateId; }
        set { SaveData.TemplateId = value; }
    }

    public int Level
    {
        get { return SaveData.Level; }
        set { SaveData.Level = value; }
    }

    public HeroOwningState OwningState
    {
        get { return SaveData.OwningState; }
        set { SaveData.OwningState = value; }
    }

    public int Exp
    {
        get { return SaveData.Exp; }
        set { SaveData.Exp = value; }
    }
    
    public HeroData HeroData { get; private set; } = new HeroData();

    public HeroInfoData HeroInfoData { get; set; }

    //Stat
    public float CombatPower { get; private set; }
    public float Atk { get; private set; }
    public float MaxHp { get; private set; }

    public int ASkillDataId { get; private set; }
    public int ASkillLevel { get; private set; }
    public int BSkillDataId { get; private set; }
    public int BSkillLevel { get; private set; }

    public HeroInfo(HeroSaveData saveData)
    {
        SaveData = saveData;
        TemplateId = saveData.TemplateId;

        if (Managers.Data.HeroDic.TryGetValue(saveData.TemplateId, out HeroData data))
            HeroData = data;

        if (Managers.Data.HeroInfoDic.TryGetValue(saveData.TemplateId, out HeroInfoData infoData))
            HeroInfoData = infoData;

        OwningState = saveData.OwningState;

        Atk = HeroData.Atk + HeroData.AtkBonus;
        MaxHp = HeroData.MaxHp;
        CombatPower = HeroData.MaxHp + HeroData.Atk; // temp

        Level = saveData.Level;
        Exp = saveData.Exp;

        // ASkill 레벨업 확인 및 업데이트
        ASkillDataId = UpdateSkillLevel(HeroData.SkillAId, ESkillSlot.A);

        // BSkill 레벨업 확인 및 업데이트
        BSkillDataId = UpdateSkillLevel(BSkillDataId, ESkillSlot.B);

        CalculateInfoStat();
    }

    public static HeroInfo MakeHeroInfo(HeroSaveData saveData)
    {
        HeroInfo heroInfo = new HeroInfo(saveData);

        return heroInfo;
    }

    #region Level System

    private int UpdateSkillLevel(int skillDataId, ESkillSlot skillSlot)
    {
        int updatedSkillDataId = 0;

        switch (skillSlot)
        {
            case ESkillSlot.A:
                if (Level >= (int)ESkillReqLevel.A_Level_3)
                {
                    if (Managers.Data.SkillDic.TryGetValue(HeroData.SkillAId + 2, out SkillData skillData))
                        updatedSkillDataId = skillData.TempleteId;
                    
                    ASkillLevel = 3;
                }
                else if (Level >= (int)ESkillReqLevel.A_Level_2)
                {
                    if (Managers.Data.SkillDic.TryGetValue(HeroData.SkillAId + 1, out SkillData skillData))
                        updatedSkillDataId = skillData.TempleteId;

                    ASkillLevel = 2;
                }
                else if (Level >= (int)ESkillReqLevel.A_Level_1)
                {
                    updatedSkillDataId = HeroData.SkillAId;;
                    ASkillLevel = 1;
                }
                break;
            case ESkillSlot.B:
                
                if (Level >= (int)ESkillReqLevel.B_Level_3)
                {
                    if (Managers.Data.SkillDic.TryGetValue(HeroData.SkillBId + 2, out SkillData skillData))
                        updatedSkillDataId = skillData.TempleteId;
                    BSkillLevel = 3;
                }
                else if (Level >= (int)ESkillReqLevel.B_Level_2)
                {
                    if (Managers.Data.SkillDic.TryGetValue(HeroData.SkillBId + 1, out SkillData skillData))
                        updatedSkillDataId = skillData.TempleteId;
                    BSkillLevel = 2;

                }
                else if (Level >= (int)ESkillReqLevel.B_Level_1)
                {
                    updatedSkillDataId = HeroData.SkillBId;
                    BSkillLevel = 1;
                }
                break;
        }

        return updatedSkillDataId;
    }

    public void AddExp(int amount)
    {
        if (IsMaxLevel())
            return;

        Exp += amount;

        // OnExpChanged?.Invoke(Exp);
    }

    public bool CanLevelUp()
    {
        return (GetExpToNextLevel() - Exp <= 0);
    }

    public void TryLevelUp()
    {
        if (!IsMaxLevel())
        {
            Exp -= GetExpToNextLevel();
            Level++;
            
            // ASkill 레벨업 확인 및 업데이트
            ASkillDataId = UpdateSkillLevel(ASkillDataId, ESkillSlot.A);
            // BSkill 레벨업 확인 및 업데이트
            BSkillDataId = UpdateSkillLevel(BSkillDataId, ESkillSlot.B);

            CalculateInfoStat();
            Managers.Game.BroadcastEvent(EBroadcastEventType.HeroLevelUp, value:Level);
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
            return (float)Exp / GetExpToNextLevel();
        }
    }

    public int GetRemainsExp()
    {
        return GetExpToNextLevel() - Exp;
    }

    public int GetExpToNextLevel()
    {
        HeroLevelData heroLevelData;
        if (Managers.Data.HeroLevelDic.TryGetValue(Level, out heroLevelData))
        {
            return heroLevelData.Exp;
        }
        else
        {
            Debug.Log("Level invalid: " + Level);
            return 100;
        }
    }

    public bool IsMaxLevel()
    {
        return Level == Managers.Data.HeroLevelDic.Count;
    }

    #endregion

    private void CalculateInfoStat()
    {
        //TODO
        Atk = HeroData.Atk + (Level * HeroData.AtkBonus);
        MaxHp = HeroData.MaxHp + (Level * HeroData.UpMaxHpBonus);
        CombatPower = HeroData.MaxHp + HeroData.Atk; // temp
    }

    #region Helpers

    public bool IsPicked()
    {
        return OwningState == HeroOwningState.Picked;
    }

    #endregion
}