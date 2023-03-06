using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script binds the gameobject within the camera bounds.
 public class StayInside : MonoBehaviour
{
    // Initialize the screen constraints and buffer distance.
    float leftConstraint;
    float rightConstraint;
    float bottomConstraint;
    float topConstraint;
    [SerializeField] float buffer;  // small buffer to add to the new teleported position 

    // Initialize the camera and distance between camera and object.
    Camera cam;
    float distanceZ;

    void Start()
    {
        cam = Camera.main;
        
        // Get the main camera and distance between camera and object.
        distanceZ = Mathf.Abs(cam.transform.position.z + transform.position.z); 

        // Convert screen coordinates to world coordinates to get screen edges respectively.
        leftConstraint   = cam.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, distanceZ)).x;
        rightConstraint  = cam.ScreenToWorldPoint(new Vector3(Screen.width, 0.0f, distanceZ)).x;
        bottomConstraint = cam.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, distanceZ)).y;
        topConstraint    = cam.ScreenToWorldPoint(new Vector3(0.0f, Screen.height, distanceZ)).y;
    }

    void Update()
    {
        // Get the current position of the object.
        Vector3 position = transform.position;

        // Check if the player has gone beyond the screen boundary and teleport it to the other side.

        if (position.x < leftConstraint - buffer)
        {
            position.x = rightConstraint + buffer;
        }
        else if (position.x > rightConstraint + buffer)
        {
            position.x = leftConstraint - buffer;
        }

        if (position.y < bottomConstraint - buffer)
        {
            position.y = topConstraint + buffer;
        }
        else if (position.y > topConstraint + buffer)
        {
            position.y = bottomConstraint - buffer;
        }

        // Update the position of the object.
        transform.position = position;
    }
}