using UnityEngine;

[CreateAssetMenu(fileName = "RunProgression", menuName = "ScriptableObjects/GameLoop/RunProgression")]
public class SO_RunProgression : ScriptableObject
{
    public ProgressionStep[] progressionSteps;
}
