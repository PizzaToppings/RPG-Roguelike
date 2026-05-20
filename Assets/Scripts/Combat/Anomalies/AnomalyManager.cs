using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Initialises all active Anomalies at the start of a combat scene.
/// Place this on a GameObject in the combat scene alongside CombatManager.
/// </summary>
public class AnomalyManager : MonoBehaviour
{
    public static AnomalyManager Instance { get; private set; }

    readonly List<Anomaly> activeAnomalies = new List<Anomaly>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        InitAnomalies();
    }

    public void InitAnomalies()
    {
        activeAnomalies.Clear();

        foreach (var so in RunData.ActiveAnomalies)
        {
            if (so == null) continue;
            var anomaly = new Anomaly();
            anomaly.Init(so);
            activeAnomalies.Add(anomaly);
        }

        Debug.Log($"[AnomalyManager] Initialised {activeAnomalies.Count} anomaly(s).");
    }

    /// <summary>Returns a read-only view of the currently active anomalies.</summary>
    public IReadOnlyList<Anomaly> ActiveAnomalies => activeAnomalies;
}
