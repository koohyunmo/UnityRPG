using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MultiplayerBuildAndRun
{

#if UNITY_EDITOR

    [MenuItem("Tool/Run Multiplayer/ 2 Player")]
    static void PerformWin64Build2()
    {
        PerforWin64Build(2);
    }
    [MenuItem("Tool/Run Multiplayer/ 3 Player")]
    static void PerformWin64Build3()
    {
        PerforWin64Build(3);
    }
    [MenuItem("Tool/Run Multiplayer/ 4 Player")]
    static void PerformWin64Build4()
    {
        PerforWin64Build(4);
    }

    static void PerforWin64Build(int PlayerCOunt)
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(
            BuildTargetGroup.Standalone,
            BuildTarget.StandaloneWindows
        );

        for(int i =1; i <= PlayerCOunt; i++)
        {
            BuildPipeline.BuildPlayer(
            GetScenePaths(),
            "Build/Win64/" + GetProjectName() + i.ToString() + "/" + GetProjectName() + i.ToString() + ".exe",
            BuildTarget.StandaloneWindows64,
            BuildOptions.AutoRunPlayer
            );
        }

    }

    static string GetProjectName()
    {
        string[] s = Application.dataPath.Split('/');
        var result = s[s.Length - 2];
        Debug.Log(result);

        return result;
    }

    static string[] GetScenePaths()
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];

        for(int i =0; i < scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }

        return scenes;
    }
#endif

}
