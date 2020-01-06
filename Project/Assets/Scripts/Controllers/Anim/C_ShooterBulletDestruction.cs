using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_ShooterBulletDestruction : MonoBehaviour
{
    [SerializeField]
    GameObject debris = null;

    public void OnBulletAnimEnd()
    {
        debris.SetActive(true);
        FindObjectOfType<C_Fx>().ShooterShootDebris(transform.position);

        StartCoroutine(DisableAll());
    }

    IEnumerator DisableAll()
    {

        yield return new WaitForSeconds(1f);

        gameObject.transform.parent.position = new Vector3(0, 0, 0);

        yield break;
    }
}
