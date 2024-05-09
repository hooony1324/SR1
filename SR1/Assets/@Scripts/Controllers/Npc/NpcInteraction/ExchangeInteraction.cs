using UnityEngine.UIElements;
using static Define;

public class ExchangeInteraction : INpcInteraction
{
	private Npc _owner;

	public void SetInfo(Npc owner)
	{
		_owner = owner;
	}

	public bool CanInteract()
	{
		return true;
	}
    
	public void HandleOnClickEvent()
	{
		//1. 캠프 를 NPC로 이동
		Managers.Object.HeroCamp.MoveToTarget(_owner.Position, () =>
		{
			//1 필요한 자원이 룬 가루 인경우
			// - 랜덤한 장비 주기
			if (_owner.Data.NpcType == ENpcType.BlackSmith)
			{
				if (Managers.Inventory.SpendCurrency(ECurrencyType.Fragments, 35))
				{
					//2. 랜덤한 아이템 드롭
					_owner.DropItem(_owner.Data.QuestDataId);
				}
			}
			else if(_owner.Data.NpcType == ENpcType.Exchange)
			{
				//2. 필요한 자원이 숲의구슬 인 경우
				//2. 랜덤한 자원 드롭
				if (Managers.Inventory.SpendCurrency(ECurrencyType.ForestMarble, 10))
				{
					//2. 랜덤한 아이템 드롭
					_owner.DropItem(_owner.Data.QuestDataId);
				}
			}
		});
	}

	private void Action()
	{
		throw new System.NotImplementedException();
	}

	private bool CheckNearby()
	{
		bool checkNearby = Managers.Object.FindCircleRangeTargets(_owner.Position, SCAN_RANGE, EObjectType.Hero, true).Count > 0;
        
		if (checkNearby == false)
		{
			return false;
		}
		return true;
	}
}
