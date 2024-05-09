using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

#if UNITY_EDITOR
using Newtonsoft.Json;
using UnityEditor;
#endif

public class MapEditor : MonoBehaviour
{

#if UNITY_EDITOR

    [MenuItem("Tools/GenerateMap %#m")]
    private static void GenerateMap()
    {
        GameObject[] gameObjects = Selection.gameObjects;

        foreach (GameObject go in gameObjects)
        {
            Tilemap tm = Util.FindChild<Tilemap>(go, "Tilemap_Collision", true);

            using (var writer = File.CreateText($"Assets/@Resources/Data/MapData/{go.name}Collision.txt"))
            {
                writer.WriteLine(tm.cellBounds.xMin);
                writer.WriteLine(tm.cellBounds.xMax);
                writer.WriteLine(tm.cellBounds.yMin);
                writer.WriteLine(tm.cellBounds.yMax);

                for (int y = tm.cellBounds.yMax; y >= tm.cellBounds.yMin; y--)
                {
                    for (int x = tm.cellBounds.xMin; x <= tm.cellBounds.xMax; x++)
                    {
                        TileBase tile = tm.GetTile(new Vector3Int(x, y, 0));
                        if (tile != null)
                        {
                            if (tile.name.Contains("O"))
                                writer.Write(Define.MAP_TOOL_NONE);
                            else
                                writer.Write(Define.MAP_TOOL_SEMI_WALL);
                        }							
                        else
                            writer.Write(Define.MAP_TOOL_WALL);
                    }
                    writer.WriteLine();
                }
            }
        }

        Debug.Log("Map Collision Generation Complete");
    }
    
    [MenuItem("Tools/Create Object Tile Asset")]
    public static void CreateObjectTile()
    {
        #region Monster
        Dictionary<int, Data.CreatureData> CreatureDic = LoadJson<Data.CreatureDataLoader, int, Data.CreatureData>("MonsterData").MakeDict();
        foreach (var data in CreatureDic.Values)
        {
            if(data.TemplateId < 202000) 
                continue;
    
            string name = $"{data.TemplateId}_{data.DescriptionTextID}";
            string path = Path.Combine("Assets/@Resources/TileMaps/01_asset/dev/Monster", $"{name}.Asset");

            if (path == "")
                continue;

            CustomTile existingTile = AssetDatabase.LoadAssetAtPath<CustomTile>(path);
            if (existingTile != null)
            {
                existingTile.Name = name;
        
                string spriteName = data.IconImage.Replace(".sprite", "");
                Sprite spr = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/@Resources/Sprites/UI/Icon/Monsters/{spriteName}.png");
                existingTile.sprite = spr;
                existingTile.TemplateId = data.TemplateId;
                existingTile.ObjectType = Define.EObjectType.Monster;

                EditorUtility.SetDirty(existingTile);
            }
            else
            {
                CustomTile customTile = ScriptableObject.CreateInstance<CustomTile>();
                customTile.Name = data.DescriptionTextID;
                string spriteName = data.IconImage.Replace(".sprite", "");
                Sprite spr = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/@Resources/Sprites/Monsters/{spriteName}.png");
                customTile.sprite = spr;
                customTile.TemplateId = data.TemplateId;
                customTile.ObjectType = Define.EObjectType.Monster;

                AssetDatabase.CreateAsset(customTile, path);
            }
        }
        #endregion

        #region Env
        Dictionary<int, Data.EnvData> Env = LoadJson<Data.EnvDataLoader, int, Data.EnvData>("EnvData").MakeDict();
        
        foreach (var data in Env.Values)
        {
            string name = $"{data.DataId}_{data.DescriptionTextID}";
            string path = "Assets/@Resources/TileMaps/01_asset/dev/Env";
            path = Path.Combine(path, $"{name}.Asset");

            if (path == "")
                continue;

            CustomTile existingTile = AssetDatabase.LoadAssetAtPath<CustomTile>(path);
            if (existingTile != null)
            {
                existingTile.Name = name;
                existingTile.TemplateId = data.DataId;
                existingTile.ObjectType = Define.EObjectType.Env;
                string spriteName = data.SpriteName.Replace(".sprite", "");
                Sprite spr = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/@Resources/Sprites/Env/{spriteName}.png");                EditorUtility.SetDirty(existingTile);
                existingTile.sprite = spr;
            }
            else
            {
                CustomTile customTile = ScriptableObject.CreateInstance<CustomTile>();
                customTile.Name = data.DescriptionTextID;
                customTile.TemplateId = data.DataId;
                customTile.ObjectType = Define.EObjectType.Env;
                string spriteName = data.SpriteName.Replace(".sprite", "");
                Sprite spr = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/@Resources/Sprites/Env/{spriteName}.png");
                AssetDatabase.CreateAsset(customTile, path);
                customTile.sprite = spr;
            }
            AssetDatabase.SaveAssets();
        }
        #endregion
        
        #region Npc
        Dictionary<int, Data.NpcData> Npc = LoadJson<Data.NpcDataLoader, int, Data.NpcData>("NpcData").MakeDict();
        foreach (var data in Npc.Values)
        {
            string name = $"{data.DataId}_{data.Name}";
            string path = "Assets/@Resources/TileMaps/01_asset/dev/Npc";
            path = Path.Combine(path, $"{name}.Asset");
                
            if (path == "")
                continue;

            CustomTile existingTile = AssetDatabase.LoadAssetAtPath<CustomTile>(path);
            if (existingTile != null)
            {
                existingTile.Name = name;
                existingTile.Name = data.Name;
                existingTile.TemplateId = data.DataId;
                existingTile.ObjectType = Define.EObjectType.Npc;
                existingTile.NpcType = data.NpcType;
                existingTile.QuestDataId = data.QuestDataId;
                existingTile.QuestTaskId = data.QuestTaskDataId;

                string spriteName = data.IconSpriteName;
                spriteName = spriteName.Replace(".sprite", "");
                Sprite spr = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/@Resources/Sprites/NPC/{spriteName}.png");
                existingTile.sprite = spr;
                EditorUtility.SetDirty(existingTile);
            }
            else
            {
                CustomTile customTile = ScriptableObject.CreateInstance<CustomTile>();
                customTile.Name = data.Name;
                customTile.TemplateId = data.DataId;
                customTile.ObjectType = Define.EObjectType.Npc;
                customTile.NpcType = data.NpcType;
                customTile.QuestDataId = data.QuestDataId;
                customTile.QuestTaskId = data.QuestTaskDataId;
                string spriteName = data.IconSpriteName;
                spriteName = spriteName.Replace(".sprite", "");
                Sprite spr = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/@Resources/Sprites/NPC/{spriteName}.png");
                customTile.sprite = spr;

                AssetDatabase.CreateAsset(customTile, path);
            }
        }
        #endregion
    }

    private static Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>($"Assets/@Resources/Data/JsonData/{path}.json");
        return JsonConvert.DeserializeObject<Loader>(textAsset.text);
    }

#endif

}
