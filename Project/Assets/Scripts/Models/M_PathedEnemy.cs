﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Enemies/Swarmer")]
public class M_PathedEnemy : M_Enemy
{
    public float nVarianceInPath;

    public float tTimeBeforeNextPath;

    public float fDistanceBeforeNextPath;

    public float speed;

    public float upScale;

    public float distanceToTargetPlayer;

    public float fDistanceMelee;

    public float fWaitDuration = 0.3f;
    public float fDistanceBeforeAttack = 6f;
    public float fJumpForce = 80f;
    public float fSpeedMultiplierWhenAttacking = 4;

}
