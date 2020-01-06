using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_OrbZoneActivation : MonoBehaviour
{

    bool bIsActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!bIsActivated)
        {
            GetComponent<C_GravityOrb>().SpawnViaScene(); 
            bIsActivated = true;
        }
        else if (bIsActivated)
        {
           // GetComponent<C_GravityOrb>().StopHolding();
        }
    }
}
