using System;
using System.Collections;
using UnityEngine;

public class UI_NpcInteraction : UI_Base
{
    #region Enums

    enum Buttons
    {
        InteractionButton,
    }

    enum Imanges
    {
        Image
    }

    enum GameObjects
    {
        ProgressInfo,
    }

    enum Texts
    {
        ProgressText
    }

    #endregion

    private Npc _owner;
    private GameObject _objProgressInfo;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImage(typeof(Imanges));
        BindButton(typeof(Buttons));
        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));

        _objProgressInfo = GetObject((int)GameObjects.ProgressInfo);
        _objProgressInfo.SetActive(false);
        GetButton((int)Buttons.InteractionButton).gameObject.BindEvent(OnClickInteractionButton);

        GetComponent<Canvas>().worldCamera = Camera.main;

        return true;
    }

    public void SetInfo(Npc owner, string spriteName = null, string progressText = null)
    {
        _owner = owner;

        switch (_owner.Data.NpcType)
        {
            case Define.ENpcType.GoldStorage:
            case Define.ENpcType.WoodStorage:
            case Define.ENpcType.MineralStorage:
                _objProgressInfo.SetActive(true);
                transform.position = _owner.FireSocketPos;
                break;
            case Define.ENpcType.Quest:
            case Define.ENpcType.Guild:
            case Define.ENpcType.Exchange:
            case Define.ENpcType.RuneStone:
            default:
                transform.position = _owner.FireSocketPos;
                break;
        }
        
        Refresh();
    }
    
    public void Refresh()
    {
        // transform.position = _owner.FireSocketPos;
        switch (_owner.Data.NpcType)
        {
            case Define.ENpcType.GoldStorage:
                if(Managers.Game.Storages.TryGetValue(Define.ECurrencyType.Gold, out Storage goldStorage))
                    GetText((int)Texts.ProgressText).text = $"{goldStorage.GetStoredQuantity()}/{goldStorage.StorageData.MaxCapacity}";
                break;
            case Define.ENpcType.WoodStorage:
                if(Managers.Game.Storages.TryGetValue(Define.ECurrencyType.Wood, out Storage woodStorage))
                    GetText((int)Texts.ProgressText).text = $"{woodStorage.GetStoredQuantity()}/{woodStorage.StorageData.MaxCapacity}";
                break;
            case Define.ENpcType.MineralStorage:
                if(Managers.Game.Storages.TryGetValue(Define.ECurrencyType.Mineral, out Storage storage))
                    GetText((int)Texts.ProgressText).text = $"{storage.GetStoredQuantity()}/{storage.StorageData.MaxCapacity}";
                break;
            default:
                transform.localPosition = Vector3.up * (_owner.GetSpineHeight() + 0.5f);
                break;
        }
        
    }

    public void OnUpdateAnimation()
    {
        StartCoroutine(CoUpdatePosition());
    }

    IEnumerator CoUpdatePosition()
    {
        yield return new WaitForSeconds(0.2f);
        transform.position = _owner.FireSocketPos;
    }

    private void OnClickInteractionButton()
    {
        _owner.OnClickEvent();
    }
}