using UnityEngine;

public class TrinketSelectUI : MonoBehaviour
{
    public static TrinketSelectUI Instance;

    [SerializeField] TrinketSelectCard[] cards;
    [SerializeField] SO_UI_Icons icons;

    void Start()
    {
        Instance = this;

        if (RunManager.Instance == null)
        {
            Debug.LogWarning("TrinketSelectUI: RunManager not found. Start from MainMenuScene or ensure RunManager GameObject exists.");
            return;
        }

        var options = RunManager.Instance.GetTrinketOptions();

        if (options.Length == 0)
            Debug.LogWarning("TrinketSelectUI: No trinkets available. Make sure SO_TrinketPool has trinkets assigned.");

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
