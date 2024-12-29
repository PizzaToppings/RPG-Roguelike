using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Character", menuName = "ScriptableObjects/Character")]
public class SO_Character : ScriptableObject
{
    public string Name;
    public List<ClassEnum> Classes;

    [Space]
    public int MaxHealth;
    public int MaxEnergy;
    public int MoveSpeed;
    public int Initiative;
    public int PhysicalPower;
    public int MagicalPower;
    public int PhysicalDefense;
    public int MagicalDefense;

    [Space]
    public SO_MainSkill basicAttack;
    public SO_MainSkill basicSkill;
    public SO_Trinket BasicTrinket;
}
