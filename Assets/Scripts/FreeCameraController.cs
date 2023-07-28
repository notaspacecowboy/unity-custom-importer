using UnityEngine;
using System.Collections;

public class FreeCameraController : MonoBehaviour
{
    [SerializeField]
    private float m_mainSpeed = 100.0f; //regular speed

    [SerializeField]
    private float m_shiftAdd = 250.0f; //multiplied by how long shift is held.  Basically running

    [SerializeField]
    private float m_maxShift = 1000.0f; //Maximum speed when holdin gshift

    [SerializeField]
    private float m_camSens = 0.25f; //How sensitive it with mouse

    private Vector3 m_lastMouse = new Vector3(255, 255, 255); //kind of in the middle of the screen, rather than at the top (play)
    private float m_totalRun = 1.0f;



    void Start()
    {
        MetadataVisualizer.Instance.Enabled = true;
    }

    void Update()
    {
        UpdateCameraRotation();
        UpdateCameraPosition();
    }


    private void UpdateCameraRotation()
    {
        if(Input.GetMouseButtonDown(1))
            m_lastMouse = Input.mousePosition; 

        if (!Input.GetMouseButton(1))
            return;

        m_lastMouse = Input.mousePosition - m_lastMouse;
        m_lastMouse = new Vector3(-m_lastMouse.y * m_camSens, m_lastMouse.x * m_camSens, 0);
        m_lastMouse = new Vector3(transform.eulerAngles.x + m_lastMouse.x, transform.eulerAngles.y + m_lastMouse.y, 0);
        transform.eulerAngles = m_lastMouse;
        m_lastMouse = Input.mousePosition;
    }

    private Vector3 GetBaseInput()
    { //returns the basic values, if it's 0 than it's not active.
        Vector3 p_Velocity = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            p_Velocity += new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.S))
        {
            p_Velocity += new Vector3(0, 0, -1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            p_Velocity += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            p_Velocity += new Vector3(1, 0, 0);
        }
        return p_Velocity;
    }

    private void UpdateCameraPosition()
    {
        Vector3 velocity = GetBaseInput();

        if (velocity.sqrMagnitude > 0)
        { // only move while a direction key is pressed
            if (Input.GetKey(KeyCode.LeftShift))
            {
                m_totalRun += Time.deltaTime;
                velocity = velocity * m_totalRun * m_shiftAdd;
                velocity.x = Mathf.Clamp(velocity.x, -m_maxShift, m_maxShift);
                velocity.y = Mathf.Clamp(velocity.y, -m_maxShift, m_maxShift);
                velocity.z = Mathf.Clamp(velocity.z, -m_maxShift, m_maxShift);
            }
            else
            {
                m_totalRun = Mathf.Clamp(m_totalRun * 0.5f, 1f, 1000f);
                velocity = velocity * m_mainSpeed;
            }

            velocity = velocity * Time.deltaTime;
            Vector3 newPosition = transform.position;
            if (Input.GetKey(KeyCode.Space))
            { //If player wants to move on X and Z axis only
                transform.Translate(velocity);
                newPosition.x = transform.position.x;
                newPosition.z = transform.position.z;
                transform.position = newPosition;
            }
            else
            {
                transform.Translate(velocity);
            }
        }
    }
}