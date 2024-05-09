using System;
using System.Collections;
using UnityEngine;

public abstract class ProjectileMotion : BaseObject
{
    protected Vector3 _endPos;
    protected float _speed;
    protected Vector3 _startPos;
    protected bool IsRotation = true;
    protected Action EndCallback;
    protected InteractionObject _target;
    protected abstract IEnumerator LaunchProjectile();

    protected Rigidbody2D _rigid;
    protected Collider2D _collider;

    protected override bool Init()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        return true;
    }

    public virtual void SetInfo(Vector2 startPos, Vector3 endPos, InteractionObject target, Data.ProjectileData projData, float speed = 5f,
        Action endCallback = null)
    {
        _startPos = startPos;
        _endPos = endPos;
        _speed = speed;
        _target = target;
        
        if(projData != null)
            _speed = projData.ProjSpeed;
        
        EndCallback = endCallback;
        StartCoroutine(LaunchProjectile());
    }

    protected Quaternion LookAt2D(Vector2 forward)
    {
        return Quaternion.Euler(0, 0, Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg);
    }
}
