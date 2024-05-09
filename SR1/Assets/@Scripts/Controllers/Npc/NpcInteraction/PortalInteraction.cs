using static Define;

public class PortalInteraction : INpcInteraction
{
	private Npc _owner;

	public Npc DestPortal { get; set; }
	public bool IsTownPortal { get; set; }
	
	public void SetInfo(Npc owner)
	{
		_owner = owner;
	}
	
	public void ConnectPortal(Npc destPortal, bool isTownPortal)
	{
		DestPortal = destPortal;
		IsTownPortal = isTownPortal;

		if (IsTownPortal == false)
		{
			(destPortal.Interaction as PortalInteraction).DestPortal = _owner;
		}
	}

	public bool CanInteract()
	{
		if (DestPortal.IsValid() == false)
			return false;

		return true;
	}

	public void HandleOnClickEvent()
	{
			if (CanInteract())
			{
				Managers.Object.HeroCamp.MoveToTarget(_owner.Position, () =>
				{
					Managers.Game.TeleportHeroes(DestPortal.Position);
					Managers.Map.StageTransition.CheckMapChanged(Managers.Game.Leader.Position);
				});
			}

			if (IsTownPortal)
			{
				Managers.Object.Despawn(DestPortal);
			}  
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
