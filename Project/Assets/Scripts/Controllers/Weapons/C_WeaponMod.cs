using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_WeaponMod : MonoBehaviour
{
    [Header ("Prefabs")]
    [HideInInspector]
    public GameObject bulletPrefab = null;
    [HideInInspector]
    public GameObject gravityOrbPrefab = null;

    //[Header("Presets")]
    [HideInInspector]
    public M_WeaponMod playerStats = null;
    //[SerializeField, Range (0,1)]
    int nWeaponIndex = 0;

    [Header("Mods")]
    [SerializeField]
    bool bAllowTwoModAtTheSameTime = false;
    [SerializeField]
    bool bOneBulletEvery = false;
    [SerializeField]
    bool bFirstBulletEvery = false;
    [SerializeField, Range(0.2f, 5f)]
    float fStrongerBulletEvery = 2;
    float fStrongBulletTImer = 0;
    [SerializeField]
    bool bIndependantFromTimeScale = true;
    [SerializeField]
    bool bFirstBulletStronger = false;
    [SerializeField]
    bool bSwitchWeaponManually = false;
    [SerializeField]
    bool bSwitchWeaponWhenAttraction = false;
    [SerializeField]
    bool bSwitchWeaponWhenZeroG = false;


    [SerializeField]
    bool bSwitchWithCharge = false;
    float fCurrentCharge = 0;
    float fCurrentChargeSpeed = 0;
    [SerializeField]
    float[] fSwitchChargeShoot = new float[0];
    [SerializeField]
    int[] nSwitchWeaponIndex = new int[0];

    [HideInInspector]
    public string sSondShoot = "";

    [SerializeField, ShowWhen("bIsAbsorbSpecial")]
    LayerMask layersHitByAbsorb = 0;
    [SerializeField, ShowWhen("bIsAbsorbSpecial")]
    LayerMask layersForMetal = 0;
    [SerializeField, ShowWhen("bIsAbsorbSpecial")]
    LayerMask layersForWood = 0;
    [SerializeField, ShowWhen("bIsAbsorbSpecial")]
    LayerMask layersForGlass = 0;

    C_GravityOrb activatedOrb;

    private float fEtimeFireRate = 0;
    private float fCurrentImprecision = 0;

    private float fOrbTimer = 0;

    bool isGravityCooldownUp = false;

    GameObject MainCam = null;
    /// <summary>
    /// Basic setup. Sets the imprecision, fire rate and charge speed
    /// </summary>
    void Start()
    {
        if (playerStats)
        {
            playerStats.fMaxImprecision = playerStats.fMaxImprecision < playerStats.fBaseImprecision ? playerStats.fMaxImprecision = playerStats.fBaseImprecision : playerStats.fMaxImprecision;
       
        
        fCurrentImprecision = playerStats.fBaseImprecision;
        fEtimeFireRate = 1;
        fCurrentChargeSpeed = playerStats.ChargeSpeed;
        }

        MainCam = Camera.main.gameObject;
        fOrbTimer = playerStats.gravityCooldown;
    }

    /// <summary>
    /// Update checks if the shot is possible or not
    /// </summary>
    void Update()
    {

        if (playerStats)
        {   // --- Calcul de timer pour la firerate
            if (fEtimeFireRate < 1)
            {
                if (playerStats.bFireRateIndependantFromTimeScale)
                    fEtimeFireRate += playerStats.fFireRate * Time.deltaTime / Time.timeScale;
                else
                    fEtimeFireRate += playerStats.fFireRate * Time.deltaTime;
            }

            if (GameObject.FindObjectOfType<C_Main>()._playerCanOrb())
            {
                if (fOrbTimer < playerStats.gravityCooldown)
                {
                    fOrbTimer += Time.deltaTime / Time.timeScale;
                }
                else if (!isGravityCooldownUp)
                {
                    isGravityCooldownUp = true;
                    GameObject.FindObjectOfType<C_Fx>().OrbAvailable();
                }
            }


        }

        if (fStrongBulletTImer <= fStrongerBulletEvery)
        {
            if (bIndependantFromTimeScale)
                fStrongBulletTImer += Time.deltaTime / Time.timeScale;
            else
                fStrongBulletTImer += Time.deltaTime;
        }
        else
        {
            if (bOneBulletEvery && nWeaponIndex == 0)
                ChangeWeapon(true, 1);
        }

       

    }

    public float GetChargeValue()
    {
        float ValueSafe = 0.25f;
        float Chargevalue = 0;
        if (fCurrentCharge > ValueSafe)
            Chargevalue = (fCurrentCharge - ValueSafe) / (1 - ValueSafe);
        return Chargevalue;
    }

    /// <summary>
    /// Starts the coroutine that manages the weapon switch by looking at the GravityOrb
    /// </summary>
    /// <param name="hOrb"></param>
    public void InitCoroutineChangeWeaponViaORb(M_GravityOrb hOrb)
    {
        StartCoroutine(CoroutineChangeWeaponViaORb(hOrb));
    }

    /// <summary>
    /// Coroutine that checks if the orb is active or not, and changes the weapon depending on this
    /// </summary>
    /// <param name="hOrb"></param>
    /// <returns></returns>
    IEnumerator CoroutineChangeWeaponViaORb(M_GravityOrb hOrb)
    {
        if (bSwitchWeaponWhenAttraction)
            ChangeWeapon(true, 1);
        Debug.LogWarning("Penser à résoudre le bug de timing de la gravity orb");
        yield return new WaitForSecondsRealtime((hOrb.fTimeBeforeHold + hOrb.fLockTime) * 2.2f);
        if (bSwitchWeaponWhenAttraction)
            ChangeWeapon(true, 0);
        if (bSwitchWeaponWhenZeroG)
            ChangeWeapon(true, 1);
        yield return new WaitForSecondsRealtime(hOrb.tFloatTime);
        if (bSwitchWeaponWhenZeroG)
            ChangeWeapon(true, 0);
        yield break;
    }

    /// <summary>
    /// Returns the current weapon index.
    /// </summary>
    /// <returns>
    /// Weapon index
    /// </returns>
    public int GetWeaponIndex()
    {
        return nWeaponIndex;
    }
    public bool GetOrbReady()
    {
        return isGravityCooldownUp;
    }

    /// <summary>
    /// Switches the current player's weapon
    /// </summary>
    /// <param name="bPriority"></param>
    /// <param name="precisedIndex"></param>
    public void ChangeWeapon(bool bPriority = false, int precisedIndex = -1)
    {
        C_Weapon mod = GameObject.FindObjectOfType<C_Weapon>();

        if (bPriority || !bPriority && bSwitchWeaponManually)
        {
            if (precisedIndex != -1)
            {
                nWeaponIndex = precisedIndex;
            }
            else
            {
                nWeaponIndex++;
            }

            if (nWeaponIndex >= (mod.GetNbWeapons() - 1))
            {
                nWeaponIndex = 0;
            }
               
            mod.ChangeWeapon(1, precisedIndex);
            //GameObject.FindObjectOfType<C_Ui>().ChangePreset(precisedIndex);
        }
    }
    /// <summary>
    /// Returns the current value of the UI for the strong bullet.
    /// </summary>
    /// <returns>UI - Strong bullet</returns>
    public float GetSliderStrongerBullet()
    {
        float Value = fStrongBulletTImer > fStrongerBulletEvery ? 1 : fStrongBulletTImer / fStrongerBulletEvery;
        Value = bSwitchWithCharge ? fCurrentCharge : Value;
        if (!bFirstBulletEvery && !bOneBulletEvery && !bSwitchWithCharge)
            Value = 666;
        return Value;
    }

    /// <summary>
    /// Returns the current value of the UI for the gravity orb.
    /// </summary>
    /// <returns>UI - Gravity Orb</returns>
    public float GetSliderOrb()
    {
        float Value = fOrbTimer > playerStats.gravityCooldown ? 1 : fOrbTimer / playerStats.gravityCooldown;
        return Value;
    }

    /// <summary>
    /// Called by the main when the input is first downed.
    /// </summary>
    /// <param name="mousePosition"></param>
    /// <param name="isGravityShot"></param>
    /// <param name="bSecondaryShoot"></param>
    public void InputDown(Vector2 mousePosition, bool isGravityShot, bool bSecondaryShoot = false)
    {
        if (bSecondaryShoot)
        {
            int nIndex = nWeaponIndex + 1;
            if (nIndex > 2)
                nIndex = 0;
            OnSecondaryShoot(mousePosition, nIndex);
        }
        else if (!bSwitchWithCharge)
        {
            if (bFirstBulletStronger)
                ChangeWeapon(true, 1);
            if (OnShoot(mousePosition, isGravityShot))
            {
                if (fStrongBulletTImer > fStrongerBulletEvery && bFirstBulletEvery && nWeaponIndex == 1)
                {
                    fStrongBulletTImer -= fStrongerBulletEvery;
                    ChangeWeapon(true, 0);
                }
                if (fStrongBulletTImer > fStrongerBulletEvery && bOneBulletEvery && nWeaponIndex == 1)
                {
                    fStrongBulletTImer -= fStrongerBulletEvery;
                    ChangeWeapon(true, 0);
                }
            }
            if (bFirstBulletStronger)
                ChangeWeapon(true, 0);
        }
    }

    /// <summary>
    /// Called by the main each frame the input is held.
    /// </summary>
    /// <param name="mousePosition"></param>
    /// <param name="isGravityShot"></param>
    /// <param name="bSecondaryShoot"></param>
    public void InputHold(Vector2 mousePosition, bool isGravityShot, bool bSecondaryShoot = false)
    {
        if (bSecondaryShoot)
        {
            int nIndex = nWeaponIndex + 1;
            if (nIndex > 2)
                nIndex = 0;
            if (!playerStats.bOneShotAtATime)
            {
                OnSecondaryShoot(mousePosition, nIndex);
            }
        }
        else
        {
            if (bSwitchWithCharge)
            {
                if (fCurrentCharge < 1)
                {
                    fCurrentCharge += Time.deltaTime/Time.timeScale * fCurrentChargeSpeed;
                    //Debug.LogWarning("ICI ON CHARGE (penser à ajouter speed) = " + fCurrentCharge);
                }
                else if (fCurrentCharge > 1)
                {
                    CustomSoundManager.Instance.PlaySound(MainCam.gameObject, "Charged_Shotgun", false, 0.5f, 0.1f);
                    fCurrentCharge = 1;
                    //GameObject.FindObjectOfType<C_Fx>().ReenableShoot();
                    GameObject.FindObjectOfType<C_Fx>().ChargedShotFunc();
                    //Debug.LogWarning("CHARGE FINI");
                }
            }
            else if (!playerStats.bOneShotAtATime)
            {
                if (OnShoot(mousePosition, isGravityShot))
                {
                    if (fStrongBulletTImer > fStrongerBulletEvery && bOneBulletEvery && nWeaponIndex == 1)
                    {
                        fStrongBulletTImer -= fStrongerBulletEvery;
                        ChangeWeapon(true, 0);
                    }
                }
                ReduceImprecision(true);
            }
        }
    }
    
    /// <summary>
    /// Called by the main when the input is first released.
    /// </summary>
    /// <param name="mousePosition"></param>
    public void InputUp(Vector2 mousePosition)
    {
        if (fCurrentCharge > 0 && bSwitchWithCharge)
        {
            int nIndexCurr = 0;
            for (int i = 0; i < fSwitchChargeShoot.Length; i++)
            {
                if (fCurrentCharge > fSwitchChargeShoot[i])
                    nIndexCurr = i;
            }
            if (fCurrentCharge == 1)
            {
                GameObject.FindObjectOfType<C_Fx>().ChargedShotReleasedFunc();
            }
            ChangeWeapon(true, nSwitchWeaponIndex[nIndexCurr]);
            //Debug.LogWarning("ICI ON TIRE");
            OnShoot(mousePosition, false);
            CustomSoundManager.Instance.RemoveSound("Charged_Shotgun");
            nIndexCurr = 0;
            ChangeWeapon(true, nSwitchWeaponIndex[nIndexCurr]);
            fCurrentCharge = 0;
        }

        if (fStrongBulletTImer > fStrongerBulletEvery && bFirstBulletEvery && nWeaponIndex == 0)
        {
            ChangeWeapon(true, 1);
        }
        ReduceImprecision();
    }

    /// <summary>
    /// Reduces the current imprecision depending on the weapon stats.
    /// </summary>
    /// <param name="bShooting"></param>
    void ReduceImprecision (bool bShooting = false)
    {
        // --- Calcul pour l'imprecision
        for (int i = 0; i < fCurrentImprecision; i++)
        {
            if (!bShooting || i != nWeaponIndex)
            {
                if (fCurrentImprecision > playerStats.fBaseImprecision)
                {
                    fCurrentImprecision -= playerStats.fImprecisionGainPerShot * Time.deltaTime;
                }
                else
                {
                    fCurrentImprecision = playerStats.fBaseImprecision;
                }
            }
        }
    }

    /// <summary>
    /// Shoots a bullet or uses the special action, depending on the booleans checked earlier.
    /// </summary>
    /// <param name="mousePosition"></param>
    /// <param name="specialShot"></param>
    /// <returns></returns>
    public bool OnShoot(Vector2 mousePosition, bool specialShot)
    {
        // ######################################## ORB GRAV ######################################## //
        if (specialShot)
        {  
            if (isGravityCooldownUp)
            {
                C_GravityOrb hOrb;
                isGravityCooldownUp = false;
                fOrbTimer = 0;
                hOrb = Instantiate(gravityOrbPrefab).GetComponent<C_GravityOrb>();
                activatedOrb = hOrb.GetComponent<C_GravityOrb>();
                hOrb.OnSpawning(mousePosition, nWeaponIndex);
                GameObject.FindObjectOfType<C_CameraRail>().AddRecoil(playerStats.RecoilPerGravityBullet);
            }
            else
            {
                if (playerStats.bTriggerGrav && activatedOrb)
                {
                    activatedOrb.StopHolding();
                }
                else
                {
                    CustomSoundManager.Instance.PlaySound(MainCam.gameObject, "NoAmmoEnergetic", false, 0.3f,0.2f);
                }
            }
        }
        // ######################################## TIR NORMAL ######################################## //
        else if (fEtimeFireRate >= 1)
        {
            fEtimeFireRate -= 1;
            GameObject.FindObjectOfType<C_Camera>().AddShake(playerStats.ShakePerShot);
            GameObject.FindObjectOfType<C_CameraRail>().AddRecoil(playerStats.RecoilPerShot);

            if (fCurrentImprecision < playerStats.fBaseImprecision)
                fCurrentImprecision = playerStats.fBaseImprecision;
            for (int i = 0; i < playerStats.nBulletPerShoot; i++)
            {
                GameObject bullet = Instantiate(bulletPrefab);
                bullet.GetComponent<C_Bullet>().OnCreation(transform.position, mousePosition, fCurrentImprecision, nWeaponIndex);
            }
            fCurrentImprecision += playerStats.fImprecisionGainPerShot;
            if (fCurrentImprecision > playerStats.fMaxImprecision)
                fCurrentImprecision = playerStats.fMaxImprecision;
            GameObject.FindObjectOfType<C_Ui>().Shoot();
            CustomSoundManager.Instance.PlaySound(MainCam.gameObject, sSondShoot, false, 0.5f, 0.2f);
            return true;
        }
        return false;
 
    }

    /// <summary>
    /// Shoots the gravity orb
    /// </summary>
    /// <param name="mousePosition"></param>
    /// <param name="nIndex"></param>
    public void OnSecondaryShoot(Vector2 mousePosition, int nIndex)
    {
        if (bAllowTwoModAtTheSameTime)
        {
            // ######################################## TIR NORMAL ######################################## //
            if (fEtimeFireRate >= 1)
            {
                fEtimeFireRate -= 1;
                GameObject.FindObjectOfType<C_Camera>().AddShake(playerStats.ShakePerShot);
                for (int i = 0; i < playerStats.nBulletPerShoot; i++)
                {
                    GameObject bullet = Instantiate(bulletPrefab);
                    bullet.GetComponent<C_Bullet>().OnCreation(transform.position, mousePosition, fCurrentImprecision, nIndex);
                }
                fCurrentImprecision += playerStats.fImprecisionGainPerShot;
                if (fCurrentImprecision > playerStats.fMaxImprecision)
                    fCurrentImprecision = playerStats.fMaxImprecision;
                GameObject.FindObjectOfType<C_Ui>().Shoot();
                CustomSoundManager.Instance.PlaySound(MainCam.gameObject, "Sound_Shot", false, 0.5f, 0.5f);
            }
        }
    }

}
