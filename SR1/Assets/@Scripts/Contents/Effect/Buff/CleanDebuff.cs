using static Define;

//TODO  Skill 로 빼기
public class CleanDebuff : EffectBase
{
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        return true;
    }
    
    public override void ApplyEffect()
    {
        base.ApplyEffect();
        Owner.Effects.ClearEffectsByCondition(effect => effect.EffectType == EEffectType.Debuff);
        ClearEffect(EEffectClearType.TimeOut);
    }

}