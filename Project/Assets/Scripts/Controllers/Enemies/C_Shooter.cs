using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Shooter : C_Enemy
{

    [SerializeField] M_ShooterBullet bullet = null;
    [SerializeField] GameObject BulletPrefabs = null;
    [SerializeField] GameObject CanonPlacement = null;
    M_Shooter Stats = null;
    [SerializeField] Transform FxPos = null;

    /// <summary>
    /// Différents états de l'ennemi
    /// </summary>
    enum State { Nothing, Rotating, Loading, Shooting, Stuned, Recovering};

    [Tooltip("Variable qui indique l'étât actuel de l'ennemi")]
    int nState = 0;

    [Tooltip("Timer qui indique le temps passé à se préparer à charger")]
    float fETimerLoading = 0;
    [Tooltip("Timer qui indique le temps passé stun après avoir impacté le joueur")]
    float fETimerRecovering = 0;
    [Tooltip("Timer qui indique le avant le prochain tir dans la salve")]
    float fETimerbeforeNextAttack = 0;
    [Tooltip("Indique à quel tir on est dans la salve")]
    int nBulletShot = 0;

    [HideInInspector]
    public bool bPlayerMoving = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        nState = (int)State.Nothing;
        Stats = enemy as M_Shooter;
        SpotPlayer();
        if (bisStuned)
            nState = (int)State.Stuned;

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        
        if (bPlayerMoving && !Stats.shootEvenIfPlayerMoving)
        {
            if (!(nState == (int)State.Stuned) && !(nState == (int)State.Recovering))
            {
                ResetTimers();
                SpotPlayer();
            }
        }
        switch (nState)
        {
            case (int)State.Nothing:
                break;
            case (int)State.Rotating:
                Vector3 vPos = new Vector3(player.position.x, this.transform.position.y, player.position.z);
                Quaternion targetRotation = Quaternion.LookRotation(transform.position - vPos, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * Stats.rotationSpeed);
                if (Quaternion.Angle(transform.rotation, targetRotation) < Stats.rotationMinimalBeforeCharge && (player.position.y - transform.position.y) < 2)
                {
                    PlayerLocked();
                    StartLoading();
                }
                break;
            case (int)State.Loading:
                fETimerLoading += Time.deltaTime;
                if (fETimerLoading > Stats.timeWaitBeforeShoot)
                {
                    EndLoading(true);
                }
                break;
            case (int)State.Shooting:
                fETimerbeforeNextAttack += Time.deltaTime;
                if (fETimerbeforeNextAttack > Stats.timeBulletBetweenSalve)
                {
                    fETimerbeforeNextAttack -= Stats.timeBulletBetweenSalve;
                    nBulletShot++;
                    Shoot();
                    GetComponent<Animator>().SetTrigger("Shoot");
                    CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "SE_Shooter_Launch", false, 0.5f);
                    if (nBulletShot >= Stats.nbShootPerSalve)
                    {
                        // RESET DES VALEURS ET STUN
                        StartRecover();
                    }
                }
                break;
            case (int)State.Stuned:
                break;
            case (int)State.Recovering:
                fETimerRecovering += Time.deltaTime;
                if (fETimerRecovering > Stats.recoverTime)
                {
                    StopRecover();
                }
                break;
            default:
                Debug.LogWarning("Incorrect State on " + this.name + ". Please check the code. Current State = " + nState);
                break;
        }

    }

    /// <summary>
    /// Commencement de la rotation du shooter vers le joueur
    /// </summary>
    void SpotPlayer()
    {
        nState = (int)State.Rotating;
        ChangeColor(Stats.StartColor);
        CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "SE_Shooter_Spot", false, 0.6f);
    }

    /// <summary>
    /// Le chargeur a fini de s'orienter vers le joueur
    /// </summary>
    void PlayerLocked()
    {
        Vector3 vPos = new Vector3(player.position.x, this.transform.position.y, player.position.z);
        transform.rotation = Quaternion.LookRotation(transform.position - vPos, Vector3.up);
    }

    /// <summary>
    /// Le chargeur se prépare à charger
    /// </summary>
    void StartLoading()
    {
        nState = (int)State.Loading;
        CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "SE_Shooter_Bip_Rockets", false, 1);
        ChangeColor(Stats.ChargeColor);
    }

    /// <summary>
    /// Le chargeur arrete sa préparation (si bool true, ça lance la charge)
    /// </summary>
    /// <param name="bShoot"></param>
    void EndLoading(bool bShoot)
    {
        ResetTimers();
        if (bShoot)
            StartShoot();
    }

    /// <summary>
    /// Le tireur commence à tirer
    /// </summary>
    void StartShoot()
    {
        nState = (int)State.Shooting;
        ChangeColor(Stats.ShotingColor);
    }

    /// <summary>
    /// Le tireur arrête de tirer
    /// </summary>
    void StopShoot(bool bIsStun)
    {
        ResetTimers();
        if (bIsStun)
        {
            IsStun();
        }
        else
        {
            SpotPlayer();
        }
    }

    /// <summary>
    /// Déclenche le stun
    /// </summary>
    protected override void IsStun()
    {
        if (!bisStuned)
        {
            GetComponent<Animator>().SetTrigger("StartStun");
            GameObject.FindObjectOfType<C_Fx>().StunFx(FxPos, enemy.timeStunned);
        }
        base.IsStun();
        EndLoading(false);
        /*StopRecover(false);
        StopCharge(false);
        StopRepositioning(false);*/
        GetComponent<Rigidbody>().AddForce(transform.forward * Stats.StunRecoil, ForceMode.Impulse);
        nState = (int)State.Stuned;
        ChangeColor(Stats.StartColor);
    }

    /// <summary>
    /// Arrête le stun
    /// </summary>
    protected override void StopStun()
    {
        GetComponent<Animator>().SetTrigger("StopStun");
        base.StopStun();
        SpotPlayer();
    }

    void ChangeColor(Material NewMaterial)
    {
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            if (transform.GetChild(0).GetChild(i).GetComponent<MeshRenderer>())
            {
                transform.GetChild(0).GetChild(i).GetComponent<MeshRenderer>().material = NewMaterial;
            }
        }
    }


    /// <summary>
    /// Fonction qui lance le recover apres un impact
    /// </summary>
    void StartRecover()
    {
        ResetTimers();
        nState = (int)State.Recovering;
        ChangeColor(Stats.StartColor);
    }

    /// <summary>
    /// Fin de recover après un impact
    /// </summary>
    void StopRecover()
    {
        ResetTimers();
        SpotPlayer();

    }

    public void PlayerChangePosition()
    {
        bPlayerMoving = true;
    }
    public void PlayerArrivedToPosition()
    {
        bPlayerMoving = false;
    }

    void Shoot()
    {
        GameObject.FindObjectOfType<C_Camera>().AddShake(3);
        for (int i = 0; i < Stats.nbBulletPerShoot; i++)
        {
            GameObject CurrBullet = Instantiate(BulletPrefabs);
            CurrBullet.GetComponent<C_ShooterBullet>().OnCreation(player.gameObject, CanonPlacement.transform.position, Stats.amplitudeMultiplier, bullet);
        }
    }

    /// <summary>
    /// Reset Des timers
    /// </summary>
    void ResetTimers()
    {
        fETimerLoading = 0;
        fETimerRecovering = 0;
        fETimerbeforeNextAttack = 0;
        nBulletShot = 0;
    }

    public override void TakeDamage(int damage, bool ignoreResistance, float StunValue)
    {
        CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "SE_Shooter_Damage", false, 1, 0, 0, false); ;
        base.TakeDamage(damage, ignoreResistance, StunValue);
    }

    public override void Die(bool isSuicide, bool countsAsPlayerKill = true)
    {
        CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "SE_Shooter_Death", false, .6f);
        GameObject.FindObjectOfType<C_Fx>().BigEnnemiDied(transform.position);
        base.Die(isSuicide, countsAsPlayerKill);
    }



}
