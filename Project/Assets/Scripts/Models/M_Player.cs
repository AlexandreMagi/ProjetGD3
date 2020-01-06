using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/M_Player")]
public class M_Player : ScriptableObject
{
    [RangeAttribute(1, 100)]
    public int maxHealth = 20;
    [RangeAttribute(1f, 100f)]
    public float ShakePerHit = 20;
    [SerializeField]
    public Material mBloodEffect = null;
    [RangeAttribute(0f, 5f)]
    public float fMaxBloodAmount = 0.5f;
    [RangeAttribute(0f, 1f)]
    public float fTimeToBloodMax = 0.1f;
    [RangeAttribute(0f, 5f)]
    public float fTimeBeforRecup = 1.5f;
    [RangeAttribute(0f, 10f)]
    public float fBloodDisabled = 4f;
}
