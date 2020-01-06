using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/M_WeaponMod")]
public class M_WeaponMod : ScriptableObject
{
    [Header("Modes de tir")]
    public bool bOneShotAtATime;
    public bool bFireRateIndependantFromTimeScale = true;

    [RangeAttribute(0.01f, 25f)]
    public float fFireRate;
    [RangeAttribute(0f, 0.5f)]
    public float fBaseImprecision;
    [RangeAttribute(0f, 0.5f)]
    public float fImprecisionGainPerShot;
    [RangeAttribute(0f, 5f)]
    public float fImprecisionLostPerSec;
    [RangeAttribute(0f, 0.5f)]
    public float fMaxImprecision;
    [RangeAttribute(0f, 50f)]
    public float ShakePerShot;
    [RangeAttribute(0f, 50f)]
    public float ShakePerHit;

    [Header("Recul")]
    [RangeAttribute(0f, 1.5f)]
    public float RecoilPerShot = 0.2f;
    [RangeAttribute(0f, 1.5f)]
    public float RecoilPerGravityBullet = 0.5f;

    [Header("Other")]
    [RangeAttribute(0.001f, 5)]
    public float ChargeSpeed;

    [RangeAttribute(1, 100)]
    public int nBulletPerShoot;

    [Header("Rechargement")]
    public bool bMustReload;
    public bool bMustHoldToReload;
    public bool bCanCutReload;
    public bool bBulletPerBullet;
    public bool bReloadResetnBullet;

    [RangeAttribute(0.01f, 5f)]
    public float fReloadTime;

    [RangeAttribute(1, 50)]
    public int nBulletMax;

    [Header("Tir de gravité")]

    [RangeAttribute(0f, 10f)]
    public float gravityCooldown;

    public bool bTriggerGrav;

}
