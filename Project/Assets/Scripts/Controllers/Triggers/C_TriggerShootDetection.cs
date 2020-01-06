using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_TriggerShootDetection : MonoBehaviour
{

    bool bCanStartCoroutine = true;
    [SerializeField]
    string sSoundPlayed = "";
    [SerializeField]
    float fSoundVolume = 1;
    [SerializeField]
    float fDelay = 0;

    bool bSoundPlayed = false;

    public void OnAnimDetection()
    {
        Destroy(GetComponent<Animator>());

        Rigidbody rb = GetComponent<Rigidbody>();

        rb.isKinematic = false;

        rb.AddForce(new Vector3(200, 0, 0));

        if (!bSoundPlayed)
        {
            bSoundPlayed = true;
            Invoke("PlaySound", fDelay);
        }

        if (bCanStartCoroutine)
            StartCoroutine(TimerBeforeNextSequence()); bCanStartCoroutine = false;
        
    }

    public void OnAnimDetectionBase()
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null)
            anim.SetTrigger("MakeAction");
    }


    void PlaySound()
    {
        if (sSoundPlayed != "")
        {
            CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, sSoundPlayed, false, fSoundVolume);
        }
    }

    IEnumerator TimerBeforeNextSequence()
    {
        yield return new WaitForSeconds(0.5f);

        FindObjectOfType<C_SequenceHandler>().NextSequence();

        yield break;
    }
}
