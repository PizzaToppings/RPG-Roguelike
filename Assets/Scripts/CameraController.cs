using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform camRotator;

    [Space]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float rotationSpeed = 100f;
    [SerializeField] float zoomSpeed = 5f;
    [SerializeField] Vector2 zoomRange = new Vector2(5f, 50f);

    [Space]
    [SerializeField] float panToCharacterSpeed = 60f;

    void Update()
    {
        Move();
        Scroll();
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
        if (Input.GetKey(KeyCode.W))
            transform.position += transform.forward * Time.deltaTime * moveSpeed;

        if (Input.GetKey(KeyCode.A))
            transform.position -= transform.right * Time.deltaTime * moveSpeed;

        if (Input.GetKey(KeyCode.S))
            transform.position -= transform.forward * Time.deltaTime * moveSpeed;

        if (Input.GetKey(KeyCode.D))
            transform.position += transform.right * Time.deltaTime * moveSpeed;
    }

    void Scroll()
    {
        Vector3 mousePosition = Input.mousePosition;

        if (mousePosition.x >= Screen.width - 10)
            transform.position += transform.right * Time.deltaTime * moveSpeed;
        else if (mousePosition.x <= 10)
            transform.position += -transform.right * Time.deltaTime * moveSpeed;

        if (mousePosition.y >= Screen.height - 10)
            transform.position += transform.forward * Time.deltaTime * moveSpeed;
        else if (mousePosition.y <= 10)
            transform.position += -transform.forward * Time.deltaTime * moveSpeed;
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
            float rotateVertical = -Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            transform.Rotate(Vector3.up, rotateHorizontal, Space.World);
            camRotator.Rotate(Vector3.right, rotateVertical, Space.Self);
        }
    }

	public IEnumerator MoveToUnit(Unit unit)
	{
        var startDistance = Vector3.Distance(unit.position, transform.position);
        var distance = startDistance;

        while (distance > 0.1f)
        {
            distance = Vector3.Distance(unit.position, transform.position);
            var distanceSpeed = distance / startDistance;
            var speed = panToCharacterSpeed * distanceSpeed * Time.deltaTime;

            transform.position = Vector3.MoveTowards(transform.position, unit.transform.position, speed);

            yield return null;
        }
    }
}
