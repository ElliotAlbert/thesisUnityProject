
using UnityEngine;
using UnityEditor;
using System;
 
public class LidarGraphWindow : EditorWindow
{
    public Vector2[] lidarData; 
    private SerializedProperty lastLidarReadingsProperty;

    [MenuItem("Lidar/GraphView")]
    public static void ShowWindow(){ 
        GetWindow<LidarGraphWindow>("Lidar Graph");

    }
    public void SetVectorFromOtherComponent(Vector2[] otherComponentVector){
        lidarData = otherComponentVector; 
    }
    private void OnGUI()
    {
        GUILayout.Label("Vectors from Other Component:");

        if (lidarData != null)
        {
            DrawGraph(lidarData);
        }
        else
        {
            GUILayout.Label("No vectors available");
        }
    }
    private void Update() {
        Repaint(); 
    }
    // Method to draw the graph
    private void DrawGraph(Vector2[] vectors)
    {
        Rect graphRect = GUILayoutUtility.GetRect(100,300); // Adjust the size as needed
        Handles.DrawSolidRectangleWithOutline(graphRect, Color.white, Color.black);

        if (vectors.Length > 1)
        {
            Handles.color = Color.blue;

            Vector3[] graphPoints = new Vector3[vectors.Length];
            
            for (int i = 0; i < vectors.Length; i++)
            {
                graphPoints[i] = new Vector3(vectors[i].x + 45f , (vectors[i].y - 223) *-1, 0f);
                
            }

            Handles.DrawAAPolyLine(2f, graphPoints);
        }
    }
}
