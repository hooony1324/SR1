using System;
using System.Collections;
using Data;
using UnityEngine;
using Spine.Unity;

public class Projectile : BaseObject
{
    protected Creature _owner;
    protected SkillBase _skill;
    protected ProjectileData _projectileData;
    protected SpriteRenderer _projectileSprite;
    protected GameObject _spineObject;
    protected GameObject _spriteObject;
    protected Vector3 endPos;
    protected event Action<Vector3, Vector3, int> _eventMotionFinished;
    
    protected override void OnDisable()
    {
        StopAllCoroutines();
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        //_projectileSprite.sortingOrder = SortingLayers.PROJECTILE;
        ObjectType = Define.EObjectType.Projectile;

        _spineObject = Util.FindChild<SkeletonAnimation>(gameObject, "Spine").gameObject;
        _spriteObject = Util.FindChild<SpriteRenderer>(gameObject).gameObject;
        return true;
    }

    public virtual void SetInfo(Creature owner, SkillBase skill,  Action<Vector3, Vector3, int> onMotionFinished)
    {
        _spineObject.SetActive(true);
        _spriteObject.SetActive(true);

        int projDataId = skill.SkillData.ProjectileId;
        _projectileData = Managers.Data.ProjectileDic[projDataId];
        if (string.IsNullOrEmpty(_projectileData.SpineName))
        {
            _projectileSprite = Util.FindChild<SpriteRenderer>(gameObject);
            _projectileSprite.sortingOrder = SortingLayers.PROJECTILE;
            _projectileSprite.sprite = Managers.Resource.Load<Sprite>(_projectileData.SpriteName);
            _spineObject.SetActive(false);
        }
        else
        {
            _spriteObject.SetActive(false);
            SetSpineAnimation(_projectileData.SpineName, SortingLayers.PROJECTILE, "Spine");
            Flip(true);
            PlayAnimation(0, AnimName.IDLE, true);
        }
        _owner = owner;
        _skill = skill;
        _eventMotionFinished = onMotionFinished;
        
        ProjectileMotion motion;
        switch (_projectileData.ProjectileMotion)
        {
            case Define.EProjetionMotion.Straight:
                motion = gameObject.GetOrAddComponent<StraightMotion>();
                break;
            case Define.EProjetionMotion.Parabola:
                motion = gameObject.GetOrAddComponent<ParabolaMotion>();
                break;
            default:
                motion = gameObject.GetOrAddComponent<StraightMotion>();
                break;
        }

        if (motion != null)
        {
            endPos = _skill.SkillTarget.CenterPosition;
            motion.SetInfo(Position, endPos,  _skill.SkillTarget, _projectileData, endCallback: OnMotionFinished);
        }

        if (gameObject.IsValid())
            StartCoroutine(CoReserveDestroy());
    }

    protected virtual void OnMotionFinished()
    {
        switch(_projectileData.ProjectileMotion)
        {
            case Define.EProjetionMotion.Straight:
                if (_skill.SkillTarget.IsValid())
                {
                    _skill.SkillTarget.Effects.GenerateEffects(_skill.SkillData.EffectIds, Define.EEffectSpawnType.Skill, _owner);
                }
                break;
            case Define.EProjetionMotion.Parabola:
                var targets = Managers.Object.FindCircleRangeTargets(gameObject.transform.position, 1f, _owner.ObjectType);
                foreach (var target in targets)
                {
                    if (target.IsValid())
                    {
                        target.Effects.GenerateEffects(_skill.SkillData.EffectIds, Define.EEffectSpawnType.Skill, _owner);
                    }
                }
                break;
        }
        
        _eventMotionFinished?.Invoke(transform.position, endPos, _skill.SkillData.TempleteId);
        DestroyProjectile();
    }

    private IEnumerator CoReserveDestroy()
    {
        yield return new WaitForSeconds(5f);
        DestroyProjectile();
    }

    private void DestroyProjectile()
    {
        Managers.Object.DespawnProjectile(this);
    }

}
