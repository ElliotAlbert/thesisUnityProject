using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using Object = System.Object;
using Random = UnityEngine.Random;

public class sceneManagement : MonoBehaviour
{
    public GameObject lighting; 
    public GameObject lidar; 
    public Material[] materials; 
    public string reletiveMaterialsFolder = "Materials/Floor"; 
    [Header("Data creation settings")]
    public float timeSinceLastSS = 0;
    public bool makingData; 
    public string screenshotPath = "F:/TrainingData/Screenshots/";
    public string lidarPath = "F:/TrainingData/Lidar.txt"; 
    public int numberOfFiles = 0; 
    [Header("Scene Variables")]
    public bool indoor = true; 
    [Header("Lighting Variables")]
    public int xLightingFactor; 
    public int yLightingFactor; 
    public int spotNumber = 5; 
    public int spotxBound = 10; 
    public int spotyBound = 14; 
    public float spotIntenceLower = 1000; 
    public float spotIntenceUpper = 3500;
    public List<GameObject> spotlights; 
    [Header("Object Variables")]
    public GameObject[] activeObjects = new GameObject[0];
    public bool genericObjects = true; 
    [Header("Floor Variables")]
    public GameObject floor; 
    [Header("Wall Variables")]
    public GameObject wallPrefab; 
    public bool uniformWallMaterial = false; 
    //These shouldn't change too much however keeping them public is good for inspector 
    public float wall1XOffsetLower = -40f; 
    public float wall1XOffsetUpper = 0f; 
    public float wall2XOffsetLower = 6.25f;
    public float wall2XOffsetUpper = 40;
    public float wall3ZOffsetLower = -8; 
    public int wall3ZOffsetUpper = 24;
    public float[] wallApprox = new float[3];
    public GameObject[] walls = new GameObject[3];
    void Start()
    {
    }

