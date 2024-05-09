using Spine;

public class SelfCenteredAreaSkill : AreaSkill
{
    protected override void OnAttackEvent(TrackEntry arg1, Spine.Event arg2)
    {
        if (arg1.Animation.Name != SkillData.AnimName)
            return;
        GenerateAoE(Owner.Position, SkillTarget.Position);
    }
}
