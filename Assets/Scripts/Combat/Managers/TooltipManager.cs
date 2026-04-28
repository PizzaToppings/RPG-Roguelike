using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;

    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TextMeshProUGUI tooltipTitle;
    [SerializeField] private TextMeshProUGUI tooltipText;

    private void Awake()
    {
        Instance = this;
        tooltipPanel.SetActive(false); // Hide at start
    }

    public void ShowTooltip(string title, string text, Vector2 position)
    {
        tooltipTitle.text = title;
        tooltipText.text = text;
        tooltipPanel.transform.position = position;
        tooltipPanel.SetActive(true);
    }

    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }
}
