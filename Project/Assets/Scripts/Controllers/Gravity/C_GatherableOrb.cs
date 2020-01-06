using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_GatherableOrb : MonoBehaviour
{

    bool bPlayerCanDammage = true;
    float fCurrentScale = 1;
    float fScaleBoostBeforeExplosion = .5f;

    float DammageDone = 0;
    float DammageDoneSaved = 0;
    float DammageBeforeExplosion = 20;

    bool bItemDestroyed = false;
    bool bItemDestroyedCompletly = false;

    [SerializeField]
    GameObject GravityOrb = null;

    // Update is called once per frame
    void Update()
    {
        if (DammageDone < DammageBeforeExplosion)
        {
            fCurrentScale = Mathf.Lerp(fCurrentScale, 1 + DammageDone * fScaleBoostBeforeExplosion / DammageBeforeExplosion, Time.deltaTime * 5);
            transform.localScale = Vector3.one * fCurrentScale;
        }
        else if (!bItemDestroyed)
        {
            bPlayerCanDammage = false;
            GameObject.FindObjectOfType<C_Fx>().OrbGatherableExplosionFinal(transform.position + Vector3.up * 0.9542458f * fCurrentScale);
            Invoke("OrbPreDestroyed", 2.6f);
            Invoke("OrbDestroyed", 2.8f);
            Invoke("GoToNextSequence", 6f);
            bItemDestroyed = true;
            CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "GravityOrbOvercharge_Boosted", false, 1f);
        }
        if (bItemDestroyed && !bItemDestroyedCompletly)
        {
            GameObject.FindObjectOfType<C_Camera>().AddShake(30f*Time.deltaTime);
        }
    }

    void OrbPreDestroyed()
    {
        transform.localScale = Vector3.zero;
    }

    void OrbDestroyed()
    {
        GameObject.FindObjectOfType<C_Camera>().AddShake(50);
        bItemDestroyedCompletly = true;
        GravityOrb.GetComponent<C_GravityOrb>().StopHolding();
        GameObject.FindObjectOfType<C_Fx>().GatherOrb(transform.position + Vector3.up * 0.9542458f * fCurrentScale);
        CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "EquipOrb_Boosted", false, 1f);
    }

    void GoToNextSequence()
    {
        GameObject.FindObjectOfType<C_SequenceHandler>().NextSequence();
        GameObject.FindObjectOfType<C_GravOrbReady>().PlayFeedback();
        GameObject.FindObjectOfType<MainFuncTest>().bActivation = true;
        GameObject.FindObjectOfType<MainFuncTest>().ChangeText("PRESS A TO ACTIVATE GRAVITY ORB");
    }

    public void PlayerShootOnObjet(float Dmg)
    {
        if (bPlayerCanDammage)
        {
            DammageDone += Dmg / 35;
            for (int i = Mathf.CeilToInt(DammageDoneSaved); i < DammageDone; i++)
            {
                GameObject.FindObjectOfType<C_Fx>().OrbGatherableExplosion(transform.position + Vector3.up * 0.9542458f * fCurrentScale);
                GameObject.FindObjectOfType<C_Camera>().AddShake(i*0.8f);
                CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "ImpactOrbeSequence_Boosted", false, 1f);
            }
            DammageDoneSaved = DammageDone;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        bPlayerCanDammage = true;
    }
}
