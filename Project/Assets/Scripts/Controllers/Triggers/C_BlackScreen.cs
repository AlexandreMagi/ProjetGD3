using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class C_BlackScreen : MonoBehaviour
{
    [SerializeField]
    GameObject hFonduNoir = null;
    [SerializeField]
    GameObject hTextEnd = null;

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(EndCoroutine());
    }

    IEnumerator EndCoroutine()
    {
        CustomSoundManager.Instance.StopAllSound();
        CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "Glass_Break", false, 1);
        hFonduNoir.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        hTextEnd.SetActive(true);
        yield break;
    }
}
