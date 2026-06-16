using UnityEngine;

public class EmpowerWeakenStatusEffect : DefaultStatusEffect
{
    public int Power; // positive for Empowered (+3), negative for Weakened (-2)

    public override void Apply()
    {
        base.Apply();
    }

    public override void EndEffect()
    {
        base.EndEffect();
    }
}
