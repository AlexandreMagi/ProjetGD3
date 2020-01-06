using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Enemies/Basic")]

public class M_Enemy : ScriptableObject
{
    public uint nBaseHealth;

    public int nDamage;

    public int nResistance;


    [Header("Jauge de stun")]
    public float stunJaugeMax;
    public float stunJaugeLostPerSecond;
    public float timeStunned;
    public float timeStunAllowedAfterFirst;

}
