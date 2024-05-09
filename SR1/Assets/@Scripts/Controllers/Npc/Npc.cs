using System.Collections;
using System.Collections.Generic;
using Data;
using Spine.Unity;
using UnityEngine;
using UnityEngine.PlayerLoop;
using static Define;

public interface INpcInteraction
{
    public void SetInfo(Npc owner);
    public void HandleOnClickEvent();
    public bool CanInteract();
}

public class Npc : InteractionObject
{
    public NpcData Data;

    [field: SerializeField] public INpcInteraction Interaction { get; private set; }

    private SkeletonAnimation _skeletonAnim;
    public UI_NpcInteraction InteractionUI;

    private string _currentAnimName;
    
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = EObjectType.Npc;
        return true;
    }

    private IEnumerator CheckInteraction()
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);
        yield return wait;
        while (true)
        {
            //같은 스테이지에 있을때만 확인함
            if (SpawnStage.StageIndex != Managers.Map.StageTransition.CurrentStageIndex)
                yield return wait;
            
            if (Interaction != null && Interaction.CanInteract())
            {
                InteractionUI.gameObject.SetActive(true);
            }
            else
            {
                InteractionUI.gameObject.SetActive(false);
                if(Data.NpcType == ENpcType.Quest)
                    PlayAnimation(0, "complete", true);

            }

            switch (Data.NpcType)
            {
                case ENpcType.GoldStorage:
                case ENpcType.WoodStorage:
                case ENpcType.MineralStorage:
                    (Interaction as StorageInteraction).Refresh();
                    break;
            }

            yield return wait;
        }
    }

    public virtual void SetInfo(int templateId)
    {
        Dictionary<int, Data.NpcData> dict = Managers.Data.NpcDic;
        TemplateId = templateId;
        Data = dict[templateId];
        ExtraCells = 1;
    
        #region Spine Animation

        SetSpineAnimation(Data.SkeletonDataID, SortingLayers.NPC, gameObject.name);
        SetFireSocket();
        PlayAnimation(0, AnimName.IDLE, true);

        #endregion

        //Npc 상호작용을 위한 버튼
        GameObject button = Managers.Resource.Instantiate("UI_NpcInteraction", gameObject.transform);
        InteractionUI = button.GetComponent<UI_NpcInteraction>();
        InteractionUI.SetInfo(this);

        switch (Data.NpcType)
        {
            case ENpcType.Quest:
                Interaction = new QuestInteraction();
                break;
            case ENpcType.Portal:
                Interaction = new PortalInteraction();
                break;
            case ENpcType.Waypoint:
                Interaction = new WaypointInteraction();
                break;
            case ENpcType.BlackSmith:
                Interaction = new ExchangeInteraction();
                break;
            case ENpcType.Training:
                Interaction = new TrainingInteraction();
                break;
            case ENpcType.TreasureBox:
                break;
            case ENpcType.GoldStorage:
            case ENpcType.WoodStorage:
            case ENpcType.MineralStorage:
                Interaction = new StorageInteraction();
                
                break;
            case ENpcType.Exchange:
                Interaction = new ExchangeInteraction();
                break;
            case ENpcType.RuneStone:
                Interaction = new RuneStoneInteraction();
                break;
            case ENpcType.Guild:
                Interaction = new GuildInteraction();
                break;
        }

        Interaction?.SetInfo(this);

        StartCoroutine(CheckInteraction());
    }
    
    public void UpdateAnimation(string animName)
    {
        if (_currentAnimName != animName)
        {
            PlayAnimation(0, animName, true);
            _currentAnimName = animName;
            InteractionUI.OnUpdateAnimation();
        }
    }

    public virtual void OnClickEvent()
    {
        Interaction?.HandleOnClickEvent();
    }

    public void TeleportHeroes(Vector3Int pos, float delay = 0)
    {
        StartCoroutine(CoReturnToLastPos(pos, delay));
    }

    private IEnumerator CoReturnToLastPos(Vector3Int pos, float delay)
    {
        yield return new WaitForSeconds(delay);
        Managers.Game.TeleportHeroes(pos);
    }
}