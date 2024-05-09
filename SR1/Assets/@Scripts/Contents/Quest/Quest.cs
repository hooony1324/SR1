using System;
using Data;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Quest
{
    public QuestSaveData SaveData { get; set; }

    public QuestData QuestData;

    public List<QuestTask> _questTasks = new List<QuestTask>();

    public event Action<Quest> OnQuestCompleted;

    public int TemplateId
    {
        get { return SaveData.TemplateId; }
        set { SaveData.TemplateId = value; }
    }

    public EQuestState State
    {
        get { return SaveData.State; }
        set { SaveData.State = value; }
    }

    public int DailyScore
    {
        get { return SaveData.DailyScore; }
        set { SaveData.DailyScore = value; }
    }
    
    public int WeeklyScore
    {
        get { return SaveData.WeeklyScore; }
        set { SaveData.WeeklyScore = value; }
    }

    public Quest(QuestSaveData saveData)
    {
        SaveData = saveData;
        State = saveData.State;
        QuestData = Managers.Data.QuestDic[TemplateId];

        _questTasks.Clear();

        for (int i = 0; i < QuestData.QuestTasks.Count; i++)
        {
            _questTasks.Add(new QuestTask(i, QuestData.QuestTasks[i], saveData.TaskProgressCount[i],  saveData.TaskStates[i], this));
        }
    }

    public bool IsCompleted()
    {
        for (int i = 0; i < QuestData.QuestTasks.Count; i++)
        {
            if (i >= SaveData.TaskProgressCount.Count)
                return false;

            QuestTaskData questTaskData = QuestData.QuestTasks[i];

            int progressCount = SaveData.TaskProgressCount[i];
            if (progressCount < questTaskData.ObjectiveCount)
                return false;
        }
        
        SaveData.State = EQuestState.Completed;
        return true;
    }

    public bool IsRewarded()
    {
        foreach (var questTask in _questTasks)
        {
            if (questTask.TaskState != EQuestState.Rewarded)
                return false;
        }
        SaveData.State = EQuestState.Rewarded;
        return true;
    }
    
    public QuestTask GetCurrentTask(int templateId)
    {
        return _questTasks.Find(x => x.TaskData.TemplateId == templateId);
    }
    
    public QuestTask GetCurrentTask()
    {
        int lastCompleteIdx = 0;

        for (int i = 0; i < _questTasks.Count; i++)
        {
            if (_questTasks[i].TaskState == EQuestState.None )
            {
                _questTasks[i].TaskState = EQuestState.Processing;
                return _questTasks[i];
            }

            if (_questTasks[i].TaskState == EQuestState.Processing || _questTasks[i].TaskState == EQuestState.Completed)
            {
                return _questTasks[i];
            }

            lastCompleteIdx = i;
        }

        return _questTasks[lastCompleteIdx];
    }

    public static Quest MakeQuest(QuestSaveData saveData)
    {
        if (Managers.Data.QuestDic.TryGetValue(saveData.TemplateId, out QuestData questData) == false)
            return null;

        Quest quest = null;
        quest = new Quest(saveData);
        
        return quest;
    }

    public void OnHandleBroadcastEvent(EBroadcastEventType eventType, ECurrencyType currencyType, int value)
    {
        GetCurrentTask().OnHandleBroadcastEvent(eventType, currencyType, value);
        UpdateQuest();
    }

    public void UpdateQuest()
    {
        //Task들의 Count 업데이트
        for (int i = 0; i < _questTasks.Count; i++)
        {
            SaveData.TaskProgressCount[i] = _questTasks[i].Count;
            SaveData.TaskStates[i] = _questTasks[i].TaskState;
        }

        if (IsCompleted()) //퀘스트 클리어
        {
            State = EQuestState.Completed;
            OnQuestCompleted?.Invoke(this);

            IsRewarded();
        }
    }
}