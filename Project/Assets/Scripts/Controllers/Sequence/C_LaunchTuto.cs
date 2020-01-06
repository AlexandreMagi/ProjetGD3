using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_LaunchTuto : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        GameObject.FindObjectOfType<MainFuncTest>().bActivation = true;
        GameObject.FindObjectOfType<MainFuncTest>().ChangeText("HOLD TO CHARGE YOUR SHOT");
    }
}
