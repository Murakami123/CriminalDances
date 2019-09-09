using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public class ApplicationBuilder
{
    // ビルド実行でAndroidのapkを作成する例
    [UnityEditor.MenuItem("Build/WebGL")]
    public static void BuildProjectAllSceneAndroid()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.WebGL);
        List<string> allScene = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                allScene.Add(scene.path);
            }
        }

        // Assetsより1つ上のフォルダに ビルド後ファイル を作成
        var outputPath = Application.dataPath + "/../Build_WebGL/WebGL";

        PlayerSettings.applicationIdentifier = "com.yourcompany.newgame";
        PlayerSettings.statusBarHidden = true;
        BuildPipeline.BuildPlayer(
            allScene.ToArray(),
            outputPath,
            BuildTarget.WebGL,
            BuildOptions.None
        );
    }
}