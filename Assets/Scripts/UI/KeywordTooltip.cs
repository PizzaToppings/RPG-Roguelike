using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class KeywordTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI tmpText;
    private Camera uiCamera;

    private void Awake()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
        uiCamera = Camera.main; // Adjust if using a different UI camera
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
                TooltipManager.Instance.ShowTooltip(keyword, Input.mousePosition);
            }
            else
            {
                TooltipManager.Instance.HideTooltip();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData) { }
    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Instance.HideTooltip();
    }
}
