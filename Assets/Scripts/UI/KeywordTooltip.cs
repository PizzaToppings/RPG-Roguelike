using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class KeywordTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI tmpText;
    private Camera uiCamera;
    private TooltipManager tooltipManager;

    private void Awake()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
        uiCamera = GetComponentInParent<Canvas>().worldCamera;
        tooltipManager = TooltipManager.Instance;
    }

    private void Update()
    {
        if (Input.mousePosition != null)
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(tmpText, Input.mousePosition, uiCamera);
            if (linkIndex != -1)
            {
                TMP_LinkInfo linkInfo = tmpText.textInfo.linkInfo[linkIndex];
                string keyword = linkInfo.GetLinkID();
                ShowToolTip(keyword);
            }
            else
            {
                tooltipManager.HideTooltip();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData) { }
    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Instance.HideTooltip();
    }

    void ShowToolTip(string keyword)
    {
        var tooltiptext = string.Empty;
        switch (keyword)
        {
            case "Bleed":
                tooltiptext = "Deal physical damage at the end of the unit's turn. While this is active, all physical damage done is increase by 2.";
                tooltipManager.ShowTooltip(keyword, tooltiptext, Input.mousePosition);
                return;

            case "Burn":
                tooltiptext = "At the end of the unit's turn, explode to deal fire damage to the unit and those within 2 range. If Burn is applied to a target effected by Burn, trigger the first one.";
                tooltipManager.ShowTooltip(keyword, tooltiptext, Input.mousePosition);
                return;

            case "Poison":
                tooltiptext = "Deal poison damage at the end of the units turn. Doesn't fade and applying new poison increase the damage.";
                tooltipManager.ShowTooltip(keyword, tooltiptext, Input.mousePosition);
                return;
        }
    }
}
