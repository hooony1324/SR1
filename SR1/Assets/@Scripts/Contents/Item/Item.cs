using Data;
using System.Collections.Generic;
using static Define;

public class Item
{
    public ItemSaveData SaveData { get; set; }

	public int InstanceId
	{
		get { return SaveData.InstanceId; }
		set { SaveData.InstanceId = value; }
	}

	public long DbId
	{
		get { return SaveData.DbId; }
	}

	public int TemplateId
	{
		get { return SaveData.TemplateId; }
		set { SaveData.TemplateId = value; }
	}

	public int Count
	{
		get { return SaveData.Count; }
		set { SaveData.Count = value; }
	}

	public int EquipSlot
	{
		get { return SaveData.EquipSlot; }
		set { SaveData.EquipSlot = value; }
	}

	public int EnchantCount
	{
		get { return SaveData.EnchantCount; }
		set { SaveData.EnchantCount = value; }
	}

	public bool IsLock
	{
		get { return SaveData.IsLock; }
		set { SaveData.IsLock = value; }
	}

	public ItemData TemplateData
	{
		get
		{
			return Managers.Data.ItemDic[TemplateId];
		}
	}

	public EItemType ItemType { get; private set; }
	public EItemSubType SubType { get; private set; }

	public Item(ItemSaveData saveData)
	{
		SaveData = saveData;
		InstanceId = saveData.InstanceId;
		Count = saveData.Count;
		
		TemplateId = saveData.TemplateId;
		ItemType = TemplateData.Type;
		SubType = TemplateData.SubType;
	}

	public virtual bool Init()
	{
		return true;
	}

	public static Item MakeItem(ItemSaveData itemInfo)
	{
		if (Managers.Data.ItemDic.TryGetValue(itemInfo.TemplateId, out ItemData itemData) == false)
			return null;

		Item item = null;

		switch (itemData.ItemGroupType)
		{
			case EItemGroupType.Equipment:
				item = new Equipment(itemInfo);
				break;
			case EItemGroupType.Consumable:
				item = new Consumable(itemInfo);
				break;
			case EItemGroupType.Currency:
				item = new Currency(itemInfo);
				break;
		}

		return item;
	}

	#region Helpers
	public bool IsEquippable()
	{
		return GetEquipItemEquipSlot() != EEquipSlotType.None;
	}

	public EEquipSlotType GetEquipItemEquipSlot()
	{
		switch (SubType)
		{
			case EItemSubType.PinkRune:
				return EEquipSlotType.Pink;
			case EItemSubType.RedRune:
				return EEquipSlotType.Red;
			case EItemSubType.YellowRune:
				return EEquipSlotType.Yellow;
			case EItemSubType.MintRune:
				return EEquipSlotType.Mint;
		}

		return EEquipSlotType.None;
	}

	public bool IsEquippedItem()
	{
		return SaveData.EquipSlot > (int)EEquipSlotType.None && SaveData.EquipSlot < (int)EEquipSlotType.EquipMax;
	}

	public bool IsInInventory()
	{
		return SaveData.EquipSlot == (int)EEquipSlotType.Inventory;
	}

	public bool IsInWarehouse()
	{
		return SaveData.EquipSlot == (int)EEquipSlotType.WareHouse;
	}

	#endregion
}

public class EquipmentOption
{
	public ECalcStatType CalcStatType;
	public EStatModType StatModType;
	public float OptionValue;
	public bool isMainOption;

	public EquipmentOption(ECalcStatType statType, EStatModType modType, float value,bool isMain)
	{
		CalcStatType = statType;
		StatModType = modType;
		OptionValue = value;
		isMainOption = isMain;
	}
}

public class Equipment : Item
{
	public List<EquipmentOption> Options = new List<EquipmentOption>(); 

	protected EquipmentData EquipmentData { get { return (EquipmentData)TemplateData; } }
	
	public Equipment(ItemSaveData saveData) : base(saveData)
	{
		Init();
	}

	public EquipmentOption MainOption {
		get
		{
			return Options.Find(x => x.isMainOption == true);
		}
	}

	public int ItemScore
	{
		get { return CalculateItemScore(); }
	}

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		if (TemplateData == null)
			return false;

		if (TemplateData.Type != EItemType.Equipment)
			return false;

		EquipmentData data = (EquipmentData)TemplateData;
		{
			//MainOption + SubOption
			EquipmentOption equipmentData = new EquipmentOption(
				data.CalcStatType, data.StatModType, data.BonusValue, true);
			
			Options.Add(equipmentData);

			foreach (var optionId in SaveData.OptionIds)
			{
				if (Managers.Data.EquipmentOptionDic.TryGetValue(optionId, out EquipmentOptionData equipOptionData))
				{
					EquipmentOption subOption = new EquipmentOption(
						equipOptionData.CalcStatType, equipOptionData.StatModType, equipOptionData.OptionValue, false);
					Options.Add(subOption);
				}
			}
		}

		return true;
	}

	public float GetStatModifier( ECalcStatType calcStatType, EStatModType type)
	{
		float result = 0;

		foreach (var equipmentOption in Options)
		{
			if(calcStatType != equipmentOption.CalcStatType)
				continue;
			
			if(equipmentOption.StatModType != type)
				continue;
			
			switch (type)
			{
				case EStatModType.Add:
					result += equipmentOption.OptionValue;
					break;
				case EStatModType.PercentAdd:
					result += equipmentOption.OptionValue;
					break;
				case EStatModType.PercentMult:
					result += equipmentOption.OptionValue;
					break;
			}
		}

		return result;
	}

	public void Enchant(int count = 1)
	{
		EnchantCount++;
		MainOption.OptionValue += 2f;
	}

	public int CalculateRequiredMaterials()
	{
		return (EquipmentData.ItemLevel * 2) + EnchantCount;
	}

	public int CalculateItemScore()
	{
		float score = 0;

		//dragon 임시 계산

		foreach (var option in Options)
		{
			switch (option.StatModType)
			{
				case EStatModType.Add:
					score += option.OptionValue;
					break;
				case EStatModType.PercentAdd:
					score += option.OptionValue * 100f;
					break;
			}
		}
		return (int)score;
	}
}

public class Currency : Item
{
	public Currency(ItemSaveData saveData) : base(saveData)
	{
		Init();
	}

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		if (TemplateData == null)
			return false;

		if (TemplateData.ItemGroupType != EItemGroupType.Currency)
			return false;

		CurrencyData data = (CurrencyData)TemplateData;
		return true;
	}
}

public class Consumable : Item
{
	public double Value { get; private set; }

	public Consumable(ItemSaveData saveData) : base(saveData)
	{
		Init();
	}

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		if (TemplateData == null)
			return false;

		if (TemplateData.Type != EItemType.Potion || TemplateData.Type != EItemType.Scroll)
			return false;

		ConsumableData data = (ConsumableData)TemplateData;
		{
			Value = data.Value;
		}

		return true;
	}
}
