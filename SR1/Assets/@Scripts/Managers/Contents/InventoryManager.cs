using System;
using Data;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static Define;

public class InventoryManager
{
	public const int DEFAULT_INVENTORY_SLOT_COUNT = 50;

	public int MaxWood
	{
		get { return Managers.Game.SaveData.MaxWood; }
		private set { Managers.Game.SaveData.MaxWood = value; }
	}
	
	public int MaxMineral
	{
		get { return Managers.Game.SaveData.MaxMineral; }
		private set { Managers.Game.SaveData.MaxMineral = value; }
	}

	public int MaxMeat
	{
		get { return Managers.Game.SaveData.MaxMeat; }
		private set { Managers.Game.SaveData.MaxMeat = value; }
	}
    
	public List<Item> AllItems { get; } = new List<Item>();

	// Cache
	Dictionary<int /*EquipSlot*/, Item> EquippedItems = new Dictionary<int, Item>(); // 장비 인벤
	Dictionary<ECurrencyType, Item> Currencys = new Dictionary<ECurrencyType, Item>(); // 
	List<Item> InventoryItems = new List<Item>(); // 가방 인벤
	List<Item> WarehouseItems = new List<Item>(); // 창고

	public Item MakeItem(int itemTemplateId, int count = 1)
	{
		int itemDbId = Managers.Game.GenerateItemDbId();

		if (Managers.Data.ItemDic.TryGetValue(itemTemplateId, out ItemData itemData) == false)
			return null;

		//장비아이템 인 경우 옵션을 추가한다.
		List<int> optionIds = new List<int>();
		if (Managers.Data.EquipmentDic.TryGetValue(itemTemplateId, out EquipmentData equipmentData))
		{
			if(equipmentData.SubOptionCount > 0)
				optionIds = GenerateOptionIds(equipmentData);
		}

		ItemSaveData saveData = new ItemSaveData()
		{
			InstanceId = itemDbId,
			DbId = itemDbId,
			TemplateId = itemTemplateId,
			Count = count,
			EquipSlot = (int)EEquipSlotType.Inventory,
			EnchantCount = 0,
			OptionIds = optionIds
		};

		return AddItem(saveData);
	}

	public Item AddItem(ItemSaveData itemInfo)
	{
		Item item = Item.MakeItem(itemInfo);
		if (item == null)
			return null;

		if (item.IsEquippedItem())
		{
			EquippedItems.Add(item.SaveData.EquipSlot, item);
		}
		else if (item.IsInInventory())
		{
			InventoryItems.Add(item);
		}
		else if (item.IsInWarehouse())
		{
			WarehouseItems.Add(item);
		}

		if (item.TemplateData.ItemGroupType == EItemGroupType.Currency)
		{
			CurrencyData data = (CurrencyData)item.TemplateData;
			Currencys.Add(data.currencyType, item);
		}

		AllItems.Add(item);
		Managers.Game.BroadcastEvent(EBroadcastEventType.ChangeInventory, 0);
		return item;
	}

	public void RemoveItem(int instanceId)
	{
		Item item = AllItems.Find(x => x.SaveData.InstanceId == instanceId);
		if (item == null)
			return;

		if (item.IsEquippedItem())
		{
			EquippedItems.Remove(item.SaveData.EquipSlot);
		}
		else if (item.IsInInventory())
		{
			InventoryItems.Remove(item);
		}
		else if (item.IsInWarehouse())
		{
			WarehouseItems.Remove(item);
		}
		AllItems.Remove(item);
		Managers.Game.BroadcastEvent(EBroadcastEventType.ChangeInventory, 0);
	}

	public void EquipItem(int instanceId)
	{
		Item item = InventoryItems.Find(x => x.SaveData.InstanceId == instanceId);
		if (item == null)
		{
			Debug.Log("아이템존재안함");
			return;
		}

		EEquipSlotType equipSlotType = item.GetEquipItemEquipSlot();
		if (equipSlotType == EEquipSlotType.None)
			return;

		// 기존 아이템 해제
		if (EquippedItems.TryGetValue((int)equipSlotType, out Item prev))
			UnEquipItem(prev.InstanceId);

		// 아이템 장착
		item.EquipSlot = (int)equipSlotType;
		EquippedItems[(int)equipSlotType] = item;
		InventoryItems.Remove(item);
		Managers.Game.BroadcastEvent(EBroadcastEventType.ChangeInventory, 0);
	}

