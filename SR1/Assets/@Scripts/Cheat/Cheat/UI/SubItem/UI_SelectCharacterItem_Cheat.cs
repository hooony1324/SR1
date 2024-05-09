using Data;
using Spine.Unity;
using UnityEngine;

public class UI_SelectCharacterItem_Cheat : UI_Base
{
    enum Buttons
    {
        CharacterButton,
    }

    [SerializeField]
    SkeletonGraphic _spine;

    CreatureData _creatureData;

    UI_SelectCharacterPopup_Cheat _selectAnimationPopup;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));

        GetButton((int)Buttons.CharacterButton).gameObject.BindEvent(OnClickCharacterButton);

        Refresh();

        return true;
    }

    public void SetInfo(CreatureData creatureData, UI_SelectCharacterPopup_Cheat popup)
    {
        _creatureData = creatureData;
        _selectAnimationPopup = popup;

        Refresh();
    }

    void Refresh()
    {
        if (_init == false)
            return;
        if (_creatureData == null)
            return;

        _spine.skeletonDataAsset = Managers.Resource.Load<SkeletonDataAsset>(_creatureData.SkeletonDataID);
        _spine.Initialize(true);
    }

    void OnClickCharacterButton()
    {
        if (_creatureData == null)
            return;

        _selectAnimationPopup.SetAnimation(_creatureData);
    }
}
