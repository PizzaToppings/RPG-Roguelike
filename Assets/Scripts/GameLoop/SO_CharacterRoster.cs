using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Create one of these assets and add all your SO_Character assets to the Characters list.
/// The RunManager will pick 3 random characters from this pool for the character selection screen.
/// Create via: right-click in Project window > ScriptableObjects > GameLoop > CharacterRoster
/// </summary>
[CreateAssetMenu(fileName = "CharacterRoster", menuName = "ScriptableObjects/GameLoop/CharacterRoster")]
public class SO_CharacterRoster : ScriptableObject
{
    [Tooltip("All characters available to the player. Add every SO_Character asset here.")]
    public List<SO_Character> Characters = new List<SO_Character>();
}