    void Update()
    {
        timeSinceLastSS += Time.deltaTime;
        if(makingData){
            int counter = 0; 
            if(timeSinceLastSS > 0.3f){
                timeSinceLastSS=0; 
                if(Random.Range(0,2) == 1){
                    uniformWallMaterial = true;
                }else{
                    uniformWallMaterial = false; 
                }
                RandomizeScene(); 
                takeScreenshot(); 
                storeLidarData(); 
                counter++; 
            }
        }
    }
    public void RandomizeScene(){
        if (indoor == true){
            lighting.SetActive(false);
            destoryWalls(); 
            destroySpotlights(); 
            destroyObjects(); 
            createWalls(); 
            createSpotGrid(); 
            randomizeFloorTexture(); 
            randomizeSpotIntence(); 
            randomizeSpotTemp(); 
            randomizeWallTexture();
            createObjects(); 
        }else{
            lighting.SetActive(true);
            destoryWalls(); 
            destroySpotlights(); 
            destroyObjects(); 
            randomizeLightRotation(); 
            randomizeFloorTexture(); 
            createObjects(); 
        }
    }
    Material[] LoadMaterialsFromFolder(string folder)
    {
        List<Material> materialList = new List<Material>(); // Temporary list to store materials
        Object[] materials = Resources.LoadAll("Materials/" + folder, typeof(Material));
        if (materials != null && materials.Length > 0)
        {
            foreach (Material material in materials)
            {
                materialList.Add(material); // Add material to the list
            }
            Debug.Log("Materials loaded successfully."); // Debug message indicating successful loading
        }
        else
        {
            Debug.LogError("No materials found in the folder."); // Error message if no materials are found
            return null;
        }

        return materialList.ToArray(); // Convert the list to an array and return it
    }
    public void destroyObjects(){ 
        foreach (GameObject x in activeObjects){
            Destroy(x); 
        }
        activeObjects = null; 
    }
    public void randomizeObjectTexture(){ 
        Material[] materials = LoadMaterialsFromFolder("Object"); 
        foreach(GameObject x in activeObjects){
            Renderer objectRender = x.GetComponent<Renderer>();
            int index = Random.Range(0, materials.Length); 
            objectRender.material = materials[index]; 
        }
    }
    Object[] loadObjectsFromFolder(){
        string folder = "Basics/Prefabs"; 
        if(!genericObjects){
            folder = "Complex";
        }
        Object[] objects = Resources.LoadAll("Objects/" + folder, typeof(GameObject)); 
        if(objects != null && objects.Length >0){
            Debug.Log("Objects loaded successfully"); 
        }else{
            Debug.LogError("No Objects found in folder");
        }
        return objects; 
    }
    public void createObjects(){
        Object[] objects = loadObjectsFromFolder(); 
        int objectNumber = 0; 
        if(indoor){
            objectNumber = Random.Range(3, 10); 
        }
        else{
            objectNumber = Random.Range(5,15); 
        }
        activeObjects = new GameObject[objectNumber]; 
        if(objects!=null){
            for(int i = 0; i < objectNumber; i++){
                Vector3 objectCoords = new Vector3(0,0,0); 
                float y = Random.Range(0.49f, 2f);
                if(indoor){
                    float x = Random.Range(wallApprox[0],wallApprox[1]); 
                    float z = Random.Range(-8,wallApprox[2]); 
                    objectCoords = new Vector3(x,y,z); 
                    int objectIndex = Random.Range(0,objects.Length);
                    activeObjects[i] = Instantiate((GameObject)objects[objectIndex], objectCoords, Quaternion.identity); 
                }else{
                    float x = Random.Range(-4f,9.29f); 
                    float z = Random.Range(-6,2); 
                    objectCoords = new Vector3(x,y,z); 
                    int objectIndex = Random.Range(0,objects.Length);
                    activeObjects[i] = Instantiate((GameObject)objects[objectIndex], objectCoords, Quaternion.identity); 
                }
                // Randomize rotation this needs to be applied wether or not this is indoor 
                activeObjects[i].transform.rotation = new Quaternion(Random.Range(0,360),Random.Range(0,360),Random.Range(0,360),0); 
                 
            }
            randomizeObjectTexture();
        }
    }
    public void createWalls(){
       for(int i = 0; i < 3; i++){
        walls[i] = Instantiate(wallPrefab,new Vector3(0, 0,0), Quaternion.identity); 
       }
       randomizeWallCoords(); 
       randomizeWallTexture(); 
    }

    public void destoryWalls(){
        if (walls != null){
                foreach (GameObject wall in walls){
                Destroy(wall); 
                walls[Array.IndexOf(walls,wall)] = null;
                }
        }
        
    }
    // File path where the screenshot will be saved

    public void storeLidarData(){ 
        float[] lidarData = lidar.GetComponent<LidarSensor>().singlePulse(); 
        //Test line remove
        if(lidarData != null){
            string directory = Path.GetDirectoryName(lidarPath); 
            if(!Directory.Exists(directory)){
                Directory.CreateDirectory(directory); 
            }
            using (StreamWriter writer = new StreamWriter(lidarPath,true)){
                string output = ""; 
                for (int i = 0; i < lidarData.Length; i++)
                {
                    output += lidarData[i]; 
                    if (i < lidarData.Length - 1)
                    {
                        output+=',';
                    }else{
                        output+=(';');
                    }
                }
                writer.WriteLine(output);
            }
        }
    }

    // Function to take a screenshot and save it to the specified file path
    public void takeScreenshot()
    {
        if (!Directory.Exists(screenshotPath))
        {
            Directory.CreateDirectory(screenshotPath);
        }

        string fileName = "Screenshot_"+ numberOfFiles  + ".png";
        numberOfFiles++; 
        ScreenCapture.CaptureScreenshot(Path.Combine(screenshotPath, fileName));

        Debug.Log("Screenshot saved to: " + Path.Combine(screenshotPath, fileName));
    }

    public void randomizeFloorTexture(){ 
        Material[] materials = LoadMaterialsFromFolder("Floor"); 
        Renderer floor_renderer = floor.GetComponent<Renderer>(); 
        int index = Random.Range(0, materials.Length); 
        floor_renderer.material = materials[index]; 
    }
    
