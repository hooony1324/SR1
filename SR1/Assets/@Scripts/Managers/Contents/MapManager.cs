using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static Define;

public struct ObjectSpawnInfo
{
    public ObjectSpawnInfo(string name, int templateId, int x, int y, Vector3 worldPos, EObjectType type, ENpcType npcType)
    {
        Name = name;
        TemplateId = templateId;
        Vector3Int pos = new Vector3Int(x, y, 0);
        CellPos = pos;
        WorldPos = worldPos;
        ObjectType = type;
        NpcType = npcType;
    }

    public string Name;
    public int TemplateId;
    public Vector3Int CellPos;
    public Vector3 WorldPos;
    public EObjectType ObjectType;
    public ENpcType NpcType;
}

class Cell
{
    // public HashSet<InteractionObject> Objects { get; } = new HashSet<InteractionObject>();
    public HashSet<InteractionObject> Monsters { get; } = new HashSet<InteractionObject>();
    public HashSet<InteractionObject> Heroes { get; } = new HashSet<InteractionObject>();
    public HashSet<InteractionObject> GatheringResources { get; } = new HashSet<InteractionObject>();
}

public class MapManager
{
    public GameObject Map { get; private set; }
    public string MapName { get; private set; }
    public Grid CellGrid { get; private set; }
    public Dictionary<Vector3Int, BaseObject> _cells = new Dictionary<Vector3Int, BaseObject>();
    public StageTransition StageTransition;
    
    private int _minX;
    private int _maxX;
    private int _minY;
    private int _maxY;

    public Vector3Int World2Cell(Vector3 worldPos)
    {
        return CellGrid.WorldToCell(worldPos);
    }

    public Vector3 Cell2World(Vector3Int cellPos)
    {
        return CellGrid.CellToWorld(cellPos);
    }

    private ECellCollisionType[,] _collision;

    public void LoadMap(string mapName)
    {
        DestroyMap();

        GameObject map = Managers.Resource.Instantiate(mapName);
        StageTransition = map.GetComponent<StageTransition>();
        map.transform.position = Vector3.zero;
        map.name = $"@Map_{mapName}";

        Map = map;
        MapName = mapName;
        CellGrid = map.GetComponent<Grid>();

        ParseCollisionData(map, mapName);
    }

    public void DestroyMap()
    {
        ClearObjects();

        if (Map != null)
            Managers.Resource.Destroy(Map);
    }

    public bool CanGo(BaseObject self, Vector3 worldPos, bool ignoreObjects = false, bool ignoreSemiWall = false)
    {
        return CanGo(self, World2Cell(worldPos), ignoreObjects, ignoreSemiWall);
    }

    public bool CanGo(BaseObject self, Vector3Int cellPos, bool ignoreObjects = false, bool ignoreSemiWall = false)
    {
        int extraCells = 0;
        if (self != null)
            extraCells = self.ExtraCells;

        for (int dx = -extraCells; dx <= extraCells; dx++)
        {
            for (int dy = -extraCells; dy <= extraCells; dy++)
            {
                Vector3Int checkPos = new Vector3Int(cellPos.x + dx, cellPos.y + dy);

                if (CanGo_Internal(self, checkPos, ignoreObjects, ignoreSemiWall) == false)
                    return false;
            }
        }

        return true;
    }

    bool CanGo_Internal(BaseObject self, Vector3Int cellPos, bool ignoreObjects = false, bool ignoreSemiWall = false)
    {
        if (cellPos.x < _minX || cellPos.x > _maxX)
            return false;
        if (cellPos.y < _minY || cellPos.y > _maxY)
            return false;

        if (ignoreObjects == false)
        {
            BaseObject obj = GetObject(cellPos);
            if (obj != null && obj != self)
                return false;

            /* 이 코드 기반으로 고민 시작하면 될듯.
             * 1. 캐릭터가 이동시 너무 겹침
             * 2. 조이스틱을 놓았을때 그 자리 그대로 멈추기 때문에 조이스틱을 놓았을때 다시 한번 자리를 찾아주는 행위가 필요
            if (obj != null && obj != self)
            {
                if (Managers.Game.JoystickState == EJoystickState.PointerUp || self is not Hero || obj is not Hero)
                    return false;
            }*/
        }

        int x = cellPos.x - _minX;
        int y = _maxY - cellPos.y;
        ECellCollisionType type = _collision[x, y];
        if (type == ECellCollisionType.None)
            return true;

        if (ignoreSemiWall && type == ECellCollisionType.SemiWall)
            return true;

        return false;
    }

