using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float zoomSpeed = 3f;
    [SerializeField] float mouseDragPanSensitivity = 0.12f;

    [Space]
    [SerializeField] float panToCharacterSpeed = 60f;

    [Space]
    [SerializeField] Vector2 zoomRange = new Vector2(3f, 20f);

    [Space]
    [SerializeField] Vector2 xRange = new Vector2(-10f, 10f);
    [SerializeField] Vector2 yRange = new Vector2(-10f, 10f);

    bool receivingMovementInput = false;
    bool isRightMouseDragging = false;

    Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        HandleMouseDragState();
        Move();
        Zoom();
    }

    void OnDisable()
    {
        EndMouseDragPan();
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            EndMouseDragPan();
    }

    void HandleMouseDragState()
    {
        if (Input.GetMouseButtonDown(1))
            BeginMouseDragPan();

        if (Input.GetMouseButtonUp(1))
            EndMouseDragPan();
    }

    void BeginMouseDragPan()
    {
        if (isRightMouseDragging)
            return;

        isRightMouseDragging = true;
    }

    void EndMouseDragPan()
    {
        if (!isRightMouseDragging)
            return;

        isRightMouseDragging = false;
    }

    void Move()
    {
        Vector3 moveDirection = Vector3.zero;

        if (isRightMouseDragging)
        {
            // Drag movement uses raw mouse delta so speed reflects drag intensity.
            moveDirection = new Vector3(-Input.GetAxisRaw("Mouse X"), -Input.GetAxisRaw("Mouse Y"), 0f);
            receivingMovementInput = moveDirection.sqrMagnitude > 0f;

            if (receivingMovementInput)
            {
                Vector3 newPos = transform.position + moveDirection * moveSpeed * mouseDragPanSensitivity * cam.orthographicSize;
                newPos.x = Mathf.Clamp(newPos.x, xRange.x, xRange.y);
                newPos.y = Mathf.Clamp(newPos.y, yRange.x, yRange.y);
                transform.position = newPos;
            }

            return;
        }

        Vector3 mousePosition = Input.mousePosition;

        if (Input.GetKey(KeyCode.W) || mousePosition.y >= Screen.height - 10)
            moveDirection += Vector3.up;

        if (Input.GetKey(KeyCode.A) || mousePosition.x <= 10)
            moveDirection -= Vector3.right;

        if (Input.GetKey(KeyCode.S) || mousePosition.y <= 10)
            moveDirection -= Vector3.up;

        if (Input.GetKey(KeyCode.D) || mousePosition.x >= Screen.width - 10)
            moveDirection += Vector3.right;

        receivingMovementInput = moveDirection != Vector3.zero;

        if (receivingMovementInput)
        {
            moveDirection.Normalize();
            // Scale speed by zoom level so panning feels consistent at any zoom.
            Vector3 newPos = transform.position + moveDirection * moveSpeed * cam.orthographicSize * Time.deltaTime;
            newPos.x = Mathf.Clamp(newPos.x, xRange.x, xRange.y);
            newPos.y = Mathf.Clamp(newPos.y, yRange.x, yRange.y);
            transform.position = newPos;
        }
    }

    void Zoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll == 0) return;

        float newSize = cam.orthographicSize - scroll * zoomSpeed;
        cam.orthographicSize = Mathf.Clamp(newSize, zoomRange.x, zoomRange.y);
    }

    public IEnumerator MoveToUnit(Unit unit)
    {
        while (UnitData.ActiveUnit == unit && !receivingMovementInput)
        {
            Vector3 target = new Vector3(
                unit.transform.position.x,
                unit.transform.position.y,
                transform.position.z);

            float distance = Vector3.Distance(transform.position, target);
            float speed = panToCharacterSpeed * (distance / 10f) * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, speed);

            yield return null;
        }
    }

    // Kept to avoid breaking any remaining callers; no longer used internally.
    float FixedMoveSpeed()
    {
        return moveSpeed * cam.orthographicSize * Time.deltaTime;
    }
}
