using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(sceneManagement))]
public class sceneManagementGUI : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        sceneManagement script = (sceneManagement)target;
        if (GUILayout.Button("Test datastorage"))
        {
            script.storeLidarData();;
        }
        if (GUILayout.Button("Take Screenshot"))
        {
            script.takeScreenshot();
        }
        if (GUILayout.Button("Randomize Scene"))
        {
            script.RandomizeScene();
        }
        if (GUILayout.Button("Randomize Rotation"))
        {
            script.randomizeLightRotation();
        }
        if(GUILayout.Button("Randomize Floor Texture")){
            script.randomizeFloorTexture(); 
        }
        if(GUILayout.Button("Create random spot lighting")){
            script.createSpotGrid();  
        }
        if(GUILayout.Button("Randomize spotlight temprature")){
            script.randomizeSpotTemp();   
        }
        if(GUILayout.Button("Randomize spotlight brightness")){
            script.randomizeSpotIntence();   
        }
        if(GUILayout.Button("Destroy Spotlights")){
            script.destroySpotlights();   
        }
         if(GUILayout.Button("Create walls")){
            script.createWalls();   
        }
        if(GUILayout.Button("Randomize Wall Locations")){
            script.randomizeWallCoords();   
        }
        if(GUILayout.Button("Randomized Wall Textures")){
            script.randomizeWallTexture();   
        }
        if(GUILayout.Button("Destroy walls")){
            script.destoryWalls();   
        }
        if (GUILayout.Button("Create objects"))
        {
            script.createObjects();
        }
        if(GUILayout.Button("Destroy Objects")){ 
            script.destroyObjects(); 
        }
    }
}
