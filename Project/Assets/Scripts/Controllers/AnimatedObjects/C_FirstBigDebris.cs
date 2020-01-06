using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_FirstBigDebris : MonoBehaviour
{
    void CallFx()
    {
        FindObjectOfType<C_Fx>().SmokeExplosion();
        FindObjectOfType<C_Fx>().DebrisCeilling();
    }
}