    void ParseCollisionData(GameObject map, string mapName, string tilemap = "Tilemap_Collision")
    {
        GameObject collision = Util.FindChild(map, tilemap, true);
        if (collision != null)
            collision.SetActive(false);

        // Collision 관련 파일
        TextAsset txt = Managers.Resource.Load<TextAsset>($"{mapName}Collision");
        StringReader reader = new StringReader(txt.text);

        _minX = int.Parse(reader.ReadLine());
        _maxX = int.Parse(reader.ReadLine());
        _minY = int.Parse(reader.ReadLine());
        _maxY = int.Parse(reader.ReadLine());

        int xCount = _maxX - _minX + 1;
        int yCount = _maxY - _minY + 1;
        _collision = new ECellCollisionType[xCount, yCount];

        for (int y = 0; y < yCount; y++)
        {
            string line = reader.ReadLine();
            for (int x = 0; x < xCount; x++)
            {
                switch (line[x])
                {
                    case MAP_TOOL_WALL:
                        _collision[x, y] = ECellCollisionType.Wall;
                        break;
                    case MAP_TOOL_NONE:
                        _collision[x, y] = ECellCollisionType.None;
                        break;
                    case MAP_TOOL_SEMI_WALL:
                        _collision[x, y] = ECellCollisionType.SemiWall;
                        break;
                }
            }
        }
    }

    public bool MoveTo(InteractionObject obj, Vector3Int cellPos, bool forceMove = false)
    {
        if (CanGo(obj, cellPos) == false)
            return false;
        Vector3 worldPos = Cell2World(cellPos);
      
        // 기존 좌표에 있던 오브젝트를 밀어준다.
        // (단, 처음 신청했으면 해당 CellPos의 오브젝트가 본인이 아닐 수도 있음)
        RemoveObject(obj);

        // 새 좌표에 오브젝트를 등록한다.
        AddObject(obj, cellPos);

        // 셀 좌표 이동
        obj.SetCellPos(cellPos, forceMove);


        //TODO 리더인 경우 맵 확인
        if (obj == Managers.Game.Leader)
            StageTransition.CheckMapChanged(worldPos);

        return true;
    }

    private void CheckUserPosition()
    {
        // OnLeaderPosChanged?.Invoke(cellPos);
    }

    #region Helpers
    
    public BaseObject GetObject(Vector3Int cellPos)
    {
        // 없으면 null
        _cells.TryGetValue(cellPos, out BaseObject value);
        return value;
    }

    public BaseObject GetObject(Vector3 worldPos)
    {
        Vector3Int cellPos = World2Cell(worldPos);
        return GetObject(cellPos);
    }

    public void RemoveObject(BaseObject obj)
    {
        // 기존의 좌표 제거
        int extraCells = 0;
        if (obj != null)
            extraCells = obj.ExtraCells;

        Vector3Int cellPos = obj.CellPos;

        for (int dx = -extraCells; dx <= extraCells; dx++)
        {
            for (int dy = -extraCells; dy <= extraCells; dy++)
            {
                Vector3Int newCellPos = new Vector3Int(cellPos.x + dx, cellPos.y + dy);
                BaseObject prev = GetObject(newCellPos);

                if (prev == obj)
                    _cells[newCellPos] = null;
            }
        }
    }

    void AddObject(BaseObject obj, Vector3Int cellPos)
    {
        int extraCells = 0;
        if (obj != null)
            extraCells = obj.ExtraCells;

        for (int dx = -extraCells; dx <= extraCells; dx++)
        {
            for (int dy = -extraCells; dy <= extraCells; dy++)
            {
                Vector3Int newCellPos = new Vector3Int(cellPos.x + dx, cellPos.y + dy);

                BaseObject prev = GetObject(newCellPos);
                if (prev != null && prev != obj)
                    Debug.LogWarning($"AddObject 수상함");

                _cells[newCellPos] = obj;
            }
        }
    }

    public void ClearObjects()
    {
        _cells.Clear();
    }

    #endregion