	public void UnEquipItem(int instanceId, bool checkFull = true)
	{
		var item = EquippedItems.Values.Where(x => x.InstanceId == instanceId).FirstOrDefault();
		if (item == null)
			return;

		// TODO

		if (checkFull && IsInventoryFull())
			return;

		EquippedItems.Remove((int)item.EquipSlot);

		item.EquipSlot = (int)EEquipSlotType.Inventory;
		InventoryItems.Add(item);
		Managers.Game.BroadcastEvent(EBroadcastEventType.ChangeInventory, 0);
	}

	public void EnchantItem(int instanceId, int count = 1)
	{
		Equipment item = GetItem(instanceId) as Equipment;
		
		if (item == null)
		{
			Debug.Log("아이템존재안함");
			return;
		}
		
		item.Enchant(count); 
		Managers.Game.BroadcastEvent(EBroadcastEventType.ChangeInventory, 0);
	}

	public void DismantleItem(int instanceId)
	{
		Equipment item = GetItem(instanceId) as Equipment;

		if(item == null)
			return;
		if(item.IsEquippedItem())
			return;
		
		int earn = (int)(item.CalculateRequiredMaterials() * 0.5f);
		
		EarnCurrency(ECurrencyType.Fragments, earn);
		RemoveItem(instanceId);
	}

	public float GetStatModifier(ECalcStatType calcStatType, EStatModType type)
	{
		float value = 0;
		foreach (var item in EquippedItems.Values)
		{
			Equipment equipment = item as Equipment;
			if (equipment != null)
			{
				value += equipment.GetStatModifier(calcStatType, type);
			}
		}

		return value;
	}

	#region 자원관리
	public int GetCurrency(ECurrencyType type)
	{
		if (Currencys.TryGetValue(type, out Item currency))
		{
			return currency.Count;
		}
		else
		{
			Debug.LogError($"Currency Error!! :  {type}");
		}

		return -1;
	}
	
    public bool CheckCurrency(ECurrencyType eCurrencyType, int amount)
    {
	    int currency = GetCurrency(eCurrencyType);
	    if (currency < 0)
		    return false;
	    
	    if(currency >= amount)
	    {
		    return true;
	    }
	    
	    return GetCurrency(eCurrencyType) >= amount;
    }

    public bool SpendCurrency(ECurrencyType eCurrencyType, int amount)
    {
        if (!CheckCurrency(eCurrencyType, amount))
        {
            Managers.UI.ShowToast("@@자원이 부족합니다.");
            return false;
        }

        if (Currencys.TryGetValue(eCurrencyType, out Item currency))
        {
	        int prev = currency.Count;
	        currency.Count = Mathf.Max(0, currency.Count - amount);
	        int diff = currency.Count - prev;
	        Managers.Game.BroadcastEvent(EBroadcastEventType.ChangeCurrency, eCurrencyType, diff );
        }
        else
        {
	        return false;
        }
        return true;
    }

    public void EarnCurrency(ECurrencyType eCurrencyType, int amount)
    {
	    if (Currencys.TryGetValue(eCurrencyType, out Item currency) == false)
	    {
		    return;
	    }

	    int prev = currency.Count;
	    currency.Count = Mathf.Clamp(currency.Count + amount, 0, GetMax(eCurrencyType));
	    int diff = currency.Count - prev;
	    Managers.Game.BroadcastEvent(EBroadcastEventType.ChangeCurrency, eCurrencyType, diff );
    }

    public int GetMax(ECurrencyType type)
    {
	    switch (type)
	    {
		    case ECurrencyType.Wood:
			    return MaxWood;
		    case ECurrencyType.Mineral:
			    return MaxMineral;
		    case ECurrencyType.Meat:
			    return MaxMeat;
		    case ECurrencyType.Fragments:
		    case ECurrencyType.Ruby:
		    case ECurrencyType.ForestMarble:
			    return 9999;
		    case ECurrencyType.Gold:
		    case ECurrencyType.Dia:
			    return int.MaxValue;
	    }

	    return 9999;
    }

    #endregion
    
	public void Clear()
	{
		AllItems.Clear();

		EquippedItems.Clear();
		InventoryItems.Clear();
		WarehouseItems.Clear();
	}

	#region Random Option Generator

