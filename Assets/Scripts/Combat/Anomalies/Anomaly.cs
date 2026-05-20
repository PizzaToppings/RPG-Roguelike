/// <summary>
/// Runtime instance of an active Anomaly during combat.
/// </summary>
public class Anomaly
{
    public SO_Anomaly anomalySO;

    public int chargeCount;
    public bool hasTriggered;

    public void Init(SO_Anomaly so)
    {
        anomalySO = so;
        chargeCount = 0;
        hasTriggered = false;

        so.Init(this);
    }
}
