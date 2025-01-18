using UnityEngine;

[CreateAssetMenu(fileName = "Displacement", menuName = "ScriptableObjects/SkillEffects/DisplacementEffect")]
public class SO_DisplacementEffect : ScriptableObject
{
    public DisplacementEnum DisplacementType;
    public SO_Skillpart Unit;
    public SO_Skillpart TargetPosition;

    [Space]
    public float Delay;

    [Space]
    public AnimationCurve SpeedCurve = AnimationCurve.Constant(0, 1, 1);
    public float Speed = 1;
    public float Offset;

    [Space]
    public AnimationCurve HeightCurve = AnimationCurve.Constant(1, 1, 1);
    public float Height = 1;
    public float Duration;
    public bool UseDuration;

    [Space]
    public SO_SKillVFX VFX;
}
