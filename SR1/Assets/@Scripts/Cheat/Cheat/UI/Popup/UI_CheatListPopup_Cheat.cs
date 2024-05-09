using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class UI_CheatListPopup_Cheat : UI_Popup
{
    enum GameObjects
    {
        CloseArea
    }

    enum Buttons
    {
        AddHeroesButton,
        AddMonstersButton,
        ScaleButton,
        ForceMoveButton,
        GenerateRuneButton,
    }


    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));

        GetButton((int)Buttons.AddHeroesButton).gameObject.BindEvent(OnClickAddHeroesButton);
        GetButton((int)Buttons.AddMonstersButton).gameObject.BindEvent(OnClickAddMonstersButton);
        GetButton((int)Buttons.ScaleButton).gameObject.BindEvent(OnClickScaleButton);
        GetButton((int)Buttons.ForceMoveButton).gameObject.BindEvent(OnClickForceMoveButton);
        GetButton((int)Buttons.GenerateRuneButton).gameObject.BindEvent(OnClickGenerateRune);

        GetObject((int)GameObjects.CloseArea).BindEvent(OnClickCloseArea);

        return true;
    }

    public void SetInfo()
    {

    }

    public void Refresh()
    {

    }

    void OnClickAddHeroesButton()
    {
        Managers.UI.ClosePopupUI(this);

        Managers.UI.ShowPopupUI<UI_AddHeroesPopup_Cheat>();
    }

    void OnClickAddMonstersButton()
    {
        Managers.UI.ClosePopupUI(this);

        Managers.UI.ShowPopupUI<UI_AddMonsterPopup_Cheat>();
    }

    void OnClickScaleButton()
    {
        Managers.UI.ClosePopupUI(this);

        Managers.UI.ShowPopupUI<UI_ScalePopup_Cheat>();
    }

    void OnClickCloseArea()
    {
        Managers.UI.ClosePopupUI(this);
    }

    void OnClickForceMoveButton()
    {
    }
    
    void OnClickGenerateRune()
    {
        StartCoroutine(CoDropItem(80000));
    }
    
    private void SpawnItemHolder(int dropItemId, RewardData rewardData)
    {
        var position = Managers.Game.Leader.CenterPosition;
        var itemHolder = Managers.Object.Spawn<ItemHolder>(position, dropItemId);
        Vector2 ran = new Vector2(position.x +  UnityEngine.Random.Range(-1.5f, -1.0f), position.y);
        Vector2 ran2 = new Vector2(position.x +  UnityEngine.Random.Range(1.0f, 1.5f), position.y);
        Vector2 dropPos =  UnityEngine.Random.value < 0.5 ? ran : ran2;
        itemHolder.SetInfo( rewardData,position, dropPos);
    }
    
    IEnumerator CoDropItem(int dropItemId)
    {
        List<RewardData> rewards = GetRewards(dropItemId);
        if (rewards != null)
        {
            foreach (var reward in rewards)
            {
                SpawnItemHolder(dropItemId, reward);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
    
    List<RewardData> GetRewards(int dropItemId)
    {
        if (Managers.Data.DropTableDic.TryGetValue(dropItemId, out DropTableData dropTableData) == false)
            return null;

        if (dropTableData.Rewards.Count <= 0)
            return null;

        List<RewardData> rewardDatas = new List<RewardData>();

        int sum = 0;
        int randValue = UnityEngine.Random.Range(0, 100);

        foreach (RewardData item in dropTableData.Rewards)
        {
            if (item.Probability == 100)
            {
                //확정드롭아이템
                rewardDatas.Add(item);
                continue;
            }

            //확정드롭아이템을 제외한 아이템
            sum += item.Probability;
            if (randValue <= sum)
            {
                rewardDatas.Add(item);
                break;
            }

        }

        return rewardDatas;
    }

}
