using Data;
using System.Collections.Generic;
using UnityEngine;

public class UI_AddMonsterPopup_Cheat : UI_Popup
{
    enum GameObjects
    {
        AddMonstersList,
        Background,
    }

    enum Buttons
    {
        CloseButton,
    }

    MonsterData _monsterData;

    List<UI_AddMonster_MonsterItem_Cheat> _monsterItemList = new List<UI_AddMonster_MonsterItem_Cheat>();

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));

        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);

        gameObject.BindEvent(OnPointerDown, null, type: Define.UIEvent.PointerDown);

        Refresh();

        return true;
    }

    public void SetInfo()
    {


        Refresh();
    }

    void Refresh()
    {
        if (_init == false)
            return;

        foreach (KeyValuePair<int, MonsterData> dict in Managers.Data.MonsterDic)
        {
            UI_AddMonster_MonsterItem_Cheat item = Managers.UI.MakeSubItem<UI_AddMonster_MonsterItem_Cheat>(GetObject((int)GameObjects.AddMonstersList).transform);
            item.SetInfo(dict.Value, this);
            _monsterItemList.Add(item);
        }

    }

    public void ResetMonsterList()
    {
        for(int i=0; i< _monsterItemList.Count;i++)
        {
            _monsterItemList[i].UnselectMonster();
        }
        _monsterData = null;
    }

    public void SetMonster(MonsterData monsterData)
    {
        _monsterData = monsterData;
    }

    void OnClickCloseButton()
    {
        Managers.UI.ClosePopupUI();
    }

    void OnPointerDown()
    {
        if (_monsterData == null)
            return;

        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.5f);  

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        //Managers.Object.Spawn<Monster>(worldPos, _monsterData.DataId);

        Grid cellGrid = Managers.Map.CellGrid;

        Vector3Int cellPos = cellGrid.WorldToCell(worldPos);
        //Vector3Int spawnPos = new Vector3Int((int)worldPos.x, (int)worldPos.y);

        //Monster monster = Managers.Object.Spawn<Monster>(Cell2World(cellPos), tile.DataId);
        //monster.SetCellPos(cellPos, true);

        Monster monster = Managers.Object.Spawn<Monster>(worldPos, _monsterData.TemplateId);
        monster.SetCellPos(cellPos, true);
    }
}
