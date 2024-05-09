using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using static Define;

public class QuestManager
{
    public Dictionary<int, Quest> AllQuests = new Dictionary<int, Quest>();

    public Quest MainQuest
    {
        get
        {
            int mainQuestId = 8;
            return AllQuests[mainQuestId];
        }
    }

    public void Init()
    {
        Managers.Game.OnBroadcastEvent -= OnHandleBroadcastEvent;
        Managers.Game.OnBroadcastEvent += OnHandleBroadcastEvent;
    }

    public void AddUnknownQuests()
    {
        foreach (QuestData questData in Managers.Data.QuestDic.Values.ToList())
        {
            if (AllQuests.ContainsKey(questData.TemplateId))
                continue;

            QuestSaveData questSaveData = new QuestSaveData()
            {
                TemplateId = questData.TemplateId,
                State = Define.EQuestState.None,
                NextResetTime = DateTime.MaxValue,
            };

            for (int i = 0; i < questData.QuestTasks.Count; i++)
                questSaveData.TaskProgressCount.Add(0);

            AddQuest(questSaveData);
        }
    }

    public void CheckWaitingQuests()
    {
        // TODO
    }

    public void CheckProcessingQuests()
    {
        foreach (Quest quest in AllQuests.Values)
        {
            if (quest.State == EQuestState.Processing)
                quest.UpdateQuest();
        }    
    }

    public Quest AddQuest(QuestSaveData questInfo)
    {
        Quest quest = Quest.MakeQuest(questInfo);
        if (quest == null)
            return null;

        AllQuests.Add(quest.TemplateId, quest);

        return quest;
    }

    public void Clear()
    {
        AllQuests.Clear();
    }

    void OnHandleBroadcastEvent(EBroadcastEventType eventType, ECurrencyType currencyType, int value)
    {
        foreach (Quest quest in AllQuests.Values)
        {
            if (quest.State == EQuestState.Processing)
                quest.OnHandleBroadcastEvent(eventType, currencyType, value);
        }
    }

    #region Helper

    public int GetQuestScore(EQuestRewardType rewardType)
    {
        int sum = 0;
        switch (rewardType)
        {
            case EQuestRewardType.DailyScore:
                foreach (var quest in AllQuests.Values)
                {
                    sum += quest.DailyScore;
                }
                break;
            case EQuestRewardType.WeeklyScore:
                foreach (var quest in AllQuests.Values)
                {
                    sum += quest.WeeklyScore;
                }
                break;
        }

        return sum;
    }

    public List<Quest> GetDailyQuests()
    {
        return AllQuests.Values.Where(x => x.QuestData.QuestPeriodType == EQuestPeriodType.Daily).ToList();
    }

    public List<Quest> GetWeeklyQuests()
    {
        return AllQuests.Values.Where(x => x.QuestData.QuestPeriodType == EQuestPeriodType.Weekly).ToList();
    }

    public Quest GetQuest(int templateId)
    {
        if (AllQuests.TryGetValue(templateId, out Quest quest))
        {
            return quest;
        }

        return null;
    }

    #endregion
}