    public void randomizeWallCoords(){ 
        //Store the generated data for use in object and lighting randomization
        wallApprox[0] = Random.Range(wall1XOffsetLower,wall1XOffsetUpper); 
        wallApprox[1] = Random.Range(wall2XOffsetLower,wall2XOffsetUpper); 
        wallApprox[2] = Random.Range(wall3ZOffsetLower,wall3ZOffsetUpper);
        walls[0].transform.position = new Vector3(wallApprox[0],0,0); 
        walls[1].transform.position = new Vector3(wallApprox[1],0,0); 
        walls[2].transform.position = new Vector3(0,0,wallApprox[2]); 
        walls[2].transform.rotation = Quaternion.Euler(0,90,0); 
    }
    public int randomizeWallTexture(){ 
        Material[] materials = LoadMaterialsFromFolder("Wall"); 
        //Copying c based error handling
        if (materials == null){return 1;}
        //Create starting index from wall texture 
        int index = Random.Range(0,materials.Length); 
        foreach (GameObject wall in walls){
            Renderer wall_renderer = wall.GetComponent<Renderer>();
            if(!uniformWallMaterial){
                //If uniform wall textures not selected, create a new index for each wall
                index = Random.Range(0,materials.Length); 
            }
            wall_renderer.material = materials[index]; 
        }
        return 0; 
    }
    public int calcSpotNumber(){
        float xDifference = wallApprox[1] - wallApprox[0];
        float yDifference = wallApprox[2] - wall3ZOffsetLower; 
        int xLightingNumber =(int) (xDifference/xLightingFactor); 
        int yLightingNumber =(int) (yDifference/yLightingFactor);
        return (xLightingNumber + yLightingNumber)/2; 
    }
    public void createSpotGrid(){ 
        List<GameObject> spots = new List<GameObject>();
        List<Light> lights = new List<Light>(); 
        float ligthingTemp = Random.Range(5500, 14000);
        spotNumber = calcSpotNumber(); 
        for(int i = 0; i < spotNumber; i++){
            GameObject spot = new GameObject("spot " + i);
            Light ls = spot.AddComponent<Light>();
            ls.type = UnityEngine.LightType.Spot; 
            ls.lightmapBakeType = LightmapBakeType.Realtime;
            ls.intensity = Random.Range(spotIntenceLower, spotIntenceUpper); 
            ls.range = 110; 
            ls.colorTemperature = ligthingTemp; 
            float x = Random.Range(wallApprox[0],wallApprox[1]); 
            float y = Random.Range(-8,wallApprox[2]); 
            ls.transform.position = new Vector3(x,30,y);
            ls.transform.rotation = Quaternion.Euler(90,0,0);
            spotlights.Add(spot); 
        }
    }
    public void randomizeSpotIntence(){
        if(!spotlights.IsUnityNull() && !(spotlights.Count == 0)){
            float lightingIntensity = Random.Range(spotIntenceLower,spotIntenceUpper); 
            foreach(GameObject ls in spotlights){
                ls.GetComponent<Light>().intensity = lightingIntensity; 
            }
        }
    }
    public void randomizeSpotTemp(){
        if(!spotlights.IsUnityNull() && !(spotlights.Count == 0)){
            float ligthingTemp = Random.Range(5500, 14000);
            foreach (GameObject ls in spotlights){
                ls.GetComponent<Light>().colorTemperature = ligthingTemp;
            }
        }
        
    }
    public void destroySpotlights(){ 
        if(!spotlights.IsUnityNull() && !(spotlights.Count == 0)){
            float ligthingTemp = Random.Range(5500, 14000);
            foreach (GameObject ls in spotlights){
                GameObject.Destroy(ls); 
            }
            spotlights.Clear(); 
        }
    }
    public void randomizeLightRotation(){
        int x = Random.Range(0,180); 
        int y = Random.Range(0,360); 
        lighting.transform.rotation = Quaternion.Euler(x,y,0); 
    }
}
