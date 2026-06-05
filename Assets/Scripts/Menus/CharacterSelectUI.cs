using System.Collections;
using UnityEngine;


public class CharacterSelectUI : MonoBehaviour
{
    public static CharacterSelectUI Instance;

    [SerializeField] CharacterSelectCard[] cards;

    Coroutine showSkillCoroutine;

    [SerializeField] SkillInfoScreen infoScreen;
    [SerializeField] TraitInfoScreen traitInfoScreen;
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

    // Overload that accepts an anchor RectTransform to position the info panel
    public void StartShowSkillInformation(Skill skill, RectTransform anchor, int? displayPower = null)
    {
        // Start coroutine that positions using anchor when activating
        showSkillCoroutine = StartCoroutine(ShowSkillInformation(skill, displayPower, anchor));
    }

    // Trait hover methods for character select
    Coroutine showTraitCoroutine;

    public void StartShowTraitInformation(SO_Trait trait, RectTransform anchor)
    {
        showTraitCoroutine = StartCoroutine(ShowTraitInformation(trait, anchor));
    }

    public void EndShowTraitInformation(SO_Trait trait)
    {
        if (showTraitCoroutine != null) StopCoroutine(showTraitCoroutine);
        if (traitInfoScreen != null) traitInfoScreen.Deactivate();
    }

    public IEnumerator ShowTraitInformation(SO_Trait trait, RectTransform anchor)
    {
        yield return new WaitForSeconds(0.3f);
        if (traitInfoScreen != null) traitInfoScreen.Activate(trait, anchor);
    }

    public void EndShowSkillInformation(Skill skill)
    {
        StopCoroutine(showSkillCoroutine);

        infoScreen.Deactivate();
    }

    public IEnumerator ShowSkillInformation(Skill skill)
    {
        yield return new WaitForSeconds(0.3f);
        infoScreen.Activate(skill, false);
    }

    public IEnumerator ShowSkillInformation(Skill skill, int? displayPower, RectTransform anchor = null)
    {
        yield return new WaitForSeconds(0.3f);
        infoScreen.Activate(skill, false, displayPower, anchor);
    }
    public Sprite GetClassIcon(ClassEnum thisClass) => icons.GetClassIcon(thisClass);

}
