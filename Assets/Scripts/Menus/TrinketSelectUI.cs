using UnityEngine;

public class TraitSelectUI : MonoBehaviour
{
    public static TraitSelectUI Instance;

    [SerializeField] TraitSelectCard[] cards;
    [SerializeField] SO_UI_Icons icons;

    void Start()
    {
        Instance = this;

        if (RunManager.Instance == null)
        {
            Debug.LogWarning("TraitSelectUI: RunManager not found. Start from MainMenuScene or ensure RunManager GameObject exists.");
            return;
        }

        var options = RunManager.Instance.GetTraitOptions();

        if (options.Length == 0)
            Debug.LogWarning("TraitSelectUI: No traits available. Make sure SO_TraitPool has traits assigned.");

        for (int i = 0; i < cards.Length; i++)
        {
            if (i < options.Length)
            {
                cards[i].gameObject.SetActive(true);
                cards[i].Setup(options[i]);
            }
            else
            {
                cards[i].gameObject.SetActive(false);
            }
        }
    }

    public Sprite GetClassIcon(ClassEnum thisClass) => icons.GetClassIcon(thisClass);
}
