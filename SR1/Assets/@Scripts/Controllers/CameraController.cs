using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;
using static Define;

public class CameraController : MonoBehaviour
{
    #region Value

    public ECameraState State { get; set; }
    private bool isReady = false;
    private InteractionObject _target;

    public InteractionObject Target
    {
        get { return _target; }
        set
        {
            _target = value;
            isReady = true;
        }
    }

    // public float Height { get; set; } = 0;
    // public float Width { get; set; } = 0;
    [SerializeField] public float smoothSpeed = 6f; // 스무딩 속도

    private int _targetOrthographicSize = 18;

    #endregion

    public void Start()
    {
        State = ECameraState.Following;
        Managers.Game.OnBroadcastEvent -= HandleOnBroadcast;
        Managers.Game.OnBroadcastEvent += HandleOnBroadcast;
    }

    public void SetCameraSize()
    {
        if (Managers.Object.HeroCamp.CampState == ECampState.CampMode)
        {
            _targetOrthographicSize = 27;
        }
        else
        {
            _targetOrthographicSize = 18;
        }
    }

    private void LateUpdate()
    {
        // Smoothly transition to the target camera size
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, _targetOrthographicSize, smoothSpeed * Time.deltaTime);
        
        HandleCameraPosition();
    }

    private void HandleCameraPosition()
    {
        if (isReady == false || State == ECameraState.Targeting)
            return;

        Vector3 targetPosition = new Vector3(Target.Position.x, Target.Position.y, -10f);
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.fixedDeltaTime);
    }

    public void TargetingCamera(InteractionObject dest)
    {
        //이미 진행중이면 리턴
        if (State == ECameraState.Targeting)
            return;

        State = ECameraState.Targeting;
        Vector3 targetPosition = new Vector3(Target.CenterPosition.x, Target.CenterPosition.y, -10f);
        Vector3 destPosition = new Vector3(dest.Position.x, dest.Position.y, -10f);

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(destPosition, 0.8f).SetEase(Ease.Linear))
            .AppendInterval(2f)
            .Append(transform.DOMove(targetPosition, 0.8f).SetEase(Ease.Linear))
            .OnComplete(() => { State = ECameraState.Following; });
    }

    private void HandleOnBroadcast(EBroadcastEventType type, ECurrencyType currencyType, int value)
    {
        switch (type)
        {
            case EBroadcastEventType.ChangeCampState:
                SetCameraSize();
                break;
        }
    }
}