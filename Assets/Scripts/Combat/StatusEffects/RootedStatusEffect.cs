public class RootedStatusEffect : StatusEffect
{
    public override void Apply()
    {
        base.Apply();

        SubscribeDurationTrigger();
        Target.ThisHealthbar.AddStatusEffect(statusEfectType);

        // Hide the enemy's damage prediction since they can no longer move to engage
        if (Target is EnemyBaseAI enemyAI)
            (enemyAI.ThisHealthbar as FloatingHealthbar)?.HideDamagePreview();
    }

    public override void EndEffect()
    {
        base.EndEffect();

        Target.ThisHealthbar.RemoveStatusEffect(statusEfectType);
        UnsubscribeDurationTrigger();

        // Restore the enemy's intent and damage prediction when Rooted expires
        if (Target is EnemyBaseAI enemyAI)
            (enemyAI.ThisHealthbar as FloatingHealthbar)?.UpdateIntent(enemyAI.CurrentSkill);
    }
}
