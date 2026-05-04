using UnityEngine;
using UnityEngine.UI;

public class CharacterPortrait : Healthbar
{
    [SerializeField] Image portraitImage;

    public override void UpdateHealthbar()
    {
        base.UpdateHealthbar();
    }

    public void Set()
    {
        var character = thisUnit as Character;
        if (character == null) return;

        var partyMember = character.partyMemberIndex < RunData.Party.Count
            ? RunData.Party[character.partyMemberIndex]
            : null;

        if (partyMember != null && portraitImage != null)
            portraitImage.sprite = partyMember.Character.Image;
    }
}
