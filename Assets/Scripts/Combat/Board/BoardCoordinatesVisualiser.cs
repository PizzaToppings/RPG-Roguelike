using UnityEngine;

public class BoardCoordinatesVisualiser : MonoBehaviour
{
    [SerializeField]
    private bool showCoordinates = true;

    public static bool ShowCoordinates { get; private set; }

    private void OnValidate()
    {
        ShowCoordinates = showCoordinates;
    }

    private void Awake()
    {
        ShowCoordinates = showCoordinates;
    }
}
