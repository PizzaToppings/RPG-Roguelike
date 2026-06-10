using UnityEngine;

public class SkillAugmentSelectUI : MonoBehaviour
{
    public static SkillAugmentSelectUI Instance;

    [SerializeField] SkillAugmentCard[] cards;

    void Start()
    {
        Instance = this;

        if (RunData.CurrentShopSkillAugmentOffers == null || RunData.CurrentShopSkillAugmentOffers.Count == 0)
        {
            Debug.LogWarning("SkillAugmentSelectUI: No augment offers available.");
            return;
        }

        for (int i = 0; i < cards.Length; i++)
        {
            if (i < RunData.CurrentShopSkillAugmentOffers.Count)
            {
                var offer = RunData.CurrentShopSkillAugmentOffers[i];
                cards[i].gameObject.SetActive(true);
                cards[i].Setup(i, offer.Skill, offer.Augment);
            }
            else
            {
                cards[i].gameObject.SetActive(false);
            }
        }
    }
}
