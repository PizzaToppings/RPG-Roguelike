using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Create one of these assets and add all non-basic special skills to the Skills list.
/// Only skills that share a class with the selected character will be offered during skill selection.
/// Skills with an empty Classes list are offered to all characters ("Universal" skills).
/// Create via: right-click in Project window > ScriptableObjects > GameLoop > SkillPool
/// </summary>
[CreateAssetMenu(fileName = "SkillPool", menuName = "ScriptableObjects/GameLoop/SkillPool")]
public class SO_SkillPool : ScriptableObject
{
    [Tooltip("All special skills that can be offered during a run. Avoid adding basic attack/skill assets here.")]
    public List<SO_MainSkill> Skills = new List<SO_MainSkill>();
}
