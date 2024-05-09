using System;
using UnityEngine;
using UnityEngine.AdaptivePerformance.Provider;
using static Define;

public class StorageInteraction : INpcInteraction
{
	private Npc _owner;
	private Storage _storage;
	private EStorageState _state = EStorageState.None;
	private UI_StoragePopup _popup;
	private bool isNearby = false;
	
	public void SetInfo(Npc owner)
	{
		_owner = owner;
		switch (_owner.Data.NpcType)
		{
			case ENpcType.GoldStorage:
				if (Managers.Game.Storages.TryGetValue(ECurrencyType.Gold, out _storage) == false)
				{
					Managers.UI.ShowToast("골드 저장소 찾기 실패");
					return;
				}
				break;
			case ENpcType.WoodStorage:
				if (Managers.Game.Storages.TryGetValue(ECurrencyType.Wood, out _storage) == false)
				{
					Managers.UI.ShowToast("목재 저장소 찾기 실패");
					return;
				}
				break;
			case ENpcType.MineralStorage:
				if (Managers.Game.Storages.TryGetValue(ECurrencyType.Mineral, out _storage) == false)
				{
					Managers.UI.ShowToast("광산 저장소 찾기 실패");
					return;
				}
				break;
		}

		if (_storage.LastRewardTime == DateTime.MinValue)
		{
			_storage.LastRewardTime = DateTime.Now;
		}
		_owner.InteractionUI.Refresh();
	}
	
	public bool CanInteract()
	{
		return true;
	}

	public void HandleOnClickEvent()
	{
		Managers.Object.HeroCamp.MoveToTarget(_owner.Position, () =>
		{
			_popup = Managers.UI.ShowPopupUI<UI_StoragePopup>();
			_popup.SetInfo(_storage);
		});
	}
	
	public void TransferResourcesToPlayer()
    {
        int Quantity = _storage.GetStoredQuantity();
        if (Quantity <= 0)
        {
            return;
        }

        Managers.Inventory.EarnCurrency(_storage.StorageData.currencyType, Quantity);
        _storage.AddStoredQuantity(Quantity * -1);
        _owner.InteractionUI.Refresh();

        _storage.LastRewardTime = DateTime.Now;
    }

    public void Refresh()
    {
        //마을에 있을때만 계산
        if (_owner.SpawnStage.StageIndex != Managers.Map.StageTransition.CurrentStageIndex)
        {
            return;
        }
        
        DetectNearbyAndTransfer();
        UpdateAnimation();
        
        var currentTime = DateTime.Now;
        var lastUpdateTime = _storage.LastRewardTime;

        double elapsedMinutesDouble = (currentTime - lastUpdateTime).TotalSeconds;
        float elapsedMinutesClamped = Mathf.Clamp((float)elapsedMinutesDouble, 0f, 1440f);
        int elapsedMinutes = (int)elapsedMinutesClamped;

        int addQuantity = elapsedMinutes * _storage.StorageData.ProductionQuantity;

        if (addQuantity > 0)
        {
            _storage.LastRewardTime = DateTime.Now;
            _storage.AddStoredQuantity(addQuantity);
            //UI update
            _owner.InteractionUI.Refresh();
        }
    }

    public void UpdateAnimation()
    {
	    string animName = "idle";
	    
	    if(_storage.GetLevel() == 0)
		    _owner.UpdateAnimation("empty_lot");
	    else if (_storage.GetStorageRatio() == 0)
	    {
		    _owner.UpdateAnimation("empty");
	    }
	    else
	    {
		    _owner.UpdateAnimation("full");
	    }
    }

    private void DetectNearbyAndTransfer()
    {
	    isNearby = Managers.Object.FindCircleRangeTargets(_owner.Position, 6, EObjectType.Hero, true).Count > 0;
        
	    if (isNearby == false)
	    {
		    _state = EStorageState.None;
	    }

	    if (_state == EStorageState.None && isNearby)
	    {
		    _state = EStorageState.Transferring;
		    TransferResourcesToPlayer();
	    }
    }

}
