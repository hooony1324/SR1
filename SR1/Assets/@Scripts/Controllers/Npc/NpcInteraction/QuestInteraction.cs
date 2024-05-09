using UnityEngine;
using static Define;

public class QuestInteraction : INpcInteraction
{
    private Npc _owner;
    private Quest _quest;
    private Vector3Int _lastCellPos;

    public void SetInfo(Npc owner)
    {
        _owner = owner;

        if (Managers.Quest.AllQuests.TryGetValue(owner.Data.QuestDataId, out _quest) == false)
            return;

        _quest.OnQuestCompleted += HandleQuestClear;
    }

    public bool CanInteract()
    {
        if (_quest == null)
            return false;
        if (_quest.State == EQuestState.Rewarded)
            return false;
        if (_quest.GetCurrentTask(_owner.Data.QuestTaskDataId).TaskState == EQuestState.Rewarded)
        {
            return false;
        }

        return true;
    }

    public void HandleOnClickEvent()
    {
        Managers.Object.HeroCamp.MoveToTarget(_owner.Position, DoWork);
    }

    private void DoWork()
    {
        QuestTask questTask = _quest.GetCurrentTask(_owner.Data.QuestTaskDataId);

        if (questTask == null)
            return;

        if (questTask.TaskData.TemplateId != _owner.Data.QuestTaskDataId)
            return;

        if (questTask.TaskState != EQuestState.Processing)
            return;

        switch (questTask.TaskData.ObjectiveType)
        {
            case EQuestObjectiveType.Click:
                questTask.Count++;
                questTask.GiveReward();
                break;
            case EQuestObjectiveType.SpendMeat:
                if (Managers.Inventory.SpendCurrency(ECurrencyType.Meat, questTask.TaskData.ObjectiveCount) == false)
                {
                }

                break;
            case EQuestObjectiveType.SpendGold:
                if (Managers.Inventory.SpendCurrency(ECurrencyType.Gold, questTask.TaskData.ObjectiveCount) == false)
                {
                }

                break;
            case EQuestObjectiveType.SpendMineral:
                if (Managers.Inventory.SpendCurrency(ECurrencyType.Mineral, questTask.TaskData.ObjectiveCount) == false)
                {
                }

                break;
            case EQuestObjectiveType.SpendWood:
                if (Managers.Inventory.SpendCurrency(ECurrencyType.Wood, questTask.TaskData.ObjectiveCount) == false)
                {
                }

                break;

            case EQuestObjectiveType.KillMonster:
                break;
            case EQuestObjectiveType.ClearDungeon:
                UI_DungeonEntrancePopup popup = Managers.UI.ShowPopupUI<UI_DungeonEntrancePopup>();
                popup.SetInfo(() =>
                {
                    _lastCellPos = Managers.Game.Leader.CellPos;
                    Managers.Map.StageTransition.OnMapChanged(DUNGEON_IDNEX);
                    Managers.Game.TeleportHeroes(Managers.Map.StageTransition.Dungeon.StartSpawnInfo.CellPos);
                    Managers.Map.StageTransition.Dungeon.Quest = _quest;
                });

                break;
        }
    }

    private void HandleQuestClear(Quest quest)
    {
        QuestTask questTask = _quest.GetCurrentTask();
        if (questTask == null)
            return;

        switch (questTask.TaskData.ObjectiveType)
        {
            case EQuestObjectiveType.KillMonster:
                break;
            case EQuestObjectiveType.ClearDungeon:
                // changeanimation
                questTask.GiveReward();
                _owner.TeleportHeroes(_lastCellPos);
                break;
            default:
                break;
        }
    }

    private bool CheckNearby()
    {
        bool checkNearby = Managers.Object.FindCircleRangeTargets(_owner.Position, 5, EObjectType.Hero, true).Count > 0;

        if (checkNearby == false)
        {
            return false;
        }

        return true;
    }
}