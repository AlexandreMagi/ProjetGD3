using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/M_Ui")]
public class M_Ui : ScriptableObject
{
    [Header ("CrossHair Point")]
    public float PointRotationSpeed;
    public float PointSpeedTransitionHit;
    public float PointHitSizeBonus;
    public Color PointBaseColor;
    public Color PointHitColor;

    public float PointReculTir;
    public float PointReculRetourViseur;
    public float PointBaseSizeViseur;
    public float PointSpeedIdle;
    public float PointAmplitudeIdle;
    public float PointOffsetIdle;

    public float PointChargedSize;
    public float PointChargedRotation;
    public float PointChargedSpeed;

    public Color PointChargedColor;

    [Header ("CrossHair Circle")]
    public float CircleRotationSpeed;
    public float CircleSpeedTransitionHit;
    public float CircleHitSizeBonus;
    public Color CircleBaseColor;
    public Color CircleHitColor;

    public float CircleReculTir;
    public float CircleReculRetourViseur;
    public float CircleBaseSizeViseur;
    public float CircleSpeedIdle;
    public float CircleAmplitudeIdle;
    public float CircleOffsetIdle;

    public float CircleChargedSize;
    public float CircleChargedRotation;
    public float CircleChargedSpeed;
    public float CircleChargedAmplitude;

    public Color CircleChargedColor;

    [Header ("CrossHair Straight")]
    public float StraightRotationSpeed;
    public float StraightSpeedTransitionHit;
    public float StraightHitSizeBonus;
    public Color StraightBaseColor;
    public Color StraightHitColor;

    public float StraightReculTir;
    public float StraightReculRetourViseur;
    public float StraightBaseSizeViseur;
    public float StraightSpeedIdle;
    public float StraightAmplitudeIdle;
    public float StraightOffsetIdle;

    public float StraightChargedSize;
    public float StraightChargedRotation;
    public float StraightChargedSpeed;
    public float StraightChargedAmplitude;

    public Color StraightChargedColor;
}

