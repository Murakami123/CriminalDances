using UnityEditor;

[InitializeOnLoad]
public static class GameStopPlayOnCompiled
{
    static GameStopPlayOnCompiled()
    {
        EditorApplication.update += Update;
    }

    private static void Update()
    {
        if ( EditorApplication.isCompiling && EditorApplication.isPlaying )
        {
            EditorApplication.isPlaying = false;
        }
    }
}