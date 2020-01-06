using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_ShootTrigger : MonoBehaviour
{
    C_ShootTriggerManager parentManager = null;

    bool isTriggered = false;

    [SerializeField]
    bool keepsCombo = true;

    [SerializeField]
    string sSoundPlayed = "";
    [SerializeField]
    float fSoundVolume = 1;

    void Start()
    {
        parentManager = this.transform.GetComponentInParent<C_ShootTriggerManager>();
    }

    public void OnBulletTrigger()
    {
        if (!isTriggered)
        {
            isTriggered = true;
            if (keepsCombo) C_ComboManager.Instance.MaintainCombo();

            if (parentManager != null)
                parentManager.OnEventSent();

            GetComponent<MeshRenderer>().enabled = false;

            if (gameObject.transform.tag == "Collectibles")
            {
                FindObjectOfType<C_Fx>().Collectibles(transform.position);
                CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "SE_Bonus", false, 0.7f);
            }
            else if (gameObject.transform.tag == "EnvironnementTrigger")
            {
                FindObjectOfType<C_Fx>().TriggerShoot(transform.position);
            }
            else
            {
                FindObjectOfType<C_Fx>().TriggerShoot(transform.position);
                CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, sSoundPlayed, false, fSoundVolume);
            }


            Destroy(this);
        }

    }

}
