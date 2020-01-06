using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_TestTriggerSequences : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        //GameObject.FindObjectOfType<C_Fx>().PlayerTakesOrbe();
        GetComponent<MeshRenderer>().enabled = false;
    }
}
