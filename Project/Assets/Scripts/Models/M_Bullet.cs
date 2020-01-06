using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/M_Bullet")]
public class M_Bullet : ScriptableObject
{

    public string BulletName = "Base";

    public bool bIsInstant;
    public bool bAffectedByGravity;
    public bool bExplosionAtImpact;
    public bool bExplosionDealsDamage;
    public bool bBouncingBullet;
    public bool bPiercingBullet;
    public bool bActivateStopTimeAtImpact;
    public bool bActivateSLowMoAtImpact;
    public bool bDammageFadeWithDistance;
    public bool bStunFadeWithDistance;

    public LayerMask layerMaskHit;

    [RangeAttribute(1, 100)]
    public int nDamage;
    [RangeAttribute(0f,1f)]
    public float fTimeStopAtImpact;
    [RangeAttribute(0f,70f)]
    public float fImpulse;
    [RangeAttribute(5f, 50f)]
    public float fBulletMass;
    [RangeAttribute(0.01f, 25f)]
    public float fExplosionRange;
    [RangeAttribute(1f, 5000f)]
    public float fExplosionForce;
    [RangeAttribute(0f, 50f)]
    public float ShakePerExplosion;
    [RangeAttribute(0f, 0.999f)]
    public float fSlowMoPower;
    [RangeAttribute(0f, 5f)]
    public float fSlowMoDuration;
    [RangeAttribute(0f, 1f)]
    public float fSlowMoProbability;
    [RangeAttribute(0f, 50f)]
    public float fDistanceDammageFade;
    [RangeAttribute(0f, 50f)]
    public float fDistanceStunFade;

    [RangeAttribute(1,10)]
    public int nImpactBeforeDie;

    [RangeAttribute(1, 100)]
    public int nBounceForce;

    public float StunValue;

    [RangeAttribute(-1, 20)]
    public int nPiercingsMax;


    public LayerMask piercedLayers;

    [Tooltip("Plus c'est haut, moins le pierce est précis, mais plus il est rapide à calculer"), RangeAttribute(.01f, .5f)]
    public float pierceStep;
}
