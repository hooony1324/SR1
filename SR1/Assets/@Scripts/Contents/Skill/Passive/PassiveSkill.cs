using System.Collections.Generic;
using static Define;

public class PassiveSkill : SkillBase
{
    private List<EffectBase> _passives = new List<EffectBase>();
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        SkillType = ESkillType.PassiveSkill;

        return true;
    }

    protected override void OnDisable()
    {
        Managers.Game.OnBroadcastEvent -= HandleOnBroadcast;
        ClearPassives();
    }

    protected void OnEnable()
    {
        Managers.Game.OnBroadcastEvent -= HandleOnBroadcast;
        Managers.Game.OnBroadcastEvent += HandleOnBroadcast;
        Invoke("ApplyPassive", 1f);
    }

    private void ApplyPassive()
    {
        ClearPassives();
        
        // 1. 아군에게 이팩트를 부여한다.
        foreach (var hero in Managers.Object.Heroes)
        {
            _passives.AddRange(ApplyEffects(hero));
            // _passives = ApplyEffects(hero);
        }
    }

    private void ClearPassives()
    {
        foreach (var passive in _passives)
        {
            passive.ClearEffect(EEffectClearType.Disable);
            // passive.Owner.Effects.RemoveEffect(passive);
        }
        _passives.Clear();
    }

    private void HandleOnBroadcast(EBroadcastEventType type, ECurrencyType currencyType, int value)
    {
        switch (type)
        {
            case EBroadcastEventType.ChangeTeam:
                ApplyPassive();
                break;
        }
    }
}
