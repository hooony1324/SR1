using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Define;

public class StageTransition : InitBase
{
    public List<Stage> Stages = new List<Stage>();
    public DungeonStage Dungeon;
    public Stage CurrentStage { get; set; }
    public Stage TownStage { get; set; }
    public int CurrentStageIndex
    {
        get { return Managers.Game.SaveData.CurrentStageIndex;}
        private set { Managers.Game.SaveData.CurrentStageIndex = value; }
    }
    private Hero _leader;
    public SpriteRenderer MapBound;

    public void SetInfo()
    {
        for (int i = 0; i < Stages.Count; i++)
        {
            Stages[i].SetInfo(i);
            if (Stages[i].IsTownStage)
                TownStage = Stages[i];
        }
        
        Dungeon.SetInfo(DUNGEON_IDNEX);
        Dungeon.IsDungeon = true;
        
        OnMapChanged(CurrentStageIndex);
    }

    public void CheckMapChanged(Vector3 position)
    {
        //던전인 경우 리턴
        if(CurrentStageIndex == DUNGEON_IDNEX)
            return;
        
        if (CurrentStage.IsPointInStage(position) == false)
        {
            int stageIndex = GetStageIndex((position));
            OnMapChanged(stageIndex);
        }
    }

    public int GetStageIndex(Vector3 position)
    {
        for (int i = 0; i < Stages.Count; i++)
        {
            if(Stages[i].IsPointInStage(position))
            {
                return i;
            }
            else if (Dungeon.IsPointInStage(position))
            {
                return i;
            }
        }

        Debug.LogError("Cannot Find CurrentMapZone");
        return -1;
    }

    public void OnMapChanged(Vector3 worldPos)
    {
        int mapIdx = GetStageIndex(worldPos);
        OnMapChanged(mapIdx);
    }
    
    public void OnMapChanged(int newMapIndex)
    {
        //TODO 하드코딩
        if (newMapIndex == DUNGEON_IDNEX)
        {
            CurrentStageIndex = newMapIndex;
        }
        else
        {
            CurrentStageIndex = newMapIndex;
            CurrentStage = Stages[CurrentStageIndex];
            LoadMapsAround(newMapIndex);
            UnloadOtherMaps(newMapIndex);
        }
    }

    private void LoadMapsAround(int mapIndex)
    {
        // 이전, 현재, 다음 맵을 로드
        for (int i = mapIndex - 1; i <= mapIndex + 1; i++)
        {
            if (i > -1 && i < Stages.Count) 
            {
                {
                    Debug.Log($"{i} Stage Load -> {Stages[i].name}");
                    Stages[i].LoadStage();
                }

            }
        }
    }

    private void UnloadOtherMaps(int mapIndex)
    {
        for (int i = 0; i < Stages.Count; i++)
        {
            if (i < mapIndex - 1 || i > mapIndex + 1)
            {
                Debug.Log($"{i} Stage UnLoad -> {Stages[i].name}");
                Stages[i].UnLoadStage();
            }
        }
    }
    
    #if UNITY_EDITOR
    public void DrawCollision()
    {
        Tilemap tilemap_collision =  Util.FindChild<Tilemap>(gameObject, "Tilemap_Collision", true);
        TileBase terrainO =
            AssetDatabase.LoadAssetAtPath<TileBase>("Assets/@Resources/TileMaps/01_asset/dev/Collider/terrain_O.asset");
        TileBase terrainX =
            AssetDatabase.LoadAssetAtPath<TileBase>("Assets/@Resources/TileMaps/01_asset/dev/Collider/terrain_X.asset");

        if (tilemap_collision.cellBounds.size.x == 0 && tilemap_collision.cellBounds.size.y == 0)
        {
            tilemap_collision.cellBounds.SetMinMax(new Vector3Int(-200, -200),new Vector3Int(500, 500));
        }

        for (int y = tilemap_collision.cellBounds.yMax; y >= tilemap_collision.cellBounds.yMin; y--)
        {
            for (int x = tilemap_collision.cellBounds.xMin; x <= tilemap_collision.cellBounds.xMax; x++)
            {
                tilemap_collision.SetTile(new Vector3Int(x,y),null);
            }
        }

        for (int y = tilemap_collision.cellBounds.yMax; y >= tilemap_collision.cellBounds.yMin; y--)
        {
            for (int x = tilemap_collision.cellBounds.xMin; x <= tilemap_collision.cellBounds.xMax; x++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);
                Vector3 worldPos = tilemap_collision.CellToWorld(cellPos);
                
                foreach (var map in Stages)
                {
                     {
                         Tilemap terrain =  Util.FindChild<Tilemap>(map.gameObject, "Terrain_01", true);
                         Vector3Int pos = terrain.WorldToCell(worldPos);
                         TileBase tile = terrain.GetTile(pos);
                         if (tile != null)
                         {
                             tilemap_collision.SetTile(cellPos, terrainO);
                         }
                     }
                    
                    //1. Wall 확인
                    {
                        Tilemap wall =  Util.FindChild<Tilemap>(map.gameObject, "Wall_01", true);
                        Vector3Int pos = wall.WorldToCell(worldPos);
                        TileBase tile = wall.GetTile(pos);
                        if (tile != null)
                        {
                            //장애물이 있는 지역
                            tilemap_collision.SetTile(cellPos, null);
                            continue;
                        }
                        
                        Tilemap wall_02 =  Util.FindChild<Tilemap>(map.gameObject, "Wall_02", true);
                        if (wall_02 != null)
                        {
                            Vector3Int pos2 = wall_02.WorldToCell(worldPos);
                            TileBase wall2 = wall_02.GetTile(pos);
                            if (wall2 != null)
                            {
                                //장애물이 있는 지역
                                tilemap_collision.SetTile(cellPos, null);
                                continue;
                            }
                        }
                    }
                    //
                    //2. Objec 확인 -> 빨간색
                    // {
                    //     Tilemap wall =  Util.FindChild<Tilemap>(map.gameObject, "Objects_01", true);
                    //     Vector3Int pos = wall.WorldToCell(worldPos);
                    //     TileBase tile = wall.GetTile(pos);
                    //     if (tile != null)
                    //     {
                    //         //장애물이 있는 지역
                    //         tilemap_collision.SetTile(cellPos, terrainX);
                    //         continue;
                    //     }
                    // }
                }

            }
        }
    }
#endif
}
