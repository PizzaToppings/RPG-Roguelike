using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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

    Vector2 savedMousePosition;

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    [DllImport("user32.dll")] static extern bool SetCursorPos(int X, int Y);
    [DllImport("user32.dll")] static extern bool GetCursorPos(out POINT point);
    [StructLayout(LayoutKind.Sequential)]
    struct POINT { public int X, Y; }
#endif

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
        if (Input.GetMouseButtonDown(1))
        {
            // Stop interaction with the currently hovered tile, same as OnMouseExit
            if (BoardData.CurrentMouseTile != null)
                BoardData.CurrentMouseTile.UnTarget();

            savedMousePosition = Input.mousePosition;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            GetCursorPos(out POINT p);
            savedMousePosition = new Vector2(p.X, p.Y);
#endif
            Cursor.visible = false;
        }

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

        if (Input.GetMouseButtonUp(1))
        {
            // Restore the cursor to where it was before rotating
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            SetCursorPos((int)savedMousePosition.x, (int)savedMousePosition.y);
#endif
            Cursor.visible = true;

            // Immediately trigger hover on whichever tile the cursor lands on
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                BoardTile tile = hit.collider.GetComponent<BoardTile>();
                if (tile != null)
                {
                    BoardData.CurrentMouseTile = tile;
                    tile.Target();
                }
            }
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
