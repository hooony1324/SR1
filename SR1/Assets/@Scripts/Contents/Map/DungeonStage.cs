using static Define;

public class DungeonStage : Stage
{
    public Quest Quest { get; set; }

    protected override void HandleOnDead(InteractionObject obj)
    {
        //TODO 던전 
        if (IsDungeon)
        {
            switch (obj.ObjectType)
            {
                case EObjectType.Monster:
                    _monsters.Remove(obj as Monster);
                    break;
            }
        }

        CheckClear();
    }

    public override void SetInfo(int stageIdx)
    {
        base.SetInfo(stageIdx);
    }

    void CheckClear()
    {
        if (_monsters.Count == 0)
        {
            // Quest.UpdateProgress(1);
            Managers.Game.BroadcastEvent(EBroadcastEventType.DungeonClear, ECurrencyType.None, 1);
        }
    }
}