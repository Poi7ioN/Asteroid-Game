using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is responsible for handling the movement of spawned powerups/enemies.
public class Movement : MonoBehaviour
{
    private float _MoveSpeed = 4;        //move speed default is 4f
    private Vector3 targetPosition;      //target position to move towards.
    private float _WaitTimer;           //delay till next move
    float waitCounter;                  //wait counter.

    private void Start()
    {
        // Set the initial target position to a random position inside the screen
        targetPosition = GetRandomPositionWithinBounds();
    }

    private void Update()
    {
        //check the distance between the tranform and next target position.
        if (Vector3.Distance(transform.position, targetPosition) <= 1f) 
        {
            // If the gameobject has reached its current target position, generate a new random target position
            waitCounter += Time.deltaTime;
            if (waitCounter >= _WaitTimer)
            {
                targetPosition = GetRandomPositionWithinBounds();
                waitCounter = 0f;  // reset the waitcounter to Zero.
            }
        }

        //Move towards the next target position with movespeed
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, _MoveSpeed * Time.deltaTime);
    }

    //get random position for next move
    private Vector3 GetRandomPositionWithinBounds()
    {
        _WaitTimer = Random.Range(3f, 10f);                     //random wait delay
        // Generate a random position in world space that is within the visible area of the game camera
        Vector3 worldPosition =  Camera.main.ViewportToWorldPoint(new Vector3(Random.value, Random.value,0f));
        worldPosition.z = 0f; // Clamp the z-axis to 0
        return worldPosition;
    }

    //set the movespeed of the gameobject
    public void SetEnemyMoveDataOnSpawn(float speed)
    {
       _MoveSpeed = speed;
    } 
}