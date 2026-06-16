using UnityEngine;

public class ExposeGuardStatusEffect : DefaultStatusEffect
{
    public int Power; // positive for Exposed (+3), negative for Guarded (-2)

    public override void Apply()
    {
        base.Apply();
    }

    public override void EndEffect()
    {
        base.EndEffect();
    }
}
