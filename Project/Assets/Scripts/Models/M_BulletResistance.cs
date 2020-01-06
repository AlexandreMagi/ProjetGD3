using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/M_BulletResistance")]
public class M_BulletResistance : ScriptableObject
{
    public M_Bullet BulletPreset;
    public float DammageMultiplier = 1;
    public float RecoilMultiplier = 1;
    public float StunMultiplier = 1;

}
