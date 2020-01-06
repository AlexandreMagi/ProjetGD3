using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_RopeAction : MonoBehaviour
{

    bool bCanDestroyBox = true;

    private void Update()
    {
        if (GetComponent<MeshRenderer>().enabled == false && bCanDestroyBox)
        {
            GetComponentInChildren<Rigidbody>().isKinematic = false;
            GetComponentInChildren<Rigidbody>().AddForce(new Vector3(0, -100, 0));

            StartCoroutine(DestroyBox());

            bCanDestroyBox = false;
        }
    }

    IEnumerator DestroyBox()
    {
        yield return new WaitForSeconds(1f);

        GetComponentInChildren<C_Saw>().DisableGameObject();
        CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "CollisionSound", false, 1f);
        GameObject.FindObjectOfType<C_Camera>().AddShake(5);


        yield break;
    }
}
