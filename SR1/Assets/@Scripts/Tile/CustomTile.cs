using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
#endif

public class CustomTile : Tile
{
    [Space] [Space] [Header("For Zombie")] 
    public Define.EObjectType ObjectType;

    public Define.ENpcType NpcType;
    
    public int TemplateId;    
    public int QuestDataId;
    public int QuestTaskId;
    public string Name;

}

