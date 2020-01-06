using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class C_Camera : MonoBehaviour
{

    CinemachineImpulseSource CineMachImpulse;

    // Start is called before the first frame update
    void Start()
    {
        CineMachImpulse = GameObject.FindObjectOfType<CinemachineImpulseSource>();
    }

    public void AddShake(float value)
    {
        CineMachImpulse.GenerateImpulse(Vector3.up * value);
    }
}
