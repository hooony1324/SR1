using UnityEngine;
using Spine.Unity;
using Data;

public class UI_AddMonster_MonsterItem_Cheat : UI_Base
{
    enum Buttons
    {
        MonsterButton,
    }

    enum Images
    {
        SelectedImage,
    }

    [SerializeField]
    SkeletonGraphic _spine;

    MonsterData _monsterData;
    UI_AddMonsterPopup_Cheat _addMonsterPopup_CheatUI;

    bool _isSelected;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));
        BindImage(typeof(Images));

        GetButton((int)Buttons.MonsterButton).gameObject.BindEvent(OnClickMonsterButton);
        GetImage((int)Images.SelectedImage).gameObject.SetActive(false);

        Refresh();

        return true;
    }


    public void SetInfo(MonsterData monsterData, UI_AddMonsterPopup_Cheat popup)
    {
        _monsterData = monsterData;
        _addMonsterPopup_CheatUI = popup;

        Refresh();
    }

    void Refresh()
    {
        if (_init == false)
            return;

        if (_monsterData == null)
            return;

        _spine.skeletonDataAsset = Managers.Resource.Load<SkeletonDataAsset>(_monsterData.SkeletonDataID);
        
        _spine.Initialize(true);
    }

    void OnClickMonsterButton()
    {
        bool temp = _isSelected;

        _addMonsterPopup_CheatUI.ResetMonsterList();

        if(temp == false)
        {
            _isSelected = true;
            GetImage((int)Images.SelectedImage).gameObject.SetActive(true);
            _addMonsterPopup_CheatUI.SetMonster(_monsterData);
        }
    }

    public void UnselectMonster()
    {
        _isSelected = false;
        GetImage((int)Images.SelectedImage).gameObject.SetActive(false);
    }
}
