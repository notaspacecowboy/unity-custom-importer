using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    public Transform target; // Target around which the camera will rotate
    public float speed = 5f; // Speed of rotation

    // Update is called once per frame
    void Update()
    {
        // Check if 'Q' or 'E' is pressed
        if (Input.GetKey(KeyCode.Q))
        {
            // Rotate the camera counterclockwise around the target point at the given speed
            transform.RotateAround(target.position, Vector3.up, speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            // Rotate the camera clockwise around the target point at the given speed
            transform.RotateAround(target.position, -Vector3.up, speed * Time.deltaTime);
        }

        transform.LookAt(target, Vector3.up);
    }
}