    public List<T> GatherScreenObjects<T>() where T : BaseObject
    {
        HashSet<T> objects = new HashSet<T>();

        Vector3Int campPos = Managers.Map.World2Cell(Managers.Object.HeroCamp.Position);

        int basicDetectionRange;
        int extraVerticalDetectionRange;


        //To Dragon.
        //캠프시 true, 평소에 false입니다.
        //이 설정에서는 카메라 사이즈 30으로 하면 될듯합니다.
        bool onIdle = Managers.Object.HeroCamp.CampState == ECampState.CampMode;

        if(onIdle)
        {
            basicDetectionRange = BASIC_DETECTION_RANGE_ON_IDLE;
            extraVerticalDetectionRange = EXTRA_VERTICAL_DETECTION_RANGE_ON_IDLE;
        }
        else
        {
            basicDetectionRange = BASIC_DETECTION_RANGE;
            extraVerticalDetectionRange = EXTRA_VERTICAL_DETECTION_RANGE;
        }

        Vector3Int BottomLeft = new Vector3Int(campPos.x - basicDetectionRange - extraVerticalDetectionRange, campPos.y - extraVerticalDetectionRange);
        Vector3Int BottomRight = new Vector3Int(campPos.x - extraVerticalDetectionRange, campPos.y - basicDetectionRange - extraVerticalDetectionRange);
        Vector3Int TopRight = new Vector3Int(campPos.x + basicDetectionRange + extraVerticalDetectionRange, campPos.y + extraVerticalDetectionRange);
        Vector3Int TopLeft = new Vector3Int(campPos.x + extraVerticalDetectionRange, campPos.y + basicDetectionRange + extraVerticalDetectionRange);

        Vector3Int pos;

        bool drawing = true;
        while (drawing)
        {
            pos = BottomLeft;

            while (pos != BottomRight)
            {
                pos += new Vector3Int(1, -1);
             
                T obj = GetObject(pos) as T;
                if (obj != null)
                {
                    objects.Add(obj);
                }

                if(drawing)
                {
                    if (pos == campPos)
                        drawing = false;
                }    
            }
            if (drawing == false)
                break;

            while (pos != TopRight)
            {
                pos += new Vector3Int(1, 1);

                T obj = GetObject(pos) as T;
                if (obj != null)
                {
                    objects.Add(obj);
                }
                
                if (drawing)
                {
                    if (pos == campPos)
                        drawing = false;
                }
            }
            if (drawing == false)
                break;
            
            while (pos != TopLeft)
            {
                pos += new Vector3Int(-1, 1);

                T obj = GetObject(pos) as T;
                if (obj != null)
                {
                    objects.Add(obj);
                }
                
                if (drawing)
                {
                    if (pos == campPos)
                        drawing = false;
                }
            }
            
            if (drawing == false)
                break;
            
            while (pos != BottomLeft)
            {
                pos += new Vector3Int(-1, -1);
                
                T obj = GetObject(pos) as T;
                if (obj != null)
                {
                    objects.Add(obj);
                }

                if (drawing)
                {
                    if (pos == campPos)
                        drawing = false;
                }
            }
            BottomLeft += Vector3Int.right;
            BottomRight += Vector3Int.up;
            TopRight += Vector3Int.left;
            TopLeft += Vector3Int.down;
        }
        return objects.ToList();
    }

    
    public List<T> GatherObjects<T>(Vector3 pos, float rangeX, float rangeY) where T : BaseObject
    {
        // 비율이 1:2 니깐 2배 곱함.
        rangeX *= 2;
        HashSet<T> objects = new HashSet<T>();
    
        Vector3Int left = World2Cell(pos + new Vector3(-rangeX, 0));
        Vector3Int right = World2Cell(pos + new Vector3(+rangeX, 0));
        Vector3Int bottom = World2Cell(pos + new Vector3(0, -rangeY));
        Vector3Int top = World2Cell(pos + new Vector3(0, +rangeY));
        int minX = left.x;
        int maxX = right.x;
        int minY = bottom.y;
        int maxY = top.y;
    
        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                Vector3 worldPos = Managers.Map.Cell2World(tilePos);
                Vector3 centerWorldPos = pos;
                
                if ((worldPos - centerWorldPos).sqrMagnitude > SCAN_RANGE * SCAN_RANGE )
                    continue;
    
                // 타입에 맞는 리스트 리턴
                T obj = GetObject(tilePos) as T;
                if (obj == null)
                    continue;
    
                objects.Add(obj);
            }
        }
    
        return objects.ToList();
    }

    #region A* PathFinding

    public struct PQNode : IComparable<PQNode>
    {
        public int H; // Heuristic
        public Vector3Int CellPos;
        public int Depth;

        public int CompareTo(PQNode other)
        {
            if (H == other.H)
                return 0;
            return H < other.H ? 1 : -1;
        }
    }
    
    List<Vector3Int> _delta = new List<Vector3Int>()
    {
        new Vector3Int(0, 1, 0), // U
        new Vector3Int(1, 1, 0), // UR
        new Vector3Int(1, 0, 0), // R
        new Vector3Int(1, -1, 0), // DR
        new Vector3Int(0, -1, 0), // D
        new Vector3Int(-1, -1, 0), // LD
        new Vector3Int(-1, 0, 0), // L
        new Vector3Int(-1, 1, 0), // LU
    };

    public List<Vector3Int> FindPath(BaseObject self, Vector3Int startCellPos, Vector3Int destCellPos, int maxDepth = 10, int addCellSize = 0)
    {
        // 지금까지 제일 좋은 후보 기록.
        Dictionary<Vector3Int, int> best = new Dictionary<Vector3Int, int>();
        // 경로 추적 용도.
        Dictionary<Vector3Int, Vector3Int> parent = new Dictionary<Vector3Int, Vector3Int>();

        // 현재 발견된 후보 중에서 가장 좋은 후보를 빠르게 뽑아오기 위한 도구.
        PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>(); // OpenList

        Vector3Int pos = startCellPos;
        Vector3Int dest = destCellPos;

        // destCellPos에 도착 못하더라도 제일 가까운 애로.
        Vector3Int closestCellPos = startCellPos;
        int closestH = (dest - pos).sqrMagnitude;

        // 시작점 발견 (예약 진행)
        {
            int h = (dest - pos).sqrMagnitude;
            pq.Push(new PQNode() { H = h, CellPos = pos, Depth = 1 });
            parent[pos] = pos;
            best[pos] = h;
        }

        while (pq.Count > 0)
        {
            // 제일 좋은 후보를 찾는다
            PQNode node = pq.Pop();
            pos = node.CellPos;

            // 목적지 도착했으면 바로 종료.
            if (pos == dest)
                break;

            // 무한으로 깊이 들어가진 않음.
            if (node.Depth >= maxDepth)
                break;
            
            // 상하좌우 등 이동할 수 있는 좌표인지 확인해서 예약한다.
            foreach (Vector3Int delta in _delta)
            {
                //3*3 이상인 경우 addCellSize를 곱한다.
                Vector3Int next = pos + delta;

                // 갈 수 없는 장소면 스킵.
                if (CanGo(self,next) == false)
                    continue;

                // 예약 진행
                int h = (dest - next).sqrMagnitude;

                // 더 좋은 후보 찾았는지
                if (best.ContainsKey(next) == false)
                    best[next] = int.MaxValue;

                if (best[next] <= h)
                    continue;

                best[next] = h;

                pq.Push(new PQNode() { H = h, CellPos = next, Depth = node.Depth + 1 });
                parent[next] = pos;

                // 목적지까지는 못 가더라도, 그나마 제일 좋았던 후보 기억.
                if (closestH > h)
                {
                    closestH = h;
                    closestCellPos = next;
                }
            }
        }

        // 제일 가까운 애라도 찾음.
        if (parent.ContainsKey(dest) == false)
            return CalcCellPathFromParent(parent, closestCellPos);

        return CalcCellPathFromParent(parent, dest);
    }

    List<Vector3Int> CalcCellPathFromParent(Dictionary<Vector3Int, Vector3Int> parent, Vector3Int dest)
    {
        List<Vector3Int> cells = new List<Vector3Int>();

        if (parent.ContainsKey(dest) == false)
            return cells;

        Vector3Int now = dest;

        while (parent[now] != now)
        {
            cells.Add(now);
            now = parent[now];
        }

        cells.Add(now);
        cells.Reverse();

        return cells;
    }

    #endregion
}