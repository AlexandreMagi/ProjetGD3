using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WeaponChargedFullFB")]
public class M_WeaponChargedFullFB : ScriptableObject
{
    public AnimationCurve CurveValue;
    public float Speed = 3;
    public float FovMultiplier = 1;
    public float UiSizeMultiplier = 1;
}
