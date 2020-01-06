using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/StepCurve")]
public class StepData : ScriptableObject
{
    public AnimationCurve StepCurve = null;
}
