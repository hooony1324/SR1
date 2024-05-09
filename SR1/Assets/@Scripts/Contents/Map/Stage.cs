using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Define;

public class Stage : MonoBehaviour
{
    [SerializeField] protected List<Monster> _monsters = new List<Monster>();
    public List<ObjectSpawnInfo> SpawnInfos = new List<ObjectSpawnInfo>();
    protected ObjectSpawnInfo _startSpawnInfo;

    public ObjectSpawnInfo StartSpawnInfo
    {
        get => _startSpawnInfo;
        set => _startSpawnInfo = value;
    }

    public List<ObjectSpawnInfo> WaypointSpawnInfos = new List<ObjectSpawnInfo>();

    public int StageIndex { get; set; }

    public Tilemap TilemapTerrain;
    public Tilemap TilemapObject; // 하이어라키에서 추가

    private Grid _grid;

    private Npc _townPortal;

    //하이어라키에서 체크
    public bool IsDungeon = false;

    public bool IsTownStage
    {
        get
        {
            if (_townPortal == null)
                return false;
            return true;
        }
    }

    public virtual void SetInfo(int stageIdx)
    {
        StageIndex = stageIdx;
        if (TilemapObject == null)
            Debug.LogError("TilemapObject must be assigned in the inspector.", this);

        TilemapTerrain = Util.FindChild<Tilemap>(gameObject, "InfoBox", true);
        SaveSpawnInfos();

        //오브젝트 스폰
        SpawnObjects();
    }

    public bool IsPointInStage(Vector3 position)
    {
        Vector3Int pos = TilemapTerrain.layoutGrid.WorldToCell(position);
        TileBase tile = TilemapTerrain.GetTile(pos);

        if (tile == null)
            return false;
        return true;
    }

    public void LoadStage()
    {
        gameObject.SetActive(true);
        // SpawnObjects();
    }

    public void UnLoadStage()
    {
        gameObject.SetActive(false);
        // DespawnObjects();
    }

    IEnumerator CoReserveSpawn(ObjectSpawnInfo spawnInfo)
    {
        yield return new WaitForSeconds(MONSTER_RESPONE_TIME);
        
        switch (spawnInfo.ObjectType)
        {
            case EObjectType.Monster:
                // Vector3Int nearbyPoint = Managers.Game.GetNearbyPosition(null, spawnInfo.CellPos);
                while (Managers.Map.CanGo(null, spawnInfo.CellPos) == false)
                {
                    yield return new WaitForSeconds(MONSTER_RESPONE_TIME);
                }
                Monster monster = Managers.Object.Spawn<Monster>(spawnInfo.CellPos, spawnInfo.TemplateId);
                monster.SpawnStage = this;
                monster.SpawnInfo = spawnInfo;
                monster.transform.position = monster.SpawnInfo.WorldPos;
                Managers.Map.MoveTo(monster, spawnInfo.CellPos);
                break;
        }
    }
    
    private void SpawnObjects()
    {
        foreach (var info in SpawnInfos)
        {
            Vector3 worldPos = info.WorldPos;
            Vector3Int cellPos = info.CellPos;

            switch (info.ObjectType)
            {
                case EObjectType.Monster:
                    Monster monster = Managers.Object.Spawn<Monster>(worldPos, info.TemplateId);
                    monster.SpawnStage = this;
                    monster.SpawnInfo = info;
                    _monsters.Add(monster);
                    monster.EventOnDead -= HandleOnDead;
                    monster.EventOnDead += HandleOnDead;
                    Managers.Map.MoveTo(monster, cellPos);
                    break;
                case EObjectType.Env:
                    Env env = Managers.Object.Spawn<Env>(worldPos, info.TemplateId);
                    env.SpawnStage = this;
                    env.SpawnInfo = info;

                    env.EventOnDead -= HandleOnDead;
                    env.EventOnDead += HandleOnDead;
                    Managers.Map.MoveTo(env, cellPos);
                    break;
                case EObjectType.Npc:
                    if (info.NpcType == ENpcType.Portal)
                    {
                        //포탈이 이미 생성되어있는경우 스킵
                        if (_townPortal)
                            continue;
                        _townPortal = Managers.Object.Spawn<Npc>(worldPos, info.TemplateId);
                        _townPortal.SetInfo(info.TemplateId);
                        _townPortal.SpawnStage = this;
                        PortalInteraction pi = _townPortal.Interaction as PortalInteraction;
                        pi.IsTownPortal = true;
                        Managers.Game.TownPortal = _townPortal;
                        Managers.Map.MoveTo(_townPortal, cellPos);
                    }
                    else
                    {
                        Npc npc = Managers.Object.Spawn<Npc>(worldPos, info.TemplateId);
                        Managers.Map.MoveTo(npc, cellPos);
                        npc.SpawnStage = this;
                    }

                    break;
            }
        }
    }

    private void SaveSpawnInfos()
    {
        if (TilemapObject != null)
            TilemapObject.gameObject.SetActive(false);

        for (int y = TilemapObject.cellBounds.yMax; y >= TilemapObject.cellBounds.yMin; y--)
        {
            for (int x = TilemapObject.cellBounds.xMin; x <= TilemapObject.cellBounds.xMax; x++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);
                CustomTile tile = TilemapObject.GetTile(new Vector3Int(x, y, 0)) as CustomTile;

                if (tile == null)
                    continue;

                Vector3 worldPos = Managers.Map.Cell2World(cellPos);
                ObjectSpawnInfo info = new ObjectSpawnInfo(tile.Name, tile.TemplateId, x, y, worldPos, tile.ObjectType,
                    tile.NpcType);

                switch (tile.NpcType)
                {
                    case ENpcType.StartPosition:
                        StartSpawnInfo = info;
                        continue;
                    case ENpcType.Waypoint:
                        WaypointSpawnInfos.Add(info);
                        break;
                    case ENpcType.Quest:
                        break;
                }

                SpawnInfos.Add(info);
            }
        }
    }

    protected virtual void HandleOnDead(InteractionObject obj)
    {                
        InteractionObject cached = obj;

        switch (obj.ObjectType)
        {
            case EObjectType.Monster:
                Coroutine coroutine = StartCoroutine(CoReserveSpawn(cached.SpawnInfo));
                break;
        }
    }
}