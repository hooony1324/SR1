using System;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Rendering;
using Event = Spine.Event;

public class BaseObject : InitBase
{
    public Define.EObjectType ObjectType { get; set; }
    public SkeletonAnimation SkeletonAnim { get; set; }
    public Vector3 Position => transform.position;
    bool _lookLeft = true;
    public bool LookLeft
    {
        get { return _lookLeft; }
        set
        {
            _lookLeft = value;
            Flip(!value);
        }
    }
    public int ExtraCells = 0;

    protected virtual void OnDisable()
    {
        if (!SkeletonAnim) 
            return;
        if (SkeletonAnim.AnimationState == null) 
            return;
        SkeletonAnim.AnimationState.Event -= OnAnimEventHandler;
        SkeletonAnim.AnimationState.Complete -= OnAnimCompleteHandler;
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        return true;
    }

    public void LookAtTarget(BaseObject target)
    {
        if(target == null)
            return;
        Vector2 dir = target.transform.position - transform.position;
        if (dir.x < 0)
            LookLeft = true;
        else if(dir.x > 0)
            LookLeft = false;
    }

    public static Vector3 GetLookAtRotation(Vector3 dir)
    {
        // Mathf.Atan2를 사용해 각도를 계산하고, 라디안에서 도로 변환
        float angle = Mathf.Atan2(-dir.x, dir.y) * Mathf.Rad2Deg;
        
        // Z축을 기준으로 회전하는 Vector3 값을 리턴
        return new Vector3(0, 0, angle);
    }

    #region Spine Functions

    protected void SetSpineAnimation(string spineName, int sortingOrder, string objName)
    {
        SkeletonAnim = GetComponent<SkeletonAnimation>();
        if (SkeletonAnim == null)
            SkeletonAnim = Util.FindChild<SkeletonAnimation>(gameObject, objName);
        
        if (String.IsNullOrEmpty(spineName) == false)
        {
            SkeletonAnim.skeletonDataAsset = Managers.Resource.Load<SkeletonDataAsset>(spineName);
        }
        else
        {
        }
        SkeletonAnim.clearStateOnDisable = true;

        SkeletonAnim.Initialize(true);
        ResigterAnimEvent();

        SortingGroup sg = Util.GetOrAddComponent<SortingGroup>(SkeletonAnim.gameObject);
        sg.sortingOrder = sortingOrder;
    }

    protected void ResigterAnimEvent()
    {
        if (SkeletonAnim.AnimationState != null)
        {
            SkeletonAnim.AnimationState.Event += OnAnimEventHandler;
            SkeletonAnim.AnimationState.Complete += OnAnimCompleteHandler;
        }
    }
    
    public TrackEntry PlayAnimation(int trackIndex, string animName, bool loop, bool isMix = true)
    {
        if(SkeletonAnim == null) return null;
        if(SkeletonAnim.AnimationState == null) return null;
        TrackEntry entry = SkeletonAnim.AnimationState.SetAnimation(trackIndex, animName, loop);

        if (isMix == false)
            entry.MixDuration = 0;
        
        if(animName == AnimName.DEAD || animName.Contains("skill_a") || animName.Contains("skill_b"))
            entry.MixDuration = 0;
        else
        {
            entry.MixDuration = 0.2f;
        }

        return entry;
    }

    public TrackEntry GetCurrentAnimation()
    {
        var trackEntry = SkeletonAnim.state.GetCurrent(0);
        return trackEntry;
    }

    public TrackEntry AddAnimation(int trackIndex, string AnimName, bool loop, float delay)
    {
        return SkeletonAnim.AnimationState.AddAnimation(trackIndex, AnimName, loop, delay);
    }

    public float GetSpineHeight()
    {
        float x, y, width, height;
        float[] vertexBuffer = null;
        SkeletonAnim.skeleton.GetBounds(out x, out y, out width, out height, ref vertexBuffer);
        return height;
    }

    protected virtual void OnAnimEventHandler(TrackEntry trackEntry, Event e) { }
    
    protected virtual void OnAnimCompleteHandler(TrackEntry arg1) { }

    
    public void Flip(bool flag)
    {
        if (SkeletonAnim == null)
            return;

        SkeletonAnim.Skeleton.ScaleX = flag ? -1 : 1;
    }

    #endregion
    
    #region Map
    public bool LerpCellPosCompleted { get; protected set; }

    
    [SerializeField]Vector3Int _cellPos;
    public Vector3Int CellPos
    {
        get { return _cellPos; }
        protected set
        {
            _cellPos = value;
            LerpCellPosCompleted = false;
        }
    }

    public void SetCellPos(Vector3Int cellPos, bool forceMove = false)
    {
        CellPos = cellPos;
        LerpCellPosCompleted = false;

        if (forceMove)
        {
            transform.position = Managers.Map.Cell2World(CellPos);
            LerpCellPosCompleted = true;
        }
    }

    public void LerpToCellPos(float moveSpeed , bool canFlip = true)
    {
        if (LerpCellPosCompleted)
        {
            return;
        }

        Vector3 destPos = Managers.Map.Cell2World(CellPos);
        Vector3 dir = destPos - transform.position;
        if (canFlip)
        {
            if (dir.x < 0)
                LookLeft = true;
            else if (dir.x > 0)
            {
                LookLeft = false;
            }
        }

        // if (dir.magnitude < 0.01f)
        if(destPos == transform.position)
        {
            // transform.position = destPos;
            LerpCellPosCompleted = true;
            return;
        }

        float moveDist = Mathf.Min(dir.magnitude, moveSpeed * Time.deltaTime);
        // transform.position = Vector3.MoveTowards(transform.position, destPos, moveDist);
        transform.position += dir.normalized * moveDist;
    }
    #endregion
}

