using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_GravOrbReady : MonoBehaviour
{

    public ParticleSystem Fx = null;
    public GameObject Text = null;

    float CurrentScale = 0;
    float ScaleSpeed = 0;

    public void PlayFeedback()
    {
        StartCoroutine(FxCoroutine());
    }

    IEnumerator FxCoroutine()
    {
        Fx.Play();
        ScaleSpeed = 5;

        yield return new WaitForSeconds(2);

        ScaleSpeed = -5;

        yield break;
    }

    private void Update()
    {
        if (CurrentScale < 1 && ScaleSpeed > 0)
        {
            CurrentScale += Time.deltaTime * ScaleSpeed;
            if (CurrentScale >= 1)
            {
                CurrentScale = 1;
                //End
            }
        }
        else if (CurrentScale > 0 && ScaleSpeed < 0)
        {
            CurrentScale += Time.deltaTime * ScaleSpeed;
            if (CurrentScale <= 0)
            {
                CurrentScale = 0;
                //End
            }
        }
        Text.transform.localScale = Vector3.one * CurrentScale;
    }

}
