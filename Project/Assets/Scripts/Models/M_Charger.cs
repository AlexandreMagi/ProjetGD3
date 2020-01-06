using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Enemies/Charger")]
public class M_Charger : M_Enemy
{

    [Header("Rotation")]
    public float rotationSpeed;
    public float rotationMinimalBeforeCharge;
   
    [Header("Vitesse")]
    public float timeWaitBeforeDash;
    public float speedAtStart;
    public float speedAccel;

    [Header("Specific Stun")]
    public float AfterStunDistanceNeededToReCharge;
    public float StunRecoil;

    [Header("Dégats")]
    public bool dammageGoesWithSpeed;
    public float dammagePerSpeedMultiplier;
    public float distanceBeforeImpact;
    public float tooFarawayDistance;
    public float ImpactTimeRecover;
    public float ImpactTimeRecoil;
    public float fRepositioningSpeed;
    public int cacDammage;
    public float delayBetweenCacAttack;
    public float ShakeAtImpact;

    [Header("Skin")]
    public Material StartColor;
    public Material ChargeColor;
    public Material CacColor;

}
