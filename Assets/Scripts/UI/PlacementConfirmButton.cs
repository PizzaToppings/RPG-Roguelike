using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple button handler for confirming character placement.
/// Attach this to a UI Button that will confirm the placement phase.
/// </summary>
[RequireComponent(typeof(Button))]
public class PlacementConfirmButton : MonoBehaviour
{
    Button button;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    void Start()
    {
        // Wire up the button to call the placement manager's confirm method
        button.onClick.AddListener(OnConfirmClicked);
    }

    void OnConfirmClicked()
    {
        if (CharacterPlacementManager.Instance != null)
        {
            CharacterPlacementManager.Instance.ConfirmPlacement();
        }
        else
        {
            Debug.LogWarning("[PlacementConfirmButton] CharacterPlacementManager instance not found!");
        }
    }

    void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(OnConfirmClicked);
    }
}
