using System;
using UnityEditor.Build.Content;
using UnityEngine;

public class ParticleController : InitBase
{
    private ParticleSystem _ps;
    private Action OnStopped;
    
    protected override bool Init()
    {
        base.Init();
        _ps = GetComponent<ParticleSystem>();
        return true;
    }

    private void OnEnable()
    {
        var main = _ps.main;
        main.stopAction = ParticleSystemStopAction.Callback;
        
        _ps.Play();
    }

    public void SetInfo(Action action)
    {
        OnStopped = action;
    }

    public void OnParticleSystemStopped()
    {
        OnStopped?.Invoke();
        Managers.Resource.Destroy(gameObject);
    }
    
}