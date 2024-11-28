using UnityEngine;
using UnityEditor;
using System.Numerics;
using Vector2 = UnityEngine.Vector2;
using System;
[CustomEditor(typeof(LidarSensor))]
public class GraphInspector : Editor
{
    private SerializedProperty lastLidarReadingsProperty;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        // Display a button in the inspector
        if (GUILayout.Button("Show Lidar Front"))
        {
            LidarSensor lr = (LidarSensor)target;
            lr.showFront(); 
        }
        if (GUILayout.Button("Single Pulse"))
        {
            LidarSensor lr = (LidarSensor)target;
            lr.singlePulse(); 
        }
    }
    //private void OnEnable()
    //{
    //    // Subscribe to the OnSceneGUI event to redraw the graph when the scene view is updated
    //    lastLidarReadingsProperty = serializedObject.FindProperty("lastLidarReadings");
    //    SceneView.duringSceneGui += OnSceneGUI;
    //}
//
    //private void OnDisable()
    //{
    //    // Unsubscribe from the OnSceneGUI event when the Inspector is disabled to avoid memory leaks
    //    SceneView.duringSceneGui -= OnSceneGUI;
    //}
//
    //public override void OnInspectorGUI()
    //{
    //    serializedObject.Update();
    //    // Call the base OnInspectorGUI method to draw the default Inspector GUI for the target component
    //    base.OnInspectorGUI();
    //    // Draw the graph in the Inspector using GUILayout.BeginVertical() and GUILayout.EndVertical()
    //    GUILayout.BeginVertical(GUI.skin.window);
    //    GUILayout.Label("Graph", EditorStyles.boldLabel);
    //    GUILayout.BeginHorizontal(GUI.skin.window);
    //    if (lastLidarReadingsProperty != null)
    //    {
    //        // Iterate over the last lidar readings array and draw the graph lines
    //        for (int i = 0; i < lastLidarReadingsProperty.arraySize - 1; i++)
    //        {
    //            SerializedProperty point1Property = lastLidarReadingsProperty.GetArrayElementAtIndex(i);
    //            SerializedProperty point2Property = lastLidarReadingsProperty.GetArrayElementAtIndex(i + 1);
    //            Vector2 offset = new Vector2(50, 400); 
    //            // Retrieve the Vector2 values from the serialized properties
    //            Vector2 point1 = new Vector2(point1Property.vector2Value.x, - point1Property.vector2Value.y) + offset;
    //            Vector2 point2 = new Vector2(point2Property.vector2Value.x, - point2Property.vector2Value.y) + offset;
//
    //            // Draw the line segment between the two points
    //            Handles.DrawLine(point1, point2);
    //        }
    //        // Apply any changes made to the serialized object
    //        serializedObject.ApplyModifiedProperties();
    //    }
    //    // Draw your graph here using GUILayout or EditorGUILayout
    //    
    //    GUILayout.EndHorizontal(); 
    //    GUILayout.EndVertical();
    //}

    
}