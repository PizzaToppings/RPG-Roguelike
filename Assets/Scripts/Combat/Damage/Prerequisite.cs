using UnityEngine;

public class SO_Prerequisite : ScriptableObject
{
    public virtual bool HasPrerequisite(Unit caster, Unit target)
    {
        return false;
    }
}
