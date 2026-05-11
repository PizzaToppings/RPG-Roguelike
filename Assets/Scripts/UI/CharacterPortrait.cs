using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterPortrait : Healthbar
{
    [SerializeField] Image portraitImage;
    [SerializeField] TextMeshProUGUI characterName;

    public void Set()
    {
        gameObject.SetActive(true);

        var character = thisUnit as Character;
        if (character == null) return;

        if (characterName != null)
            characterName.text = character.UnitName;

        var partyMember = character.partyMemberIndex < RunData.Party.Count
            ? RunData.Party[character.partyMemberIndex]
            : null;

        if (partyMember != null && portraitImage != null)
            portraitImage.sprite = partyMember.Character.Image;
    }
}
