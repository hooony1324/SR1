using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Spine;
using Spine.Unity;
using UnityEngine;

public class InteractionObject : BaseObject
{
    public Vector3 CenterPosition => transform.position + Vector3.up * CurrentCollider.radius;
    public CircleCollider2D CurrentCollider;
    public Stage SpawnStage { get; set; }
    public ObjectSpawnInfo SpawnInfo { get; set; }

    public event Action<InteractionObject> EventOnDead;
    
    protected HurtFlashEffect _hurtFlash;
    
    public int TemplateId { get; set; }
    public EffectComponent Effects { get; set; }

    public Vector3 FireSocketPos
    {
        get
        {
            if (_pointAttachment == null)
            {
                return CenterPosition;
            }

            Slot slot = SkeletonAnim.Skeleton.FindSlot("fire_socket");
            Vector3 ret = _pointAttachment.GetWorldPosition(slot, SkeletonAnim.transform);
            return ret;
        }
    }

    protected PointAttachment _pointAttachment;

    
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        CurrentCollider = gameObject.GetOrAddComponent<CircleCollider2D>();
        CurrentCollider.isTrigger = true;
        _hurtFlash = gameObject.GetOrAddComponent<HurtFlashEffect>();
        // Rigid = gameObject.GetOrAddComponent<Rigidbody2D>();
        // Rigid.simulated = false;
        //Effects       
        GameObject effectObj = new GameObject();
        Effects = effectObj.AddComponent<EffectComponent>();
        effectObj.name = "Effects";
        effectObj.transform.parent = gameObject.transform;
        Effects.SetInfo(this);
        return true;
    }

    protected void SetFireSocket()
    {
        if (SkeletonAnim.Skeleton.FindSlot("fire_socket") == null)
        {
            return;
        }
        //FireSocket
        Attachment attachment = SkeletonAnim.Skeleton.GetAttachment("fire_socket", "fire_socket");

        if (attachment == null)
        {
            Debug.Log("attachment not found");
            return;
        }

        _pointAttachment = attachment as PointAttachment;
    }
    
    public virtual void OnDamage(InteractionObject Attacker, float damage)
    {
        _hurtFlash.Flash();
    }

    public void BroadcastOnDead()
    {
        EventOnDead?.Invoke(this);
    }
    
    protected void SetCenterPosition()
    {
        // if (SkeletonAnim == null) 
        //     return;
        //
        // Bone bone = SkeletonAnim.Skeleton.FindBone("center");
        // if(bone != null)
            // CenterPosition = bone.GetWorldPosition(SkeletonAnim.transform);
    }


    #region DropItem
    public void DropItem(int dropItemId)
    {
        StartCoroutine(CoDropItem(dropItemId));
    }

    IEnumerator CoDropItem(int dropItemId)
    {
        List<RewardData> rewards = GetRewards(dropItemId);
        if (rewards != null)
        {
            foreach (var reward in rewards)
            {
                SpawnItemHolder(dropItemId, reward);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    private void SpawnItemHolder(int dropItemId, RewardData rewardData)
    {
        var itemHolder = Managers.Object.Spawn<ItemHolder>(CenterPosition, dropItemId);
        Vector2 ran = new Vector2(CenterPosition.x +  UnityEngine.Random.Range(-1.5f, -1.0f), CenterPosition.y);
        Vector2 ran2 = new Vector2(CenterPosition.x +  UnityEngine.Random.Range(1.0f, 1.5f), CenterPosition.y);
        Vector2 dropPos =  UnityEngine.Random.value < 0.5 ? ran : ran2;
        itemHolder.SetInfo( rewardData,CenterPosition, dropPos);
    }

    List<RewardData> GetRewards(int dropItemId)
    {
        if (Managers.Data.DropTableDic.TryGetValue(dropItemId, out DropTableData dropTableData) == false)
            return null;

        if (dropTableData.Rewards.Count <= 0)
            return null;

        List<RewardData> rewardDatas = new List<RewardData>();

        int sum = 0;
        int randValue = UnityEngine.Random.Range(0, 100);

        foreach (RewardData item in dropTableData.Rewards)
        {
            if (item.Probability == 100)
            {
                //확정드롭아이템
                rewardDatas.Add(item);
                continue;
            }

            //확정드롭아이템을 제외한 아이템
            sum += item.Probability;
            if (randValue <= sum)
            {
                rewardDatas.Add(item);
                break;
            }

        }

        return rewardDatas;
    }
    #endregion

}
