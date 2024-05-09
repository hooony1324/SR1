using System.Collections;
using UnityEngine;
using static Define;

public class Env : InteractionObject
{
    public Data.EnvData EnvData;
    
    [SerializeField] private EEnvState _envState = EEnvState.Idle;
    public EEnvState EnvState
    {
        get => _envState;
        set
        {
            _envState = value;
            UpdateAnimation();
        }
    }

    public float MaxHp { get; set; }
    [field: SerializeField]public float Hp { get; set; }

    private float _flipValue;

    protected override bool Init()
    {
        base.Init();
        ObjectType = EObjectType.Env;
        _flipValue = Random.value;
        return true;
    }
    
    protected void UpdateAnimation()
    {
        switch (EnvState)
        {
            case EEnvState.Idle:
                PlayAnimation(0, "spawn", false);
                AddAnimation(0, AnimName.IDLE, true, 0);
                break;
            case EEnvState.OnDamaged:
                PlayAnimation(0, AnimName.DAMAGED, false);
                break;
            case EEnvState.Dead:
                PlayAnimation(0, AnimName.DEAD, false);
                DropItem(EnvData.DropItemId);
                Managers.Map.RemoveObject(this);
                Coroutine coroutine = StartCoroutine(CoReserveSpawn());
                break;
            default:
                break;
        }
    }   
    
    public void SetInfo(int templateId)
    {
        TemplateId = templateId;
        EnvData = Managers.Data.EnvDic[templateId];
            
        MaxHp = EnvData.MaxHp;
        Hp = EnvData.MaxHp;
        Managers.Map.MoveTo(this, CellPos);

        #region Spine Animation
        SetSpineAnimation(EnvData.SkeletonDataID, SortingLayers.GATHERING_RESOURCES, "EnvPrefab");
        Flip(_flipValue> 0.3f);
        EnvState = EEnvState.Idle;

        #endregion

        _hurtFlash.Init();
    }
    
    public override void OnDamage(InteractionObject Attacker, float damage)
    {
        base.OnDamage(Attacker, damage);

        float dmg = 1;
        EnvState = EEnvState.OnDamaged;
        
        Managers.Object.ShowDamageFont(CenterPosition, dmg, transform, EDamageResult.Hit);

        Hp = Mathf.Clamp(Hp - dmg, 0, MaxHp);
        if (Hp == 0)
        {
            EnvState = EEnvState.Dead;
        }
    }

    IEnumerator CoReserveSpawn()
    {
        yield return new WaitForSeconds(EnvData.RegenTime);

        while (Managers.Map.CanGo(null, CellPos) == false)
        {
            yield return new WaitForSeconds(EnvData.RegenTime);
        }
        SetInfo(TemplateId);

    }
}
                    