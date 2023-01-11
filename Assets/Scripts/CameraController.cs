using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] int moveSpeed = 1;

    Transform camTransform;



    void Start()
    {
        camTransform = transform.GetChild(0);
    }

    void Update()
    {
        Move();
        Rotate();
    }

    void Move()
    {
        if (Input.GetKey(KeyCode.W)) 
        {
            transform.position += transform.forward *Time.deltaTime * moveSpeed;
        }

        if (Input.GetKey(KeyCode.A)) 
        {
            transform.position -= transform.right *Time.deltaTime * moveSpeed;
        }

        if (Input.GetKey(KeyCode.S)) 
        {
            transform.position -= transform.forward *Time.deltaTime * moveSpeed;
        }

        if (Input.GetKey(KeyCode.D)) 
        {
            transform.position += transform.right *Time.deltaTime * moveSpeed;
        }
    }

    void Rotate()
    {

        if (Input.GetKey(KeyCode.Mouse1))
        {
           camTransform.eulerAngles -= new Vector3(Input.GetAxis("Mouse Y"), 0, 0);
           transform.eulerAngles += new Vector3(0, Input.GetAxis("Mouse X"), 0);
        }

    }
}
