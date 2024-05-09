using System.Collections.Generic;
using System.Linq;
using Data;
using DG.Tweening;
using UnityEngine;
using static Define;

//
public class ItemHolder : BaseObject
{
    // Owner
    // HolderSprite? (상자? 고기?)
    // DespawnTime

    private ItemData _data;
    private SpriteRenderer _currentSprite;
    private ParabolaMotion _parabolaMotion;
    private RewardData _reward;
    private Color _color = new Color(1f, 1f, 1f, 1f);
    
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = EObjectType.ItemHolder;
        _currentSprite = gameObject.GetOrAddComponent<SpriteRenderer>();
        _parabolaMotion = gameObject.GetOrAddComponent<ParabolaMotion>();
        _currentSprite.sortingOrder = SortingLayers.DROP_ITEM;
        
        return true;
    }

    public void SetInfo(RewardData reward, Vector2 startPos, Vector2 pos)
    {
        _reward = reward;
        _data = Managers.Data.ItemDic[reward.ItemTemplateId];
        _currentSprite.color = _color;
        _parabolaMotion.SetInfo(startPos, pos ,null ,null, 3f, endCallback: Arrived);

        AcquireItem();
    }

    void Arrived()
    {
        //획득한 재화를 슬라이더나 인벤토리로 이동시켜줄시 1초로 해도 될듯
        _currentSprite.DOFade(0, 0.5f).SetDelay(0.5f).OnComplete(() =>
        {
            // if (_data != null)
            // {
            //     AcquireItem();
            // }

            Managers.Object.Despawn(this);
        });
    }

    private void AcquireItem()
    {
        switch (_data.ItemGroupType)
        {
            case EItemGroupType.Equipment:
                if (Managers.Inventory.IsInventoryFull())
                {
                    Managers.UI.ShowToast("Inventory is full.");
                    break;
                }
                int itemId = GetRandomEquipment();
                if (Managers.Data.EquipmentDic.TryGetValue(itemId, out EquipmentData equipData))
                {
                    Managers.Inventory.MakeItem(itemId);
                    _currentSprite.sprite = Managers.Resource.Load<Sprite>(equipData.SpriteName);
                    Managers.UI.ShowToast($"Items : {equipData.Name}");
                }
                break;
            case EItemGroupType.Consumable:
                break;
            case EItemGroupType.Currency:
                CurrencyData currencyData = _data as CurrencyData;
                if (currencyData != null)
                {
                    _currentSprite.sprite = Managers.Resource.Load<Sprite>(_data.SpriteName);
                    Managers.Inventory.EarnCurrency(currencyData.currencyType, _reward.Count);
                }
                break;
        }
    }

    private EItemSubType[] equipmentItems = new EItemSubType[]
    {
        EItemSubType.PinkRune,
        EItemSubType.RedRune,
        EItemSubType.YellowRune,
        EItemSubType.MintRune,
    };
    
    private int GetRandomEquipment()
    {
        EquipmentData[] equipments = Managers.Data.EquipmentDic.Values.ToArray();

        //1. 드롭할 장비 파츠 결정(헬멧 장갑 무기 등)
        EItemSubType subType = ChooseSubEquipmentType();
        
        //2. 등급 결정
        EItemGrade grade = Util.ChooseItemGrade();

        List<EquipmentData> filtered = equipments.Where(e => e.SubType == subType && e.Grade == grade).ToList();

        
        EquipmentData selected = null;
        if (filtered.Count > 0)
        {
            selected = filtered[UnityEngine.Random.Range(0, filtered.Count)];
        }
        else
        {
            Debug.LogError("WHY?");
        }
        
        if (selected != null)
            return selected.DataId;
        
        return -1;
    }
    
    private EItemSubType ChooseSubEquipmentType()
    {
        int index = UnityEngine.Random.Range(0, equipmentItems.Length);
        return equipmentItems[index];
    }
}
