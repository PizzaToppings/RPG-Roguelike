using UnityEngine;

public class SkillSelectUI : MonoBehaviour
{
    public static SkillSelectUI Instance;

    [SerializeField] SkillSelectCard[] cards;
    [SerializeField] SO_UI_Icons icons;

    void Start()
    {
        Instance = this;

        if (RunManager.Instance == null)
        {
            Debug.LogWarning("SkillSelectUI: RunManager not found. Start from MainMenuScene or ensure RunManager GameObject exists.");
            return;
        }

        var options = RunManager.Instance.GetSkillOptionsForParty();

        if (options.Length == 0)
            Debug.LogWarning("SkillSelectUI: No eligible skills available. Make sure SO_SkillPool has skills matching the party's classes.");

        for (int i = 0; i < cards.Length; i++)
        {
            if (i < options.Length)
            {
                Debug.Log(options[i] != null
                    ? $"Setting up card {i} with skill: {options[i].SkillName}"
                    : $"Warning: Skill option {i} is null.");
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
