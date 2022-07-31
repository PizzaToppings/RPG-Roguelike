using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skillshot", menuName = "ScriptableObjects/Skillshot", order = 1)]
public class SO_Skillshot : ScriptableObject
{
    public enum OriginTileEnum {Caster, LastTarget, Click};

    [HideInInspector] public Unit Caster;
    public OriginTileEnum OriginTile;
    [HideInInspector] public BoardTile startTileTile;
    public int Damage;
    public int Range;

    // public DamageType DamageType;
    // debuffs

    public void Preview() 
    {
        Debug.Log("HEY");
    }
}
