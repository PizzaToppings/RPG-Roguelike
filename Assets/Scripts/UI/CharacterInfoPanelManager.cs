using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterInfoPanelManager : MonoBehaviour
{
    public static CharacterInfoPanelManager Instance;

    [Header("Panel Root")]
    [SerializeField] private GameObject characterInfoPanel;

    [Header("Basic Info")]
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI physicalPowerText;
    [SerializeField] private TextMeshProUGUI magicalPowerText;
    [SerializeField] private TextMeshProUGUI physicalDefenseText;
    [SerializeField] private TextMeshProUGUI magicalDefenseText;
    [SerializeField] private TextMeshProUGUI resistancesText;
    [SerializeField] private TextMeshProUGUI vulnerabilitiesText;

    [Header("Character Specific")]
    [SerializeField] private TextMeshProUGUI energyText;

    private RectTransform panelRect;
    private Canvas parentCanvas;
    private bool isVisible;

    private void Awake()
    {
        Instance = this;
        panelRect = characterInfoPanel.GetComponent<RectTransform>();
        parentCanvas = characterInfoPanel.GetComponentInParent<Canvas>();
        characterInfoPanel.SetActive(false);
    }

    private void Update()
    {
        if (!isVisible) return;
    }

    /// <summary>Call from Character.OnMouseEnter to show the panel.</summary>
    public void ShowPanel(Character character)
    {
        if (characterInfoPanel == null) return;

        PopulateStats(character);

        characterInfoPanel.SetActive(true);
        isVisible = true;
    }

    /// <summary>Call from Character.OnMouseExit to hide the panel.</summary>
    public void HidePanel()
    {
        if (characterInfoPanel == null) return;
        characterInfoPanel.SetActive(false);
        isVisible = false;
    }

    private void PopulateStats(Character character)
    {
        if (characterNameText != null)
            characterNameText.text = character.UnitName;

        if (physicalPowerText != null)
            physicalPowerText.text = character.PhysicalPower.ToString();
        if (magicalPowerText != null)
            magicalPowerText.text = character.MagicalPower.ToString();
        if (physicalDefenseText != null)
            physicalDefenseText.text = character.PhysicalDefense.ToString();
        if (magicalDefenseText != null)
            magicalDefenseText.text = character.MagicalDefense.ToString();

        if (energyText != null)
            energyText.text = $"{character.Energy} / {character.MaxEnergy}";

        if (resistancesText != null)
            resistancesText.gameObject.SetActive(false);

        if (vulnerabilitiesText != null)
            vulnerabilitiesText.gameObject.SetActive(false);
    }

    private void ClampToScreen()
    {
        Vector3[] corners = new Vector3[4];
        panelRect.GetWorldCorners(corners);

        float rightOverflow  = corners[2].x - Screen.width;
        float bottomOverflow = corners[0].y;

        Vector2 pos = panelRect.position;

        if (rightOverflow > 0)
            pos.x -= rightOverflow;

        if (bottomOverflow < 0)
            pos.y -= bottomOverflow;

        panelRect.position = pos;
    }

}
