using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class C_VignetteOverTime : MonoBehaviour
{

    Material mat;

    bool bCanDo = true;

    private void OnTriggerEnter(Collider other)
    {
        if (bCanDo)
        {
            mat = FindObjectOfType<C_Player>().mBloodEffect;

            mat.SetColor("Color_2E964CA1", Color.cyan);

            mat.DOFloat(0.8f, "Vector1_9171129A", 4f).OnComplete(() => StartCoroutine(BackUp()));

            bCanDo = false;
        }
    }

    IEnumerator BackUp()
    {
        yield return new WaitForSeconds(3f);

        mat.DOFloat(4f, "Vector1_9171129A", 4f);

        yield return new WaitForSeconds(1f);

        ResetShader();

        yield break;
    }

    void ResetShader()
    {
        FindObjectOfType<C_Player>().ResetPlayerBlood();
        DOTween.Clear();
    }
}
