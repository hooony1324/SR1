using System.Collections.Generic;
using Spine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class TargetCenterdAreaSkill : AreaSkill
{
    private float _scanRadius = 2f;
    public override void SetInfo(int skillId)
    {
        base.SetInfo(skillId);
        _scanRadius = Util.GetEffectRadius(SkillData.EffectSize);
    }
    
    protected override void OnAttackEvent(TrackEntry arg1, Spine.Event arg2)
    {
        // 타겟 새로 지정
        //1. 타겟만 떄리면 재미없으니깐 타겟 주변에 있는 애들중에 하나 골라서 때린다.
        List<InteractionObject> targets = Managers.Object.FindCircleRangeTargets(Owner.Target.Position, _scanRadius, Owner.ObjectType);
        Vector3 spawnPos;
        if (targets.Count == 0)
        {
            spawnPos = SkillTarget.Position;
        }
        else
        {
            spawnPos = targets[Random.Range(0, targets.Count)].Position;
        }

        GenerateAoE(spawnPos, spawnPos);
    }
    

}