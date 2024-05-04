using UnityEngine;
using UnityEditor;
using System.IO;

public class SpritePixelsPerUnitChanger : EditorWindow
{
    #if UNITY_EDITOR
    //Assets/Tiny RPG Forest/Artwork/sprites/hero
    private string folderPath = "Assets/Tiny RPG Forest/Artwork/sprites/hero/"; // 변경하고 싶은 스프라이트들이 위치한 폴더 경로

    [MenuItem("Tools/Change Sprites Pixels Per Unit")]
    public static void ShowWindow()
    {
        GetWindow(typeof(SpritePixelsPerUnitChanger), true, "Sprite PPU Changer");
    }

    void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        folderPath = EditorGUILayout.TextField("Folder Path", folderPath);

        if (GUILayout.Button("Change Pixels Per Unit"))
        {
            ChangePixelsPerUnitForAllSprites(folderPath);
        }
    }

    private static void ChangePixelsPerUnitForAllSprites(string folderPath)
    {
        string fullPath = Path.GetFullPath(folderPath);
        var filePaths = Directory.GetFiles(fullPath, "*.png", SearchOption.AllDirectories); // 또는 "*.jpg" 등 필요에 맞게 변경

        foreach (var filePath in filePaths)
        {
            string assetPath = "Assets" + filePath.Replace(Path.GetFullPath("."), "").Replace('\\', '/');
            TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;

            if (textureImporter != null)
            {
                textureImporter.spritePixelsPerUnit = 20; // Pixels Per Unit 값을 20으로 설정
                textureImporter.SaveAndReimport();
            }
        }

        Debug.Log("Completed changing Pixels Per Unit for all sprites in folder: " + folderPath);
    }
    #endif
}
