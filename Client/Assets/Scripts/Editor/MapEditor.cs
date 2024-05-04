

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.IO;

public class MapEditor
{

#if UNITY_EDITOR
    // % Control # Shift , & Alt
    [MenuItem("Tools/Generate Map %#g")]
    private static void GenerateMap()
    {
        CreateByPath("Assets/Resources/Map");
        CreateByPath("../Common/MapData");

    }

    private static void CreateByPath(string pathPrefix)
    {
        if (EditorUtility.DisplayDialog("Generate Map", "Create?", "Create!", "Cancle!"))
        {
            string path = "Prefabs/Map";
            GameObject[] maps = Resources.LoadAll<GameObject>(path);

            if (maps.Length == 0)
            {
                Debug.LogError($"maps are Null : path {path}");
                return;
            }
            else
            {
                Debug.Log($"Maps are Loaded : {maps.Length} ");
            }

            foreach (var map in maps)
            {
                if (map == null)
                {
                    Debug.LogError("Map is Null");
                    continue;
                }


                List<Vector3Int> blocked = new List<Vector3Int>();
                Tilemap tmBase = Util.FindChild<Tilemap>(map, "Tilemap_Base", true);
                Tilemap tmEnv = Util.FindChild<Tilemap>(map, "Tilemap _Env", true);
                Tilemap tm = Util.FindChild<Tilemap>(map, "Tilemap _Collision", true);

                if (tm == null)
                {
                    Debug.LogError("Tile Map IS null");
                    continue;
                }
                using (var writer = File.CreateText($"{pathPrefix}/{map.name}.txt"))
                {
                    writer.WriteLine(tm.cellBounds.xMin);
                    writer.WriteLine(tm.cellBounds.xMax);
                    writer.WriteLine(tm.cellBounds.yMin);
                    writer.WriteLine(tm.cellBounds.yMax);

                    for (int y = tm.cellBounds.yMax; y >= tm.cellBounds.yMin; y--)
                    {
                        for (int x = tm.cellBounds.xMin; x <= tm.cellBounds.xMax; x++)
                        {
                            var tile = tm.GetTile(new Vector3Int(x, y, 0));
                            var envTile = tmEnv.GetTile(new Vector3Int(x,y,0));
                  
                            if (tile != null)
                            {
                                if(tile.name.Equals("portal") || tile.name.Equals("sign"))
                                    writer.Write("2");
                                else
                                    writer.Write("1");
                            }   
                            else
                                writer.Write("0");

                        }
                        writer.WriteLine();
                    }

                    Debug.Log("Finish");
                }
            }



        }
    }

#endif
}

