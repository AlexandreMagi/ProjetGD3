using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WiimoteApi;

public class C_Main : MonoBehaviour
{
    Camera mainCam;

    private uint score;

    static C_Main _instance;

    C_Wiimote wiimoteController;

    bool wiimoteMode = false;

    Vector2 followingPosition;

    [SerializeField]
    float percentFollowFrame = .210f;
    [SerializeField]
    float distanceToMinFollow = 1;
    [SerializeField]
    float distanceToMaxFollow = 50;

    [SerializeField, RangeAttribute(0f, 10f)]
    float fCameraMoveWithAim = 1;

    C_WeaponMod wMod = null;

    GameObject hSoundHandlerMainMusic = null;
    float MainMusicVolume = 0;
    float MainMusicDropSpeed = 0.1f;

    bool playerCanShoot = true;
    bool playerCanOrb = false;

    void Awake()
    {
        _instance = this;
    }

    /// <summary>
    /// Prepares the game.
    /// </summary>
    void Start()
    {
        StopAllCoroutines();

        mainCam = Camera.main;

        wiimoteController = GetComponent<C_Wiimote>();

        followingPosition = Input.mousePosition;

        wMod = FindObjectOfType<C_WeaponMod>();

        hSoundHandlerMainMusic = CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "Drone_Ambiant", true, 0.5f);
        //CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "Turbine_Ambiant", true, 1f);
    }

    public void ChangeMusic()
    {
        MainMusicVolume = hSoundHandlerMainMusic.GetComponent<AudioSource>().volume;
        StartCoroutine(ChangeMusicCoroutine());
    }

    IEnumerator ChangeMusicCoroutine()
    {
        yield return new WaitForSeconds(3.5f);
        while (MainMusicVolume > 0)
        {
            MainMusicVolume -= Time.deltaTime * MainMusicDropSpeed;
            if (MainMusicVolume < 0)
                MainMusicVolume = 0;
            hSoundHandlerMainMusic.GetComponent<AudioSource>().volume = MainMusicVolume;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(3);

        CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "SE_Danger", false, .6f);
        CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "Horde_StartMoch", false, .6f);
        yield return new WaitForSeconds(0.8f);
        CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "HordeLoopMieux", true, .6f);


        yield break;
    }

    public static C_Main Instance
    {
        get {
            return _instance;
        }
    }

    /// <summary>
    /// Gets all the user input and the Wiimote inputs.
    /// </summary>
    void Update()
    {

        if (!GameObject.FindObjectOfType<scr_GameOver>() || !GameObject.FindObjectOfType<scr_GameOver>().GetGameOver())
        {
            if ((Input.GetKeyDown(KeyCode.Mouse0) || wiimoteController.isBDown) && playerCanShoot)
            {
                wMod.InputDown(GetControllerPos(), false);
            }
            if ((Input.GetKey(KeyCode.Mouse0) || wiimoteController.isB) && playerCanShoot)
            {
                wMod.InputHold(GetControllerPos(), false);
            }
            if (!Input.GetKey(KeyCode.Mouse0) && !wiimoteController.isB)
            {
                wMod.InputUp(GetControllerPos());
            }

            if ((Input.GetKeyDown(KeyCode.Mouse1) || wiimoteController.isADown) && playerCanOrb)
            {
                wMod.OnShoot(GetControllerPos(), true);
            }

            if (GameObject.FindObjectOfType<C_GatherableOrb>() && GameObject.FindObjectOfType<C_GatherableOrb>().bInTuto && (Input.GetKeyDown(KeyCode.Mouse1) || wiimoteController.isADown))
            {
                GameObject.FindObjectOfType<C_SequenceHandler>().NextSequence();
                GameObject.FindObjectOfType<C_GatherableOrb>().TutoFinished();
                Destroy(GameObject.FindObjectOfType<C_GatherableOrb>());
            }


            if (Input.mouseScrollDelta.y != 0)
            {
                //GameObject.FindObjectOfType<C_Weapon>().ChangeWeapon(1);
                wMod.ChangeWeapon();
            }
            if (Input.GetKeyDown(KeyCode.Mouse2))
            {
                wMod.InputDown(GetControllerPos(), false, true);
            }
            if (Input.GetKey(KeyCode.Mouse2))
            {
                wMod.InputHold(GetControllerPos(), false, true);
            }

            GameObject.FindObjectOfType<C_CameraRail>().DecalCurrentCamRotation(fCameraMoveWithAim, GetControllerPos());

            #region Debug
            //=============================================DEBUG
            if (Input.GetKeyDown(KeyCode.W))
            {
                wiimoteMode = !wiimoteMode;
            }
            //Skip sequence
            if (Input.GetKeyDown(KeyCode.N))
            {
                C_SequenceHandler.Instance.NextSequence();
            }
            //Change Music
            if (Input.GetKeyDown(KeyCode.M))
            {
                ChangeMusic();
            }

            //Kill player
            if (Input.GetKeyDown(KeyCode.O))
            {
                GameObject.FindObjectOfType<scr_GameOver>().GameOver();
            }
            //Hard restart
            if (Input.GetKeyDown(KeyCode.T))
            {
                GameObject.FindObjectOfType<scr_GameOver>().GameRestart(true);
            }
            //Add 1 combo
            if (Input.GetKeyDown(KeyCode.C))
            {
                C_ComboManager.Instance.AddCombo(1);
                //if (GameObject.FindObjectOfType<scr_ShakeUIMultiplier>())
                //    GameObject.FindObjectOfType<scr_ShakeUIMultiplier>().AddScore(1);
            }
            //Break combo
            if (Input.GetKeyDown(KeyCode.B))
            {
                C_ComboManager.Instance.BreakCombo(true);
            }
            //GodMode
            if (Input.GetKeyDown(KeyCode.G))
            {
                GetComponent<C_Player>().SetGod();
            }
            #endregion Debug
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                GameObject.FindObjectOfType<scr_GameOver>().GameRestart(false);
            }
        }


    }

    public void FixedUpdate()
    {
        if (wiimoteMode && WiimoteManager.HasWiimote())
        {
            Vector2 vals = wiimoteController.GetIRValues();
            float distancePoints = Vector2.Distance(followingPosition, vals);

            float newPercentFollow;

            if (distancePoints < distanceToMinFollow)
            {
                newPercentFollow = .01f;
            }
            else if (distancePoints > distanceToMaxFollow)
            {
                newPercentFollow = .5f;
            }
            else
            {
                newPercentFollow = percentFollowFrame;
            }

            float x = followingPosition.x * (1 - newPercentFollow) + vals.x * newPercentFollow;
            float y = followingPosition.y * (1 - newPercentFollow) + vals.y * newPercentFollow;

            followingPosition = new Vector2(x, y);
        }
        else
        {
            followingPosition = Input.mousePosition;
        }
    }

    /// <summary>
    /// Returns true if the object hit by the bullet must send a feedback
    /// </summary>
    /// <param name="Hit"></param>
    /// <returns></returns>
    public bool CheckIfHitedIsSomethingImportant(GameObject Hit)
    {
        if (Hit.GetComponent<C_BulletAffected>())
            return true;
        return false;
    }

    /// <summary>
    /// Choses what to do, depending on the object hit and the kind of bullet
    /// </summary>
    /// <param name="collideTo"></param>
    /// <param name="v3HitPosition"></param>
    /// <param name="bExplodeAtImpact"></param>
    /// <param name="fExplosionRange"></param>
    /// <param name="bActivateStopTimeAtImpact"></param>
    /// <param name="bulletDamage"></param>
    /// <param name="explosionForce"></param>
    /// <param name="explosionShake"></param>
    public void OnBulletHit(GameObject collideTo, Vector3 v3HitPosition, bool bExplodeAtImpact, float fExplosionRange, bool bActivateStopTimeAtImpact, int bulletDamage, float explosionForce, float explosionShake, float bulletStun, string sBulletName, bool explosionDealsDamage)
    {
        C_ComboManager comboInstance = C_ComboManager.Instance;

        if (collideTo.gameObject.layer == 12 || collideTo.gameObject.layer == 9)
        {
            GetComponent<C_Fx>().ImpactOnEnnemi(v3HitPosition);
            comboInstance.AddCombo(1);
            Invoke("HitMarkerSoundFunc", 0.05f*Time.timeScale);
        }
        else
        {
            if(collideTo.GetComponent<C_ShootTrigger>() == null && collideTo.GetComponent <C_TriggerShootDetection>() == null && collideTo.GetComponent<C_ShooterBullet>() == null)
            {
                comboInstance.BreakCombo(true);
            }
            
            GetComponent<C_Fx>().ImpactOnWalls(v3HitPosition);
        }
           

        if (bExplodeAtImpact)
        {
            GetComponent<C_Fx>().BulletExplosion(v3HitPosition, fExplosionRange);
            Collider[] hitItems = Physics.OverlapSphere(v3HitPosition, fExplosionRange);
            bool hitableAffected = false;
            if (collideTo.GetComponent<C_BulletAffected>())
            {
                collideTo.GetComponent<C_BulletAffected>().OnBulletHit(bulletDamage, bulletStun, sBulletName);
            }
            foreach (Collider hitObj in hitItems)
            {
                if (hitObj.GetComponent<C_BulletAffected>() != null)
                {
                    // TODO : 
                    // Kill entité affectée.
                    hitableAffected = true;
                    if(explosionDealsDamage) hitObj.GetComponent<C_BulletAffected>().OnBulletHit(bulletDamage, bulletStun, sBulletName);
                    hitObj.GetComponent<C_BulletAffected>().OnExplosionAffect(v3HitPosition, explosionForce, fExplosionRange, sBulletName);
                }

            }

            if (hitableAffected) GameObject.FindObjectOfType<C_Ui>().HitSomethingImportant();
                GameObject.FindObjectOfType<C_Camera>().AddShake(explosionShake);

        }
        else
        {
            // Kill mais pas en AoE
            if (collideTo.GetComponent<C_BulletAffected>() != null)
            {
                // TODO : 
                // Kill entité affectée.
                collideTo.GetComponent<C_BulletAffected>().OnBulletHit(bulletDamage, bulletStun, sBulletName);
                GameObject.FindObjectOfType<C_Ui>().HitSomethingImportant();
                collideTo.GetComponent<C_BulletAffected>().OnSoloHitPropulsion(v3HitPosition, explosionForce, sBulletName);
            }
        }
    }

    void HitMarkerSoundFunc()
    {
        CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "HitMarker_Boosted", false, 1, 0, 3f, false);
    }


    /// <summary>
    /// Gets the current x & y positions on screen of the mouse / Wiimote.
    /// </summary>
    /// <returns></returns>
    public Vector2 GetControllerPos()
    {
        /*
        if (wiimoteMode)
        {
            Vector2 vals = wiimoteController.GetIRValues();
            Debug.Log("X : " + vals.x + " -- Y : " + vals.y);
        }
        else
        {
            Debug.Log("X : " + Input.mousePosition.x + " -- Y : " + Input.mousePosition.y);
        }
        */

        //return (wiimoteMode ? wiimoteController.GetIRValues() : new Vector2(Input.mousePosition.x, Input.mousePosition.y));

        if (wiimoteMode)
        {
            return followingPosition;
        }
        else
        {
            return new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
    }

    public void SetControlState(C_TriggerSender.Activable control, bool state)
    {

        if(control == C_TriggerSender.Activable.BaseWeapon || control == C_TriggerSender.Activable.Both)
        {
            playerCanShoot = state;
            FindObjectOfType<C_Ui>().CannotShoot(state);

            if (state)
            {
                wMod.InputUp(GetControllerPos());
            }
        }

        if(control == C_TriggerSender.Activable.Orb || control == C_TriggerSender.Activable.Both)
        {
            playerCanOrb = state;
            FindObjectOfType<C_Ui>().CannotShoot(state);
        }
    }

    public void AllowPlayerOrb()
    {
        playerCanOrb = true;
    }

    public bool _playerCanOrb()
    {
        return playerCanOrb;
    }

}