using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/M_Camera")]
public class M_Camera : ScriptableObject
{
    public float RecoilMaxValue = 1.5f;
    public float RecoilRecover = 5;
    public float RecoilPow = 2;
    public float MaxFovDecal = 10;
    public float BaseFov = 60;
}
