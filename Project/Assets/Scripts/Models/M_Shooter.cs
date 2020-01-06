using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Enemies/Shooter")]
public class M_Shooter : M_Enemy
{

    [Header("Rotation")]
    public float rotationSpeed;
    public float rotationMinimalBeforeCharge;
   
    [Header("Tir")]
    public float timeWaitBeforeShoot;
    public int nbShootPerSalve;
    public int nbBulletPerShoot = 1;
    public float timeBulletBetweenSalve;
    public float amplitudeMultiplier;
    public float recoverTime;
    public bool shootEvenIfPlayerMoving = false;

    [Header("Specific Stun")]
    public float StunRecoil;

    [Header("Skin")]
    public Material StartColor;
    public Material ChargeColor;
    public Material ShotingColor;

}
