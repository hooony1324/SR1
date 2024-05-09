using static Define;

public class BuffBase : EffectBase
{
    public override void ApplyEffect()
    {
        base.ApplyEffect();
        if (EffectType == EEffectType.Instant)
        {
            ClearEffect(EEffectClearType.Disable);
        }
        else if (EffectType == EEffectType.Infinite)
        {
            Remains = float.MaxValue;
        }
        else
        {
            StartCoroutine(StartTimer());
        }
        Owner.CalculateStat();
    }

    public override bool ClearEffect(EEffectClearType clearType)
    {
        base.ClearEffect(clearType);
        Owner.CalculateStat();
        return true;
    }
}