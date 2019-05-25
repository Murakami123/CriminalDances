using System.Linq;
using UnityEditor;
using UnityEngine;

public static class HierarchyDispInvalidClass
{
    private const int WIDTH = 16;

    [InitializeOnLoadMethod]
    private static void HierarchyDispInvalid()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnGUI;
    }
    
    private static void OnGUI( int instanceID, Rect selectionRect )
    {
        // 〇行の色を変えるバージョン
        var go = EditorUtility.InstanceIDToObject( instanceID ) as GameObject;
        
        if ( go == null )
        {
            return;
        }
        
        // Warningのチェック
        var isWarning = go
            .GetComponents<MonoBehaviour>()
            .Any( c => c == null );
        if ( !isWarning )
        {
            return;
        }

        // 強調する範囲の指定        
        var pos     = selectionRect;
        pos.x       = 0;
        pos.width   = selectionRect.xMax; // 行の端から端まで？

        // 強調：色を変更する        
        var color = GUI.color;
        GUI.color = new Color( 1f, 1f, 1f, 1f );
        GUI.Box( pos, string.Empty );
        GUI.color = color;
        // GUI.Label( pos, "!!" );      


        // 〇"!!" を表示するだけのバージョン
        // var go = EditorUtility.InstanceIDToObject( instanceID ) as GameObject;
        
        // if ( go == null )
        // {
        //     return;
        // }
        
        // var isWarning = go
        //     .GetComponents<MonoBehaviour>()
        //     .Any( c => c == null );
        
        // if ( !isWarning )
        // {
        //     return;
        // }
        
        // var pos     = selectionRect;
        // pos.x       = pos.xMax - WIDTH;
        // pos.width   = WIDTH;
        
        // GUI.Label( pos, "!!" );
    }
}