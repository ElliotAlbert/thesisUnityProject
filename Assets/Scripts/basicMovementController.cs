using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// This movement controller was made in order to test and calibrate my lidar whilest keeping my camera view open 
public class basicMovementController : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of movement
    public float rotationSpeed = 180f; // Speed of rotation

    // Update is called once per frame
    void Update()
    {
        // Get horizontal and vertical input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate movement and rotation
        Vector3 movement = transform.forward * verticalInput * moveSpeed * Time.deltaTime;
        Quaternion rotation = Quaternion.Euler(0f, horizontalInput * rotationSpeed * Time.deltaTime, 0f);

        // Apply movement and rotation
        transform.Translate(movement, Space.World);
        transform.Rotate(rotation.eulerAngles);
    }
}
