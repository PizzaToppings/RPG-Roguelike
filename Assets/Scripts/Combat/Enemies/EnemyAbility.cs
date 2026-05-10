/// <summary>
/// Runtime wrapper for an enemy ability, mirroring the Trinket class.
/// Tracks charge counts and one-shot trigger state.
/// </summary>
public class EnemyAbility
{
    public SO_EnemyAbility abilitySO;
    public int  chargeCount;
    public bool hasTriggered;

    public void Init(SO_EnemyAbility so, Enemy enemy)
    {
        abilitySO    = so;
        chargeCount  = 0;
        hasTriggered = false;
        so.Init(enemy, this);
    }
}
