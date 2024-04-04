using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spotlight : MonoBehaviour
{
    public Vector2 size = new Vector2(1, 1);
    public Rigidbody2D m_Rigidbody;
    public Rigidbody2D connection;
    public Joint2D joint;
    public GameObject Player;
    public bool isConnected = false;
    public GameObject groundObject;
    public AudioSource barrelSlide;

    // This method is called when the script is loaded or a value is changed in the Inspector
    void OnValidate()
    {
        Resize();
    }

    void Start()
    {
        Resize();
        joint.connectedBody = null;

    }

    void Resize()
    {
        // Apply scaling based on the size variable
        transform.localScale = new Vector3(size.x, size.y, transform.localScale.z);
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (isConnected)
            {
                Disconnect();
            }
            else
            {
                Connect();
                barrelSlide.Play();
            }
        }
    }

    void Connect()
    {
        float distance = Vector2.Distance(transform.position, connection.transform.position);
        if (distance <= 1.5f) // Adjust the distance threshold as needed
        {
            joint.connectedBody = connection;
            isConnected = true;
            m_Rigidbody.constraints = RigidbodyConstraints2D.None;
            m_Rigidbody.constraints = RigidbodyConstraints2D.FreezePositionY;

        }
    }

    void Disconnect()
    {
        joint.connectedBody = null;
        isConnected = false;
        m_Rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
    }


}

