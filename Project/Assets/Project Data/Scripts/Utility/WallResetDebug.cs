using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(BreakableWall))]
public class WallResetDebug : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        BreakableWall breakableWall = (BreakableWall)target;
        if (GUILayout.Button("Reset Walls"))
        {
            breakableWall.ResetWall();
        }
    }
}
#endif
