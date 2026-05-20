public class Trait
{
    public SO_Trait traitSO;

    public int chargeCount;
    public bool hasTriggered;

    public void Init(SO_Trait so, Character character)
    {
        traitSO = so;
        chargeCount = 0;
        hasTriggered = false;

        so.Init(character, this);
    }
}
