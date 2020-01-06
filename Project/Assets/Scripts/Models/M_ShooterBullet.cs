using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Enemies/M_ShooterBullet")]
public class M_ShooterBullet : ScriptableObject
{
    public AnimationCurve BulletTrajectory;
    public AnimationCurve BulletRotation;
    public AnimationCurve CircleScale;

    public float BulletSpeed = 5;
    public float RandomSpeedAdded = 2;
    public int BulletDammage = 10;
    public float ShakeAtImpact = 10;
    public float RotationSpeed = 90;
    public float CircleScaleMultiplier = 1;
    public bool bRandomRotationAtStart = false;

    public float ExplosionRange = 5;

    public float ForceAppliedOnImpact = 500;
    public float StunValue = 1.2f;
    public string BulletName = "ShooterBullet";

    public float TimeBeforeCollisionAreActived = 1;

    public Vector3 RandomFrom = -Vector3.one;
    public Vector3 RandomTo = Vector3.one;

}
