using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Composes multiple SO_Anomaly assets into a single anomaly.
/// Each sub-anomaly gets its own runtime Anomaly instance, so chargeCount
/// and hasTriggered are tracked independently per sub-anomaly.
/// </summary>
[CreateAssetMenu(fileName = "MultiAnomaly", menuName = "ScriptableObjects/Anomalies/MultiAnomaly")]
public class MultiAnomaly : SO_Anomaly
{
    [Tooltip("All anomalies that will be initialised when this anomaly is applied.")]
    public List<SO_Anomaly> Anomalies = new List<SO_Anomaly>();

    public override void Init(Anomaly anomaly)
    {
        foreach (var subSO in Anomalies)
        {
            if (subSO == null) continue;

            var subAnomaly = new Anomaly();
            subAnomaly.Init(subSO);
        }
    }
}
