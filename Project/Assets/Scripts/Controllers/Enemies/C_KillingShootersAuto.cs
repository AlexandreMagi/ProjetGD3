using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_KillingShootersAuto : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(KillEntity());
    }

    IEnumerator KillEntity()
    {
        yield return new WaitForSeconds(15f);

        Destroy(this.gameObject);

        yield break;
    }


}
