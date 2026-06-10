using UnityEngine;
using UnityEngine.UI;

public class SkillAugmentAssignButton : MonoBehaviour
{
    Button button;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    public void Setup(int offerIndex)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => RunManager.Instance.AssignShopSkillAugmentOffer(offerIndex));
    }
}
