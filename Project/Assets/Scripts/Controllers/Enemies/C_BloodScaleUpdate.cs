using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_BloodScaleUpdate : MonoBehaviour
{
    Vector3 BaseScale;
    float FOVRef = 70;

    private void Start()
    {
        BaseScale = transform.localScale;
    }

    private void Update()
    {
        transform.localScale = Camera.main.fieldOfView * 2* BaseScale / FOVRef;
    }
}
