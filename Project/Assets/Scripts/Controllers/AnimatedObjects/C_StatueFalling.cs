using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_StatueFalling : MonoBehaviour
{
    void CallFx()
    {
        FindObjectOfType<C_Fx>().SmokeExplosionStatue();
        FindObjectOfType<C_Fx>().DebrisStatue();
    }
}
