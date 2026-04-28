using System.Collections.Generic;
using UnityEngine;

public class SO_Trinket : ScriptableObject
{
    public Sprite Image;
    public List<ClassEnum> classes;

    [Range(1, 4)]
    public int Rarity; 
    
    [TextArea(15, 20)]
    public string Description;

    public virtual void Init(Character character)
    {

    }
}
