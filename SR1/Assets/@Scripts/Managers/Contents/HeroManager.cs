using Data;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class HeroManager
{
    public bool isInit = false;
    public Dictionary<int, HeroInfo> AllHeroInfos { get; set; } = new Dictionary<int, HeroInfo>();

    public List<HeroInfo> PickedHeroes
    {
        get { return AllHeroInfos.Values.Where(h => h.OwningState == HeroOwningState.Picked).ToList(); }
    }

    public List<HeroInfo> OwnedHeroes
    {
        get { return AllHeroInfos.Values.Where(h => h.OwningState == HeroOwningState.Owned).ToList(); }
    }

    public List<HeroInfo> UnownedHeroes
    {
        get { return AllHeroInfos.Values.Where(h => h.OwningState == HeroOwningState.Unowned).ToList(); }
    }

    public HeroSaveData MakeHeroInfo(int templateId)
    {
        if (Managers.Data.HeroInfoDic.TryGetValue(templateId, out HeroInfoData heroInfoData) == false)
            return null;

        HeroSaveData saveData = new HeroSaveData()
        {
            TemplateId = heroInfoData.templateId,
            Level = 1,
            Exp = 0,
            OwningState = HeroOwningState.Unowned
        };

        AddHeroInfo(saveData);
        return saveData;
    }

    public HeroInfo AddHeroInfo(HeroSaveData saveData)
    {
        HeroInfo heroInfo = HeroInfo.MakeHeroInfo(saveData);        
        if (heroInfo == null)
            return null;

        AllHeroInfos.Add(heroInfo.TemplateId, heroInfo);
        return heroInfo;
    }

    public bool CanPick()
    {
        return PickedHeroes.Count < Managers.Game.MaxTeamCount;
    }

    public Hero PickHero(int templateId, Vector3Int joinCellPos)
    {
        HeroInfo heroInfo = GetHeroInfo(templateId);
        
        if (heroInfo == null)
        {
            Debug.Log("영운존재안함");
            return null;
        }
        
        heroInfo.OwningState = HeroOwningState.Picked;

        Hero hero;
        if (joinCellPos == Vector3.zero)
        {
            Vector3Int randCellPos = Managers.Game.GetNearbyPosition(null, Managers.Game.Leader.CellPos);
            hero = Managers.Object.Spawn<Hero>(randCellPos, templateId);
            Managers.Map.MoveTo(hero, randCellPos, true);
            // hero.SetCellPos(randCellPos, true);
        }
        else
        {
            hero = Managers.Object.Spawn<Hero>(joinCellPos, templateId);
            Managers.Map.MoveTo(hero, joinCellPos, true);
            // hero.SetCellPos(joinCellPos, true);
        }
        
        Managers.Game.BroadcastEvent(EBroadcastEventType.ChangeTeam, 0);

        return hero;
    }

    public void UnpickHero(int heroId)
    {
        if (AllHeroInfos.TryGetValue(heroId, out HeroInfo info) == false)
            return;

        if (info.OwningState == HeroOwningState.Picked)
        {
            info.OwningState = HeroOwningState.Owned;
            var heroes = Managers.Object.Heroes.ToList();
            Hero despawnHero = heroes.Find(hero => hero.TemplateId == heroId);
            Managers.Object.Despawn(despawnHero);

            Managers.Game.BroadcastEvent(EBroadcastEventType.ChangeTeam, value: heroId);
        }
    }

    public void AcquireHeroCard(int heroId, int exp)
    {
        if (AllHeroInfos.TryGetValue(heroId, out HeroInfo heroInfo) == false)
            return;

        if(heroInfo.OwningState == HeroOwningState.Unowned)
            heroInfo.OwningState = HeroOwningState.Owned;
        heroInfo.Exp += exp;
        
        Managers.UI.ShowToast($"{(heroInfo.HeroData.DescriptionTextID) + exp}");
    }
    
    public void AddUnknownHeroes()
    {
        foreach (HeroInfoData heroInfo in Managers.Data.HeroInfoDic.Values.ToList())
        {
            if (AllHeroInfos.ContainsKey(heroInfo.templateId))
                continue;

            MakeHeroInfo(heroInfo.templateId);
        }
    }

    
    #region Helper

    public int GetBattlePower()
    {
        int battlePower = 0;
        foreach (var heroInfo in PickedHeroes)
        {
            battlePower += (int)heroInfo.CombatPower;
        }

        return battlePower;
    }

    public HeroInfo GetHeroInfo(int templateId)
    {
        if (AllHeroInfos.TryGetValue(templateId, out HeroInfo heroInfo))
        {
            return heroInfo;
        }
        else
        {
            return null;
        }
    }


    #endregion
}