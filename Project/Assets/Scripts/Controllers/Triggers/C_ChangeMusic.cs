using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class C_ChangeMusic : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("ChangeMusic");
        GameObject.FindObjectOfType<C_Main>().ChangeMusic();
        Destroy(this.gameObject);
    }
}
