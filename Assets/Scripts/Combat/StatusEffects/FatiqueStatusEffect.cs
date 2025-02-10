using UnityEngine;

public class FatiqueStatusEffect : StatusEffect
{
    public int Power;

    public override void Apply()
    {
        base.Apply();

        Target.OnUnitTurnStartEvent.AddListener(BurnMana);
        Target.ThisHealthbar.AddStatusEffect(StatusEffectEnum.Fatique);
    }

    public void BurnMana()
    {
        var character = Target as Character;
        character.Energy -= Power;
        character.ThisHealthbar.UpdateHealthbar();

        Duration--;

        if (Duration == 0)
            EndEffect();
    }

    public override void EndEffect()
    {
        base.EndEffect();

        Target.ThisHealthbar.RemoveStatusEffect(StatusEffectEnum.Fatique);
        Target.OnUnitTurnStartEvent.RemoveListener(ReduceDuration);
    }
}
