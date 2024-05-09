using System;
using Spine;
using Event = Spine.Event;

public class EnvSkill : SkillBase
{
    private Env _envtarget;

    protected override bool Init()
    {
        base.Init();
        
        return true;
    }

    public override void DoSkill(Action callback = null)
    {
        string animName = "gather_mine";;
        SkillTarget = Owner.Target;

        _envtarget = SkillTarget as Env;
        if (_envtarget)
        {
            //데이터아이디가 301000 보다 작으면 나무
            if (_envtarget.TemplateId < 301000)
            {
                animName = "gather_wood";
            }

        }

        SkillTrackEntry = Owner.PlayAnimation(0, animName, false);
        SkillTrackEntry.TimeScale = 1;
        SkillAnimDuration = SkillTrackEntry.Animation.Duration;
    }

    protected override void OnOwnerAnimEventHandler(TrackEntry arg1, Event arg2)
    {
        if (arg1.Animation.Name.Contains("gather"))
        {
            OnAttackEvent(arg1, arg2);
        }
    }
    protected override void OnAttackEvent(TrackEntry arg1, Event arg2)
    {
        base.OnAttackEvent(arg1, arg2);

        if (!_envtarget.IsValid()) 
            return;

        _envtarget.OnDamage(Owner, 1);
    }

}