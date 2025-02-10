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

            case "Blinded":
                tooltiptext = "Cannot use Martial Skills.";
                tooltipManager.ShowTooltip(keyword, tooltiptext, Input.mousePosition);
                return;

            case "Silenced":
                tooltiptext = "Cannot use Magical Skills.";
                tooltipManager.ShowTooltip(keyword, tooltiptext, Input.mousePosition);
                return;

            case "Frightened":
                tooltiptext = "The unit is less likely to target the caster with skills.";
                tooltipManager.ShowTooltip(keyword, tooltiptext, Input.mousePosition);
                return;

            case "Taunt":
                tooltiptext = "The unit is more likely to target the caster with skills.";
                tooltipManager.ShowTooltip(keyword, tooltiptext, Input.mousePosition);
                return;

            case "Stunned":
                tooltiptext = "The unit cannot act next turn.";
                tooltipManager.ShowTooltip(keyword, tooltiptext, Input.mousePosition);
                return;

            case "Incpacitated":
                tooltiptext = "The unit cannot act next turn. Last until damaged.";
                tooltipManager.ShowTooltip(keyword, tooltiptext, Input.mousePosition);
                return;

            case "Regen":
                tooltiptext = "Regain health at the end of the turn.";
                tooltipManager.ShowTooltip(keyword, tooltiptext, Input.mousePosition);
                return;

            case "Rooted":
                tooltiptext = "Movement speed is 0.";
                tooltipManager.ShowTooltip(keyword, tooltiptext, Input.mousePosition);
                return;

            case "Hidden":
                tooltiptext = "The unit cannot be targeted by skills. Ends when the unit is damaged.";
                tooltipManager.ShowTooltip(keyword, tooltiptext, Input.mousePosition);
                return;

            case "Fatique":
                tooltiptext = "The unit starts the next turn with less Energy.";
                tooltipManager.ShowTooltip(keyword, tooltiptext, Input.mousePosition);
                return;

            case "Lifedrain":
                tooltiptext = "Restore health whenever the unit deals damage.";
                tooltipManager.ShowTooltip(keyword, tooltiptext, Input.mousePosition);
                return;

            case "Thorns":
                tooltiptext = "Deal damage to anyone who attacks the unit in melee range.";
                tooltipManager.ShowTooltip(keyword, tooltiptext, Input.mousePosition);
                return;

            case "Dodge":
                tooltiptext = "Negate the first time the unit would take damage. Lasts one turn.";
                tooltipManager.ShowTooltip(keyword, tooltiptext, Input.mousePosition);
                return;
        }
    }
}
