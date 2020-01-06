using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/M_CamFB")]
public class M_CamFB : ScriptableObject
{
    public M_StepCurveValues[] CurvesAndValues;

    [Tooltip("Plus la valeur est haute, moins elle sera smooth")]
    public float CamFollowSpeed = 5;
    [Tooltip("Plus la valeur est haute, plus la caméra va rotate lors des mouvements latéraux")]
    public float MaxRotateWhileMoving = 5;
    [Tooltip("Valeur de detection d'arret de mouvement")]
    public float fFrenquencyGoBackToZero = 1;
    [Tooltip("Valeur de decceleration")]
    public float fFrequencyDeccel = 0.02f;
    [Tooltip("Valeur de FOV en fonction de la viteses")]
    public float fFovMultiplier = 5f;
    [Tooltip("Valeur de transition entre FOV")]
    public float fFovSpeed = 1;
    [Range(0.1f, 0.9f)]
    public float fStepSoundPlay = 0.7f;
}
