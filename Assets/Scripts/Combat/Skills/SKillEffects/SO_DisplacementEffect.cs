using System;
using UnityEngine;

/// <summary>
/// Serializable displacement effect - embedded directly in skill parts
/// </summary>
[Serializable]
public class DisplacementEffect
{
    public bool UseDisplacement;

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
    public SO_SKillVFX StartVFX;
    public SO_SKillVFX EndVFX;
}