using Spine;
using Spine.Unity;
using UnityEngine;
using static Define;

public class RuneStoneInteraction : INpcInteraction
{
	private Npc _owner;

	private GameObject _redSocket;
	private GameObject _yellowSocket;
	private GameObject _pinkSocket;
	private GameObject _mintSocket;
	
	public void SetInfo(Npc owner)
	{
		_owner = owner;
		Managers.Game.OnBroadcastEvent += HandleOnBroadcastEvent;
		Managers.Game.OnBroadcastEvent += HandleOnBroadcastEvent;

		_redSocket = Managers.Resource.Instantiate("RuneStoneSlot", _owner.transform);
		_yellowSocket = Managers.Resource.Instantiate("RuneStoneSlot", _owner.transform);
		_pinkSocket = Managers.Resource.Instantiate("RuneStoneSlot", _owner.transform);
		_mintSocket = Managers.Resource.Instantiate("RuneStoneSlot", _owner.transform);

		_redSocket.name = "RedSocket";
		_yellowSocket.name = "YellowStone";
		_pinkSocket.name = "PinkSocket";
		_mintSocket.name = "MintSocket";
		
		_redSocket.transform.position = SetSocket("red_socket");
		_yellowSocket.transform.position = SetSocket("yellow_socket");
		_pinkSocket.transform.position = SetSocket("pink_socket");
		_mintSocket.transform.position =  SetSocket("mint_socket");

		SetEquipments();
	}

	public bool CanInteract()
	{
		return true;
	}

	public void HandleOnClickEvent()
	{
		Managers.Object.HeroCamp.MoveToTarget(_owner.Position, () =>
		{
			UI_EquipPopup popup = Managers.UI.ShowPopupUI<UI_EquipPopup>();
			popup.SetInfo();
		});

	}
	
	private void HandleOnBroadcastEvent(EBroadcastEventType eventType, ECurrencyType currencyType, int value)
	{
		switch (eventType)
		{
			case EBroadcastEventType.ChangeInventory:
				SetEquipments();
				break;
		}
	}

	private void SetEquipments()
	{
		UpdateEquipmentSprite(EEquipSlotType.Red, _redSocket);
		UpdateEquipmentSprite(EEquipSlotType.Yellow, _yellowSocket);
		UpdateEquipmentSprite(EEquipSlotType.Mint, _mintSocket);
		UpdateEquipmentSprite(EEquipSlotType.Pink, _pinkSocket);
	}

	private void UpdateEquipmentSprite(EEquipSlotType slotType, GameObject socket)
	{
		Item rune = Managers.Inventory.GetEquippedItem(slotType);
		if (rune != null)
		{
			Sprite sprite = Managers.Resource.Load<Sprite>(rune.TemplateData.SpriteName);
			socket.GetComponent<SpriteRenderer>().sprite = sprite;
		}
		else
		{
			socket.GetComponent<SpriteRenderer>().sprite = null;
		}
	}
	
	private Vector3 SetSocket(string slotName)
	{
		Attachment attachment = _owner.SkeletonAnim.Skeleton.GetAttachment(slotName, slotName);
		if (attachment == null)
		{
			Debug.Log("attachment not found");
			return Vector3.zero;
		}

		PointAttachment point = attachment as PointAttachment;
		Slot slot = _owner.SkeletonAnim.Skeleton.FindSlot(slotName);
		Vector3 retPos = point.GetWorldPosition(slot, _owner.SkeletonAnim.transform);

		return retPos;
	}
	
	private bool CheckNearby()
	{
		bool checkNearby = Managers.Object.FindCircleRangeTargets(_owner.Position, 5, EObjectType.Hero, true).Count > 0;
        
		if (checkNearby == false)
		{
			return false;
		}
		return true;
	}
}
