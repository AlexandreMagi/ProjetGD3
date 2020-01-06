using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class C_Player : MonoBehaviour
{
    public M_Player playerStats = null;

    int health;
    public Material mBloodEffect;
    float fMaxBloodAmount;
    float fTimeToBloodMax;
    float fTimeBeforRecup;
    float fBloodDisabled;

    bool godMode = false;

    GameObject hMainCam = null;

    // Start is called before the first frame update
    void Start()
    {
        health = playerStats.maxHealth;

        mBloodEffect = playerStats.mBloodEffect;
        fTimeToBloodMax = playerStats.fTimeToBloodMax;
        fTimeBeforRecup = playerStats.fTimeBeforRecup;
        fMaxBloodAmount = playerStats.fMaxBloodAmount;
        fBloodDisabled = playerStats.fBloodDisabled;

        hMainCam = Camera.main.gameObject;

        ResetPlayerBlood();
    }

    /// <summary>
    /// Makes the player lose a certain quantity of health. If health reaches 0, the player dies.
    /// </summary>
    /// <param name="damageTaken"></param>
    public void TakeDamage(int damageTaken)
    {
        if(!godMode) health -= damageTaken;
        GameObject.FindObjectOfType<C_Fx>().PlayerTakesDamages(new Vector3(hMainCam.transform.localPosition.x, hMainCam.transform.localPosition.y, hMainCam.transform.localPosition.z), hMainCam.transform.localRotation);
        GetComponent<C_Camera>().AddShake(playerStats.ShakePerHit);

        FindObjectOfType<C_Fx>().LoseLife();

        mBloodEffect.DOFloat(fMaxBloodAmount, "Vector1_9171129A", fTimeToBloodMax);

        Invoke("Recup", fTimeBeforRecup);
        CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "PlayerDamage", false, 1,0.2f);

        if (health <= 0)
        {
            //Debug.LogWarning("PENSER À REACTIVER LA MORT");
            LoseLife();
        }
    }

    void Recup()
    {
        mBloodEffect.DOFloat(fBloodDisabled, "Vector1_9171129A", fTimeToBloodMax);
    }

    public void ResetPlayerBlood()
    {
        DOTween.Clear();
        mBloodEffect.SetColor("Color_2E964CA1", Color.red);
        mBloodEffect.SetFloat("Vector1_9171129A", fBloodDisabled);
    }

    public Vector2 GetLife()
    {
        return new Vector2(health, playerStats.maxHealth);
    }

    /// <summary>
    /// NIY : Shows the GameOver screen when the player dies, or makes it lose a life.
    /// </summary>
    void LoseLife()
    {
        GameObject.FindObjectOfType<scr_GameOver>().GameOver();
        //TODO : La fin si mort
    }

    public void SetGod()
    {
        godMode = !godMode;
    }
}
    