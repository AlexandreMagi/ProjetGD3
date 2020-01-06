using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Saw : MonoBehaviour
{
    [SerializeField]
    M_Saw sawItem = null;

    [SerializeField]
    bool countsAsPlayerKill = false;

    void Update()
    {
        if (sawItem.isSpinning)
        {
            Quaternion rot = transform.rotation;
            transform.Rotate(
                (sawItem.spinX ? (sawItem.spinSpeed * Time.deltaTime) : 0),
                (sawItem.spinY ? (sawItem.spinSpeed * Time.deltaTime) : 0),
                (sawItem.spinZ ? (sawItem.spinSpeed * Time.deltaTime) : 0)
            );
        }
    }

    void OnTriggerEnter(Collider other)
    {
        C_Enemy otherEnemy = other.GetComponent<C_Enemy>();
        if (other.GetComponent<C_Enemy>() != null)
        {
            otherEnemy.Die(false, countsAsPlayerKill);
        }
    }

    public void DisableGameObject()
    {
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;
       
        GetComponent<Rigidbody>().isKinematic = true;

        FindObjectOfType<C_Fx>().BoxDestruction(new Vector3(transform.position.x, transform.position.y - 0.3f, transform.position.z));

        StartCoroutine(DisableSaw());
    }

    IEnumerator DisableSaw()
    {
        yield return new WaitForSeconds(0.8f);
        GetComponent<C_Saw>().enabled = false;
        yield break;
    }

}