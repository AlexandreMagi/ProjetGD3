using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_PathedEnemy : C_Enemy
{
    C_Pather pathToFollow;

    Transform currentFollow;
    Vector3 v3VariancePoisitionFollow;

    C_GravityAffected gAffect;

    Rigidbody rbBody;

    [SerializeField]
    int pathID = 0;

    bool isChasingPlayer;

    bool bIsDead = false;

    enum State { Basic, Waiting, Attacking};
    int nState = 0;

    float fETimerWait = 0;

    //Override en tant que M_Swarmer de la variable précédente
    M_PathedEnemy swarmer;

    protected override void Start()
    {
        base.Start();
        if (Random.Range(0,100) < 30)
            CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "SE_Swarmer_Spawn", false, 0.3f, 0.3f);
        Invoke("MaybeGrunt", 1f);
    }

    void MaybeGrunt()
    {
        if (Random.Range(0, 100) < 5)
            CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "SE_Swarmer_Grunt", false, 0.5f, 0.3f);
        Invoke("MaybeGrunt", 1f);
    }


    /// <summary>
    /// Gives the enemy a C_pather to follow.
    /// </summary>
    /// <param name="path"></param>
    public void SetPathToFollow(C_Pather path)
    {
        pathToFollow = path;

        currentFollow = path.GetPathAt(0);

        v3VariancePoisitionFollow = currentFollow.position;

        rbBody = GetComponent<Rigidbody>();

        swarmer = enemy as M_PathedEnemy;

        gAffect = GetComponent<C_GravityAffected>();
    }

    /// <summary>
    /// The FixedUpdate here is the path following part. The enemy will propel itself in the direction of the next waypoint.
    /// </summary>
    protected virtual void FixedUpdate()
    {

        if (pathToFollow != null && currentFollow != null && swarmer != null && gAffect != null && rbBody.useGravity && !gAffect.isAirbone)
        {
            if (nState == (int)State.Basic)
            {
                if (isChasingPlayer)
                {
                    v3VariancePoisitionFollow = player.position;

                }

                //TODO : Follow the path
                Vector3 direction = (new Vector3(v3VariancePoisitionFollow.x, transform.position.y, v3VariancePoisitionFollow.z) - transform.position).normalized;

                rbBody.AddForce(direction * swarmer.speed + Vector3.up * Time.fixedDeltaTime * swarmer.upScale);

                //transform.Translate(direction * swarmer.speed * Time.deltaTime);

                if (!isChasingPlayer)
                {
                    if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(v3VariancePoisitionFollow.x, v3VariancePoisitionFollow.z)) < swarmer.fDistanceBeforeNextPath)
                    {
                        currentFollow = pathToFollow.GetPathAt(pathID++);
                        if (currentFollow == null) pathID--;

                        if (currentFollow != null && currentFollow != player)
                        {
                            //Debug.Log("Proc variance, variance = "+swarmer.nVarianceInPath+"%");
                            //Debug.Log("Variance = "+ (swarmer.nVarianceInPath / 100 * Random.Range(-2f, 2f)));

                            v3VariancePoisitionFollow = new Vector3(
                                currentFollow.position.x + (swarmer.nVarianceInPath / 100 * Random.Range(-2f, 2f)),
                                currentFollow.position.y,
                                currentFollow.position.z + (swarmer.nVarianceInPath / 100 * Random.Range(-2f, 2f))
                            );

                            //Debug.Log("Initial pos X: " + currentFollow.position.x + " - Varied pos X : " + v3VariancePoisitionFollow.x);
                        }
                        else
                        {
                            currentFollow = player;
                        }

                    }

                    if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(player.position.x, player.position.z)) < swarmer.distanceToTargetPlayer)
                    {
                        isChasingPlayer = true;
                        currentFollow = player;
                    }
                }
                if (CheckDistance() && Physics.Raycast(this.transform.position, new Vector3(0, -1, 0), 0.5f) && transform.position.y < player.position.y + 1)
                {
                    nState = (int)State.Waiting;
                    rbBody.velocity = Vector3.zero;
                    GetComponent<Animator>().SetTrigger("PrepareToJump");
                }
            }
            else if (nState == (int)State.Waiting)
            {
                fETimerWait += Time.deltaTime;
                if (fETimerWait > swarmer.fWaitDuration)
                {
                    fETimerWait = 0;
                    if (CheckDistance())
                    {
                        nState = (int)State.Attacking;
                        GetComponentInChildren<MeshRenderer>().material.SetColor("_BaseColor", Color.red);  
                        GetComponentInChildren<MeshRenderer>().material.SetColor("_EmissionColor", Color.red);
                        rbBody.AddForce(Vector3.up* swarmer.fJumpForce, ForceMode.Impulse);
                        CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "SE_Swarmer_Attack", false, 0.4f, 0.3f);
                    }
                    else
                        nState = (int)State.Basic;

                }
            }
            else if (nState == (int)State.Attacking)
            {
                //TODO : Follow the path
                Vector3 direction = (new Vector3(player.position.x, transform.position.y, player.position.z) - transform.position).normalized;
                rbBody.AddForce(direction * swarmer.speed * swarmer.fSpeedMultiplierWhenAttacking + Vector3.up * Time.fixedDeltaTime * swarmer.upScale);
                if (!CheckDistance())
                {
                    nState = (int)State.Basic;
                    GetComponentInChildren<MeshRenderer>().material.SetColor("_BaseColor", Color.Lerp(Color.yellow,Color.red,0.5f));
                    GetComponentInChildren<MeshRenderer>().material.SetColor("_EmissionColor", Color.Lerp(Color.yellow, Color.red, 0.5f));
                }
            }
        }
    }

    bool CheckDistance()
    {
        if (Vector3.Distance(transform.position, player.position) < swarmer.fDistanceBeforeAttack)
            return true;
        else
            return false;
    }

    public override void TakeDamage(int damage, bool ignoreResistance, float StunValue)
    {
        base.TakeDamage(damage, ignoreResistance, StunValue);
        GetComponent<Animator>().SetTrigger("TakeDammage");
    }

    /// <summary>
    /// Update checks the distance with the player to see if it has to chase him or not.
    /// </summary>
    protected override void Update()
    {
        base.Update();

        //Debug.Log(Vector3.Distance(transform.position, player.position));

        //Debug.Log("Test " + Vector3.Distance(transform.position, player.position));

        if (Vector3.Distance(transform.position, player.position) < swarmer.fDistanceMelee)
        {
            AttackPlayer();

            Die(true);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<C_GravityAffected>())
        {
            float force = collision.impulse.magnitude;
            if (force >= 75)
            {
                TakeDamage(25, true, 0);
            }
            else if(force >= 85)
            {
                TakeDamage(40, true, 0);
            }
            else if (force >= 100)
            {
                TakeDamage(70, true, 0);
            }
                
        }
       
    }

    public override void Die(bool isSuicide, bool countsAsPlayerKill = true)
    {
        if (!bIsDead)
        {
            CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "SE_Swarmer_Death", false, 0.8f, 0.3f);
            bIsDead = true;
        }
        base.Die(isSuicide, countsAsPlayerKill);
    }

}