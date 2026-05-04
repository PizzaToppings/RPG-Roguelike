using UnityEngine;

/// <summary>
/// Place on a GameObject in the combat scene (alongside EncounterManager).
/// On Awake – before CombatManager.Start runs – this:
///   1. Clears any placeholder character GameObjects under the character parent.
///   2. Instantiates the prefab for each party member in RunData.Party.
///   3. Sets each character's partyMemberIndex and starting grid position.
///
/// If RunData.Party is empty (e.g. testing directly in the editor without going
/// through the run loop) the scene's existing character GameObjects are used unchanged.
///
/// Setup in Inspector:
///   - Character Parent       : the same Transform that UnitManager uses as its characterParent.
///   - Party Start Positions  : grid (x, y) coordinates for each party slot (index 0-3).
///                              Defaults to column 0, rows 7-10.
/// </summary>
public class PartyManager : MonoBehaviour
{
    [SerializeField] Transform  characterParent;
    [SerializeField] Vector2Int[] partyStartPositions =
    {
        new Vector2Int(0, 7),
        new Vector2Int(0, 8),
        new Vector2Int(0, 9),
        new Vector2Int(0, 10)
    };

    void Awake()
    {
        if (RunData.Party.Count == 0)
            return; // direct scene testing – keep existing placeholder characters

        ClearExistingCharacters();
        SpawnPartyMembers();
    }

    void ClearExistingCharacters()
    {
        // DestroyImmediate so removals take effect before CombatManager.Start() iterates children.
        for (int i = characterParent.childCount - 1; i >= 0; i--)
            DestroyImmediate(characterParent.GetChild(i).gameObject);
    }

    void SpawnPartyMembers()
    {
        for (int i = 0; i < RunData.Party.Count; i++)
        {
            var member = RunData.Party[i];

            if (member.Character == null || member.Character.CharacterPrefab == null)
            {
                Debug.LogWarning($"PartyManager: Party member {i} ({member.Character?.Name}) has no prefab assigned.");
                continue;
            }

            var instance  = Instantiate(member.Character.CharacterPrefab, characterParent);
            var character = instance.GetComponent<Character>();

            if (character != null)
            {
                character.partyMemberIndex = i;

                var pos = i < partyStartPositions.Length
                    ? partyStartPositions[i]
                    : new Vector2Int(0, i);

                character.startXPosition = pos.x;
                character.startYPosition = pos.y;
            }
            else
            {
                Debug.LogWarning($"PartyManager: Prefab for '{member.Character.Name}' does not have a Character component.");
            }
        }
    }
}
