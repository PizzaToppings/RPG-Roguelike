using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillAugmentCard : MonoBehaviour
{
    [SerializeField] Image characterImage;
    [SerializeField] TextMeshProUGUI characterNameText;

    [SerializeField] Image skillImage;
    [SerializeField] TextMeshProUGUI skillNameText;

    [SerializeField] TextMeshProUGUI augmentNameText;
    [SerializeField] TextMeshProUGUI augmentDescriptionText;

    int offerIndex = -1;

    public void Setup(int index, SO_MainSkill skill, SO_SkillAugment augment)
    {
        offerIndex = index;

        // Find owner character for display
        var ownerIdx = RunManager.Instance.FindPartyMemberIndexForSkill_Public(skill);
        if (ownerIdx >= 0 && ownerIdx < RunData.Party.Count)
        {
            var member = RunData.Party[ownerIdx];
            if (member.Character != null)
            {
                characterNameText.text = member.Character.Name;
                if (member.Character.Image != null)
                    characterImage.sprite = member.Character.Image;
            }
        }

        if (skill != null)
        {
            skillNameText.text = skill.SkillName;
            if (skill.Image != null)
                skillImage.sprite = skill.Image;
        }

        if (augment != null)
        {
            augmentNameText.text = augment.AugmentName;
            augmentDescriptionText.text = augment.Description;
        }
    }

    public void OnChoose()
    {
        RunManager.Instance.AssignShopSkillAugmentOffer(offerIndex);
    }
}
