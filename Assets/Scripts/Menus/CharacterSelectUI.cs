using System.Collections;
using UnityEngine;


public class CharacterSelectUI : MonoBehaviour
{
    public static CharacterSelectUI Instance;

    [SerializeField] CharacterSelectCard[] cards;

    Coroutine showSkillCoroutine;

    [SerializeField] InfoScreen infoScreen;
    [SerializeField] SO_UI_Icons icons;

    void Start()
    {
        Instance = this;

        if (RunManager.Instance == null)
        {
            Debug.LogWarning("CharacterSelectUI: RunManager not found in scene. Start from MainMenuScene or ensure RunManager GameObject exists.");
            return;
        }

        var options = RunManager.Instance.GetCharacterOptions();

        for (int i = 0; i < cards.Length; i++)
        {
            if (i < options.Length)
                cards[i].Setup(options[i]);
            else
                cards[i].gameObject.SetActive(false);
        }
    }

    public void StartShowSkillInformation(Skill skill)
    {
        showSkillCoroutine = StartCoroutine(ShowSkillInformation(skill));
    }

    public void EndShowSkillInformation(Skill skill)
    {
        StopCoroutine(showSkillCoroutine);

        infoScreen.Deactivate();
    }

    public IEnumerator ShowSkillInformation(Skill skill)
    {
        yield return new WaitForSeconds(1);
        infoScreen.Activate(skill, false);
    }
    public Sprite GetClassIcon(ClassEnum thisClass) => icons.GetClassIcon(thisClass);

}
