using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    public Transform target; // Target around which the camera will rotate
    public float speed = 5f; // Speed of rotation

    private bool m_isEnabled;
    void OnEnable()
    {
        AutoRotate();
    }

    async UniTask AutoRotate()
    {
        m_isEnabled = false;

        MetadataVisualizer.Instance.Enabled = false;

        float timeElapsed = 0;
        float rotationSpeed = 360f / 3f;  // Calculate speed for a full cycle in rotationTime seconds

        while (timeElapsed < 3f)
        {
            float step = rotationSpeed * Time.deltaTime;
            transform.RotateAround(target.position, Vector3.up, step);

            timeElapsed += Time.deltaTime;
            await UniTask.Yield();
        }

        MetadataVisualizer.Instance.Enabled = true;
        m_isEnabled = true;
    } 

    // Update is called once per frame
    void Update()
    {
        if (!m_isEnabled)
            return;

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
        
    }
}