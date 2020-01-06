using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/M_Saw")]
public class M_Saw : ScriptableObject
{
    public bool isSpinning = false;

    public float spinSpeed = 0;

    [ShowWhen("isSpinning")]
    public bool spinX = false;
    [ShowWhen("isSpinning")]
    public bool spinY = false;
    [ShowWhen("isSpinning")]
    public bool spinZ = false;
}
