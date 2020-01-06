using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Charger : C_Enemy
{

    M_Charger Stats = null;
    [SerializeField] Transform FxPos = null;

    /// <summary>
    /// Différents états de l'ennemi
    /// </summary>
    enum State { Nothing, Rotating, Loading, Charging, Stuned, Recovering, Repositioning, OnCac };

    [Tooltip("Variable qui indique l'étât actuel de l'ennemi")]
    int nState = 0;

    [Tooltip("Timer qui indique le temps passé à se préparer à charger")]
    float fETimerLoading = 0;
    [Tooltip("Timer qui indique le temps passé stun après avoir impacté le joueur")]
    float fETimerRecovering = 0;
    [Tooltip("Timer qui indique le avant la prochaine attaque cac")]
    float fETimerbeforeAttack = 0;

    public bool bPlayerMoving = false;

    public GameObject[] DetachableMesh = null;
    bool[] bDetached = null;
    GameObject hWorldObjectToKillBeforeDie = null;
    public GameObject Mesh = null;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        nState = (int)State.Nothing;
        Stats = enemy as M_Charger;
        SpotPlayer();
        bDetached = new bool[DetachableMesh.Length];
        for (int i = 0; i < bDetached.Length; i++)
        {
            bDetached[i] = false;
        }
        hWorldObjectToKillBeforeDie = new GameObject();
    }

    // Update is called once per physic frame
    protected virtual void FixedUpdate()
    {
        switch (nState)
        {
            case (int)State.Charging:
                Vector3 vPos = new Vector3(player.position.x, this.transform.position.y, player.position.z);
                Quaternion targetRotation = Quaternion.LookRotation(transform.position - vPos, Vector3.up);

                GameObject.FindObjectOfType<C_Camera>().AddShake(20 * Time.fixedDeltaTime);
                GetComponent<Rigidbody>().AddForce(-transform.forward * Stats.speedAccel * Time.fixedDeltaTime, ForceMode.Impulse);
                if (Vector3.Distance(transform.position, player.position) < Stats.distanceBeforeImpact)//² && Mathf.Abs(transform.position.y - player.position.y) < Stats.distanceBeforeImpact) 
                {
                    StopCharge(true);
                }
                else if (Quaternion.Angle(transform.rotation, targetRotation) > 90)
                {
                    StopCharge(false);
                }
                break;
            case (int)State.Repositioning:
                Vector3 vDir = Vector3.Normalize(transform.position - new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));

                transform.rotation = Quaternion.LookRotation(vDir, Vector3.up);

                float Dist = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(player.position.x, player.position.z));

                if (Dist < Stats.distanceBeforeImpact)
                {
                    GetComponent<Rigidbody>().velocity = vDir* Stats.fRepositioningSpeed;
                }
                else if (Dist > Stats.tooFarawayDistance)
                {
                    GetComponent<Rigidbody>().velocity = -vDir* Stats.fRepositioningSpeed;
                }
                else
                {
                    StopRepositioning(true);
                }
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (bPlayerMoving)
        {
            if (nState == (int)State.Loading)
            {
                ResetTimers();
                SpotPlayer();
            }
        }

        if (bisStuned)
            nState = (int)State.Stuned;
        switch (nState)
        {
            case (int)State.Nothing:
                break;
            case (int)State.Rotating:
                Vector3 vPos = new Vector3(player.position.x, this.transform.position.y, player.position.z);
                Quaternion targetRotation = Quaternion.LookRotation(transform.position - vPos, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * Stats.rotationSpeed);
                if (Quaternion.Angle(transform.rotation, targetRotation) < Stats.rotationMinimalBeforeCharge && ( player.position.y - transform.position.y) < 3)
                {
                    PlayerLocked();
                    StartLoading();
                }
                break;
            case (int)State.Loading:
                fETimerLoading += Time.deltaTime;
                Mesh.transform.position = this.transform.position + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)) * fETimerLoading * 0.2f / Stats.timeWaitBeforeDash;
                if (fETimerLoading > Stats.timeWaitBeforeDash)
                {
                    Mesh.transform.position = this.transform.position;
                    EndLoading(true);
                }
                break;
            case (int)State.Charging:
                break;
            case (int)State.Stuned:
                break;
            case (int)State.Recovering:
                fETimerRecovering += Time.deltaTime;
                if (fETimerRecovering > Stats.ImpactTimeRecover)
                {
                    StopRecover(true);
                }
                break;
            case (int)State.Repositioning:
                break;
            case (int)State.OnCac:
                fETimerbeforeAttack += Time.deltaTime;
                if (fETimerbeforeAttack > Stats.delayBetweenCacAttack)
                {
                    fETimerbeforeAttack -= Stats.delayBetweenCacAttack;
                    AttackPlayer();
                }
                break;
            default:
                Debug.LogWarning("Incorrect State on " + this.name + ". Please check the code. Current State = " + nState);
                break;
        }
    }

    /// <summary>
    /// Commencement de la rotation du chargeur vers le joueur
    /// </summary>
    void SpotPlayer()
    {
        nState = (int)State.Rotating;
        ChangeColor(Stats.StartColor);
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
        ChangeColor(Stats.ChargeColor);
    }

    /// <summary>
    /// Le chargeur arrete sa préparation (si bool true, ça lance la charge)
    /// </summary>
    /// <param name="bStartCharge"></param>
    void EndLoading(bool bStartCharge)
    {
        ResetTimers();
        if (bStartCharge)
            StartCharge();
    }

    /// <summary>
    /// Le chargeur commence sa charge
    /// </summary>
    void StartCharge()
    {
        nState = (int)State.Charging;
        GetComponent<Animator>().SetTrigger("StartCharging");
        GetComponent<Rigidbody>().AddForce(-transform.forward * Stats.speedAtStart, ForceMode.Impulse);
        ChangeColor(Stats.CacColor);
    }

    /// <summary>
    /// Le joueur arrête sa charge
    /// </summary>
    void StopCharge(bool HasImpactedPlayer)
    {
        GetComponent<Animator>().SetTrigger("StopCharging");
        ResetTimers();
        if (HasImpactedPlayer)
        {
            AttackPlayer();
            //StartRecover();
            IsStun();
        }
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        if (HasImpactedPlayer)
            GetComponent<Rigidbody>().AddForce(transform.forward * Stats.ImpactTimeRecoil, ForceMode.Impulse);
        else
        {
            ResetAggro();
        }
    }

    protected override void AttackPlayer()
    {
        if (nState == (int)State.OnCac)
        {
            if (!CheckIfTooClose())
            {
                PlayerLocked();
                GameObject.FindObjectOfType<C_Player>().TakeDamage(Stats.cacDammage);
                GetComponent<Animator>().SetTrigger("CacAttack");
            }
            else
            {
                StartRepositioning();
            }
        }
        else if(nState == (int)State.Charging)
        {
            int nDamageDone = Stats.nDamage;
            if (Stats.dammageGoesWithSpeed)
            {
                nDamageDone = Stats.nDamage + Mathf.RoundToInt ((GetComponent<Rigidbody>().velocity.magnitude / 100) * Stats.dammagePerSpeedMultiplier);
            }
            GameObject.FindObjectOfType<C_Player>().TakeDamage(nDamageDone);
            GameObject.FindObjectOfType<C_Camera>().AddShake(Stats.ShakeAtImpact);
        }
    }

    /// <summary>
    /// Déclenche le stun
    /// </summary>
    protected override void IsStun()
    {
        if (!bisStuned)
        {
            GameObject.FindObjectOfType<C_Fx>().StunFx(FxPos, enemy.timeStunned);
            GetComponent<Animator>().SetTrigger("StartStun");
        }
        base.IsStun();
        EndLoading(false);
        StopRecover(false);
        StopCharge(false);
        StopRepositioning(false);
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
        ResetAggro();
    }

    void ResetAggro()
    {
        float Dist = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(player.position.x, player.position.z));
        if (Dist > Stats.AfterStunDistanceNeededToReCharge)
        {
            SpotPlayer();
        }
        else
        {
            StartRepositioning();
        }
    }

    public override void TakeDamage(int damage, bool ignoreResistance, float StunValue)
    {
        if (bisStuned)
            ignoreResistance = true;
        base.TakeDamage(damage, ignoreResistance, StunValue);


        int IndexRemoveArmor = Mathf.RoundToInt(nCurrentHealth * DetachableMesh.Length / (int)enemy.nBaseHealth);

        RemoveArmor(IndexRemoveArmor);
    }

    void RemoveArmor(int IndexRemove)
    {
        for (int i = 0; i < DetachableMesh.Length; i++)
        {
            if (i > IndexRemove)
            {
                if (!bDetached[i])
                {
                    DetachableMesh[i].transform.SetParent(hWorldObjectToKillBeforeDie.transform);
                    DetachableMesh[i].AddComponent<Rigidbody>();
                    DetachableMesh[i].GetComponent<Rigidbody>().AddForce(Vector3.up * 5 + new Vector3(Random.Range(-1,1), Random.Range(-1, 1), Random.Range(-1, 1)) * 5, ForceMode.Impulse);
                    bDetached[i] = true;
                }
            }
        }
    }

    void ChangeColor (Material NewMaterial)
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
    void StopRecover(bool bMustAttack)
    {
        ResetTimers();
        if (bMustAttack)
        {
            StartAttack();
        }
    }

    /// <summary>
    /// Lance la série d'attaque au corps à corps
    /// </summary>
    void StartAttack()
    {
        if (!CheckIfTooClose())
        {
            nState = (int)State.OnCac;
            ChangeColor(Stats.CacColor);
        }
        else
        {
            StartRepositioning();
        }
    }

    /// <summary>
    /// Vérifie si l'ennemi est trop proche ou trop loin du joueur
    /// </summary>
    /// <returns></returns>
    bool CheckIfTooClose()
    {
        float Dist = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(player.position.x, player.position.z));
        if ((Dist < Stats.distanceBeforeImpact || Dist > Stats.tooFarawayDistance) && Mathf.Abs(player.position.y - transform.position.y) < 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Lance le repositionnement
    /// </summary>
    void StartRepositioning()
    {
        nState = (int)State.Repositioning;
        ChangeColor(Stats.ChargeColor);
    }

    /// <summary>
    /// Stop le repositionnement
    /// </summary>
    void StopRepositioning(bool bMustAttack)
    {
        ResetTimers();
        if (bMustAttack)
        {
            StartAttack();
        }
    }

    /// <summary>
    /// Reset Des timers
    /// </summary>
    void ResetTimers()
    {
        fETimerLoading = 0;
        fETimerRecovering = 0;
    }

    public void PlayerChangePosition()
    {
        bPlayerMoving = true;
    }
    public void PlayerArrivedToPosition()
    {
        bPlayerMoving = false;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<C_PathedEnemy>() && nState == (int)State.Charging)
        {
            if (collision.gameObject.GetComponent<Rigidbody>())
                collision.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(collision.gameObject.transform.position - transform.position) * 200, ForceMode.Impulse);
        }
        if (collision.gameObject.GetComponent<C_ObjectStopEnemiesAtImpact>() && (nState == (int)State.Charging || collision.relativeVelocity.magnitude > 15))
        {
            if (collision.gameObject.GetComponent<Rigidbody>())
                collision.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(collision.gameObject.transform.position - transform.position) * 200, ForceMode.Impulse);
            GameObject.FindObjectOfType<C_Camera>().AddShake(0.5f);
            TakeDamage(0, false, collision.gameObject.GetComponent<C_ObjectStopEnemiesAtImpact>().fValueStun);
            if (collision.gameObject.GetComponent<C_ObjectStopEnemiesAtImpact>().bIsDestroyedAtImpact)
                Destroy(collision.gameObject);

            //Destroy(collision.gameObject);
        }
    }


    public override void Die(bool isSuicide, bool countsAsPlayerKill = true)
    {
        GameObject.FindObjectOfType<C_Fx>().BigEnnemiDied(transform.position);
        Destroy(hWorldObjectToKillBeforeDie);
        base.Die(isSuicide, countsAsPlayerKill);
    }

}
