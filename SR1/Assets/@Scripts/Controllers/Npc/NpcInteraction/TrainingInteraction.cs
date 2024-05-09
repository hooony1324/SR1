using static Define;

public class TrainingInteraction : INpcInteraction
{
	private Npc _owner;
	public bool IsActiveWaypoint { get; set; } // 웨이포인트 활성화 true false

	public void SetInfo(Npc owner)
	{
		_owner = owner;
		//TODO 데이터저장 필요 Active정보 받기
		IsActiveWaypoint = true;
	}

	public bool CanInteract()
	{
		return true;
	}

	public void HandleOnClickEvent()
	{
		Managers.Object.HeroCamp.MoveToTarget(_owner.Position, () =>
		{
			UI_TrainingPopup popup = Managers.UI.ShowPopupUI<UI_TrainingPopup>();
			popup.SetInfo();
		});
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
