using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform camRotator;

    [Space]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotationSpeed = 100f;
    [SerializeField] float zoomSpeed = 5f;

    [Space]
    [SerializeField] float panToCharacterSpeed = 60f;

    [Space]
    [SerializeField] Vector2 zoomRange = new Vector2(5f, 50f);
    [SerializeField] Vector2 RotateRange = new Vector2(-40, 40);

    bool receivingMovementInput = false;

    void Update()
    {
        Move();
        Zoom();
        Rotate();
    }

    Transform camTransform;

    void Start()
    {
        camTransform = Camera.main.transform;
    }

    void Move()
    {
        Vector3 moveDirection = Vector3.zero;
        Vector3 mousePosition = Input.mousePosition;

        if (Input.GetKey(KeyCode.W) || mousePosition.y >= Screen.height - 10)
            moveDirection += transform.forward;

        if (Input.GetKey(KeyCode.A) || mousePosition.x <= 10)
            moveDirection -= transform.right;

        if (Input.GetKey(KeyCode.S) || mousePosition.y <= 10)
            moveDirection -= transform.forward;

        if (Input.GetKey(KeyCode.D) || mousePosition.x >= Screen.width - 10)
            moveDirection += transform.right;

        receivingMovementInput = moveDirection != Vector3.zero;

        if (receivingMovementInput)
        {
            moveDirection.Normalize();
            transform.position += moveDirection * FixedMoveSpeed();
        }
    }

    void Zoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll == 0)
            return;

        Vector3 newPosition = camTransform.position + camTransform.forward * Time.deltaTime * scroll * zoomSpeed;
        float newDistance = Vector3.Distance(newPosition, camTransform.parent.position);

        if (newDistance >= zoomRange.x && newDistance <= zoomRange.y)
            camTransform.position = newPosition;
    }

    void Rotate()
    {
        if (Input.GetMouseButton(1)) // Right mouse button
        {
            float rotateHorizontal = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, rotateHorizontal, Space.World);
            
            float rotateVertical = -Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            Vector3 currentRotation = camRotator.localEulerAngles;
            float newXRotation = currentRotation.x + rotateVertical;
            if (newXRotation > 180) newXRotation -= 360;
            newXRotation = Mathf.Clamp(newXRotation, RotateRange.x, RotateRange.y);

            camRotator.localEulerAngles = new Vector3(newXRotation, currentRotation.y, currentRotation.z);
        }
    }

	public IEnumerator MoveToUnit(Unit unit)
	{
        var startDistance = Vector3.Distance(unit.position, transform.position);
        var distance = startDistance;

        while (UnitData.ActiveUnit == unit && receivingMovementInput == false) 
        {
            distance = Vector3.Distance(unit.position, transform.position);
            var distanceSpeed = distance / startDistance;
            var speed = panToCharacterSpeed * distanceSpeed * Time.deltaTime;

            transform.position = Vector3.MoveTowards(transform.position, unit.transform.position, speed);

            yield return null;
        }
    }

    float FixedMoveSpeed()
    {
        var zoomDistance = Vector3.Distance(camTransform.position, camTransform.parent.position);
        var speed = moveSpeed * Time.deltaTime * (zoomDistance * 0.4f);
        return speed;
    }
}
