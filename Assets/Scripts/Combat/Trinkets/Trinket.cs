public class Trinket
{
    public SO_Trinket trinketSO;

    public int chargeCount;
    public bool hasTriggered;

    public void Init(SO_Trinket so, Character character)
    {
        trinketSO = so;
        chargeCount = 0;
        hasTriggered = false;

        so.Init(character, this);
    }
}
