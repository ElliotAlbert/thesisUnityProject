using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class LidarSensor : MonoBehaviour
{
    public bool pulsing; 
    public Gradient colorGradient;
    public float maxRange = 200f;
    public int numRays = 1080;
    public float angleIncrement = 0.25f;
    public float pulseInterval = 1f / 400f;
    public int scanningRange = 270; 
    public float timeSinceLastPulse = 0f; 
    public float sensitivity = 1f; 
    public Vector2[] lastLidarReadings; 
    int counter = 0; 
    public LidarGraphWindow lidarGraphWindow; 
    public bool showNonHits = false; 
    public bool showHits = true; 


    private void Start() {
        lidarGraphWindow = EditorWindow.GetWindow<LidarGraphWindow>(); 
        int newRange = Convert.ToInt32(scanningRange / angleIncrement);
        lastLidarReadings = new Vector2[newRange]; 
        
    }
    public void showFront(){ 
        RaycastHit hit; 
        Vector3 direction = transform.forward;
        Physics.Raycast(transform.position, direction, out hit, 500); 
        Debug.DrawLine(transform.position, direction * 100f, Color.black);
        // Debug.Log("Front drawn" + transform.position + hit.point);
    }
    private void FixedUpdate()
    {
        int newRange = Convert.ToInt32(scanningRange / angleIncrement);
        if(lastLidarReadings.Length != newRange){
            lastLidarReadings = new Vector2[newRange]; 
        }
        
        Vector3 direction2 = Quaternion.Euler(0, 0, 0) * transform.forward;
        Debug.DrawLine(transform.position, direction2 * 100f, Color.black);
        timeSinceLastPulse += Time.fixedDeltaTime;
        // Check if it's time to pulse
        if(pulsing){
            if (timeSinceLastPulse >= pulseInterval)
            {
                timeSinceLastPulse -= pulseInterval; // Reset time since last pulse
                counter=0; 
                // Perform raycasting

                for (float angle = (0 - scanningRange/2)+ transform.rotation.y; angle < scanningRange /2; angle += angleIncrement)
                {
                    Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;
                    RaycastHit hit;

                    if (Physics.Raycast(transform.position, direction, out hit, maxRange))
                    {
                        float distance = hit.distance;
                        lastLidarReadings[counter] = new Vector2(angle + scanningRange/2, hit.distance);
                        if(showHits){
                            //This is only used for debugging and not relevant to lidar functionallty 
                            float normalizedDistance = Mathf.Clamp01(Mathf.Pow(distance, sensitivity));
                            Color lineColor = colorGradient.Evaluate(normalizedDistance);
                            // Debug.Log(distance); 
                            // Handle detection of objects
                            Debug.DrawLine(transform.position, hit.point, lineColor);
                        }

                    }
                    else
                    {
                        // No object detected
                        if(showNonHits){
                            Debug.DrawRay(transform.position, direction * maxRange, Color.black);
                        }

                        lastLidarReadings[counter] = new Vector2(angle + scanningRange/2, maxRange); 


                    }
                    if (lidarGraphWindow != null)
                    {
                        lidarGraphWindow.SetVectorFromOtherComponent(lastLidarReadings);
                    }
                    counter++; 
                }
            }
        }
        
    }
    public float[] singlePulse(){
        counter=0; 
        // Perform raycasting
        int newRange = Convert.ToInt32(scanningRange / angleIncrement);
        if(lastLidarReadings.Length != newRange){
            lastLidarReadings = new Vector2[newRange]; 
        }
        for (float angle = (0 - scanningRange/2)+ transform.rotation.y; angle < scanningRange /2; angle += angleIncrement)
        {
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;
            RaycastHit hit;            
            if (Physics.Raycast(transform.position, direction, out hit, maxRange))
            {
                float distance = hit.distance;
                lastLidarReadings[counter] = new Vector2(angle + scanningRange/2, hit.distance);
            }
            else
            {
                lastLidarReadings[counter] = new Vector2(angle + scanningRange/2, maxRange);   
            }
            if (lidarGraphWindow != null)
            {
                lidarGraphWindow.SetVectorFromOtherComponent(lastLidarReadings);
            }
            counter++; 
        }
        float[] floats = new float[lastLidarReadings.Length];
        counter = 0; 
        foreach (Vector2 vt in lastLidarReadings){
            floats[counter] = vt.y;
            counter ++; 
        }
        return floats; 
    }

}
