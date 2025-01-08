using UnityEngine;

public class ManaburnStatusEffect : StatusEffect
{
    public int Power;

    public override void Apply()
    {
        base.Apply();

        Target.OnUnitTurnStartEvent.AddListener(BurnMana);
        Target.ThisHealthbar.AddStatusEffect(StatusEfectEnum.Manaburn);
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

        Target.ThisHealthbar.RemoveStatusEffect(StatusEfectEnum.Manaburn);
        Target.OnUnitTurnStartEvent.RemoveListener(ReduceDuration);
    }
}
