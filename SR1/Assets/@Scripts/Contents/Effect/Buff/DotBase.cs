public class DotBase : EffectBase
{
    public override void ApplyEffect()
    {
        base.ApplyEffect();
        
        if (EffectType == Define.EEffectType.Infinite)
        {
            Remains = float.MaxValue;
            StartCoroutine(StartTimer());
        }
        else
        {
            StartCoroutine(StartTimer());
        }
    }

    protected override void ProcessDot()
    {
        if(Owner.IsValid() == false)
            return;
        float damage = 0;
        
        switch (EffectData.calcStatType)
        {
            case Define.ECalcStatType.Default:
                damage = EffectData.Amount;
                break;
            case Define.ECalcStatType.SourceHp:
                damage = Source.MaxHp * (1 + EffectData.PercentAdd);
                break;
            case Define.ECalcStatType.Hp:
                damage = Owner.Hp * (1 + EffectData.PercentAdd);
                if (isHeal())
                {
                    damage *= -1f;
                }

                break;
            case Define.ECalcStatType.SourceAtk:
                damage = Source.Atk + EffectData.Amount + (Source.Atk * EffectData.PercentAdd);
                if (EffectData.PercentMult > 0)
                {
                    damage *= EffectData.PercentMult;
                }

                break;
            case Define.ECalcStatType.MaxHp:
                damage = Owner.MaxHp * (1 + EffectData.PercentAdd);
                break;
        }

        Owner.OnDamage(Source, damage);
    }

    private bool isHeal()
    {
        return EffectData.Amount < 0;
    }
}