	private List<int> GenerateOptionIds(EquipmentData equipmentData)
	{
		// 아이템 옵션 만들기
		// Equipment의 ItemLevel과 같은 OptionData 적용
		// OptionGrade는 Equipment의 EquipmentGrade와 같을 필요는 없음.
		// (Rare 장비에 Legendary Option이 뜰 수도 있음.)
		// 하나의 장비에 같은 OptionType이 2개 존재할 수 없음.ㅇ
		
		List<int> optionIds = new List<int>();
		ECalcStatType[] optionTypes = ChooseRandomItemOptions(equipmentData.SubOptionCount);
		
		List<EquipmentOptionData> options = Managers.Data.EquipmentOptionDic.Values.ToList();
		foreach (var optionType in optionTypes)
		{
			EItemGrade grade = Util.ChooseItemGrade();

			var filtered = options.Where(e =>   e.ItemLevel == equipmentData.ItemLevel
			                                    && e.CalcStatType == optionType
			                                    && e.OptionGrade == grade).ToList();
			
			if (filtered.Count > 0)
			{
				EquipmentOptionData selected = filtered[UnityEngine.Random.Range(0, filtered.Count)];
				optionIds.Add(selected.DataId);
			}
		}
		return optionIds;
	}

	private ECalcStatType[] ChooseRandomItemOptions(int count)
	{
		var values = Enum.GetValues(typeof(ECalcStatType)).Cast<ECalcStatType>()
			.Where(e => e != ECalcStatType.Hp && e != ECalcStatType.Count).ToArray();
        
		values.Shuffle();
		return values.Take(count).ToArray();
	}
	
	#endregion
	
	#region Helper
	public Item GetItem(int instanceId)
	{
		return AllItems.Find(item => item.InstanceId == instanceId);
	}

	public Item GetEquippedItem(EEquipSlotType equipSlotType)
	{
		EquippedItems.TryGetValue((int)equipSlotType, out Item item);

		return item;
	}

	public Item GetEquippedItem(int instanceId)
	{
		return EquippedItems.Values.Where(x => x.InstanceId == instanceId).FirstOrDefault();
	}

	public Item GetEquippedItemBySubType(EItemSubType subType)
	{
		return EquippedItems.Values.Where(x => x.SubType == subType).FirstOrDefault();
	}

	public Item GetItemInInventory(int instanceId)
	{
		return InventoryItems.Find(x => x.SaveData.InstanceId == instanceId);
	}

	public bool IsInventoryFull()
	{
		return InventoryItems.Count >= InventorySlotCount();
	}

	public int InventorySlotCount()
	{
		return DEFAULT_INVENTORY_SLOT_COUNT;
	}

	public List<Item> GetItemsByGroupType(EItemGroupType groupType)
	{
		return AllItems.Where(x => x.TemplateData.ItemGroupType == groupType).ToList();
	}
	
	public int GetCountByGroupType(EItemGroupType groupType)
	{
		return AllItems.Count(x => x.TemplateData.ItemGroupType == groupType);
	}
	
	public List<Item> GetEquippedItems()
	{
		return EquippedItems.Values.ToList();
	}

	public List<ItemSaveData> GetEquippedItemInfos()
	{
		return EquippedItems.Values.Select(x => x.SaveData).ToList();
	}

	public List<Item> GetInventoryItems()
	{
		return InventoryItems.ToList();
	}

	public List<ItemSaveData> GetInventoryItemInfos()
	{
		return InventoryItems.Select(x => x.SaveData).ToList();
	}

	public List<ItemSaveData> GetInventoryItemInfosOrderbyGrade()
	{
		return InventoryItems.OrderByDescending(y => (int)y.TemplateData.Grade)
						.ThenBy(y => (int)y.TemplateId)
						.Select(x => x.SaveData)
						.ToList();
	}

	public List<ItemSaveData> GetWarehouseItemInfos()
	{
		return WarehouseItems.Select(x => x.SaveData).ToList();
	}

	public List<ItemSaveData> GetWarehouseItemInfosOrderbyGrade()
	{
		return WarehouseItems.OrderByDescending(y => (int)y.TemplateData.Grade)
									.ThenBy(y => (int)y.TemplateId)
									.Select(x => x.SaveData)
									.ToList();
	}

	public int GetEquippedItemScore()
	{
		int score = 0;
		foreach (var item in EquippedItems.Values)
		{
			Equipment equipment = item as Equipment;
			
			if(equipment == null)
				continue;

			score += equipment.ItemScore;
		}

		return score;
	}

	#endregion
}
