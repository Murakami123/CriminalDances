using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public class ApplicationBuilder
{
    /// <summary>
    /// アプリアイコンを変更します
    /// </summary>
    /// <param name="platform">変更するビルドプラットフォーム</param>
    /// <param name="path">アイコン画像パス</param>
    public static void SetIcon(BuildTarget platform, string path)
    {
        // アイコン読み込み
        var bytes = File.ReadAllBytes(path);

        // バイナリから縦幅・横幅を取得する
        // ※ pngファイル前提
        int pos = 16;
        int width = 0;
        for (int i = 0; i < 4; i++)
        {
            width = width * 256 + bytes[pos++];
        }

        int height = 0;
        for (int i = 0; i < 4; i++)
        {
            height = height * 256 + bytes[pos++];
        }

        var texture = new Texture2D(width, height);
        texture = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
        texture.LoadImage(bytes);
        texture.filterMode = FilterMode.Point;

        PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, new[] { texture });
    }


    // Jenkinsの自動ビルドから呼ばれます
    // 製品版のみ対応
    public static void BuildSettingAndRun()
    {
        PlayerSettings.productName = "オンエア！";
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.colyinc.onAir");
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, "com.colyinc.onAir");
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Standalone, "com.colyinc.onAir");

#if UNITY_ANDROID
        // Jenkins側のworkspace内
        PlayerSettings.Android.keystoreName = Application.dataPath + "/../../onAir.keystore";

        var lines = File.ReadAllLines(Application.dataPath + "/../../keystore_pass");

        for (var i = 0; i < lines.Length; i++)
        {
            if (i == 0)
            {
                PlayerSettings.Android.keystorePass = lines[i];
            }
            else if (i == 1)
            {
                PlayerSettings.Android.keyaliasName = lines[i];
            }
            else if (i == 2)
            {
                PlayerSettings.Android.keyaliasPass = lines[i];
            }
        }
#endif

        BuildTarget platform = BuildTarget.NoTarget;

        string[] args = System.Environment.GetCommandLineArgs();

        for (var i = 0; i < args.Length; ++i)
        {
            switch (args[i])
            {
                case "-platform":
                    {
                        platform = (BuildTarget)System.Enum.Parse(typeof(BuildTarget), args[i + 1]);
                        break;
                    }

                case "-versionName":
                    {
                        PlayerSettings.bundleVersion = args[i + 1];
                        break;
                    }

                case "-versionCode":
                    {
                        PlayerSettings.Android.bundleVersionCode = int.Parse(args[i + 1]);
                        break;
                    }

                case "-buildNumber":
                    {
                        PlayerSettings.iOS.buildNumber = args[i + 1];
                        break;
                    }
            }
        }

        RunBuild(platform);
    }

    public static void RunBuild(BuildTarget targetPlatform)
    {
        string outputPath = "";

        if (targetPlatform == BuildTarget.Android)
        {
            // Assetsより1つ上のフォルダに apk を作成
            outputPath = Application.dataPath + "/../APKs";
        }
        else
        {
            // Assetsより1つ上のフォルダにXcodeプロジェクトを作成
            outputPath = Application.dataPath + "/../iOSProject";
        }

        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, outputPath, targetPlatform, BuildOptions.None);
    }

    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
    {
#if UNITY_IOS
        var plistPath = Path.Combine(path, "Info.plist");
        var plist = new PlistDocument();
        plist.ReadFromFile(plistPath);
        plist.root.SetString("ITSAppUsesNonExemptEncryption", "false");
        plist.WriteToFile(plistPath);
#endif
    }
}