using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Enemy : MonoBehaviour
{
    // Start is called before the first frame update

    protected int nCurrentHealth;
    float fCurrentStunLevel;
    float fTimeRemainingStun;

    float fTimerPostStun = 1;

    protected bool bisStuned = false;

    [SerializeField]
    protected M_Enemy enemy = null;

    bool isDead = false;

    protected Transform player;

    protected virtual void Start()
    {
        nCurrentHealth = (int) enemy.nBaseHealth;

        fTimerPostStun = enemy.timeStunAllowedAfterFirst;
        player = Camera.main.transform;
    }

    protected virtual void Update()
    {
        //Fait des trucs
        if (bisStuned)
            StunUpdate();

        //Sécurité au cas où un ennemi tombe de la map.
        if(this.transform.position.y <= -30)
        {
            Die(true);
        }
    }

    public virtual void TakeDamage(int damage, bool ignoreResistance, float StunValue)
    {
        int damageTaken = (damage + (ignoreResistance ? 0 : enemy.nResistance));
        if (damageTaken > 0)
        {
            nCurrentHealth -= damageTaken;
        }
        AddStun(StunValue);
        if (nCurrentHealth <= 0)
        {
            Die(false);
        }
    }

    protected virtual void AttackPlayer()
    {
        GameObject.FindObjectOfType<C_Player>().TakeDamage(enemy.nDamage);
    }

    /// <summary>
    /// Security. The player is immune to back damage
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerEnter(Collider other)
    {
        //Layer 20 = ShieldBehind
        if(other.gameObject.layer == 20)
        {
            this.Die(true, false);
        }
    }

    /// <summary>
    /// L'ennemi subit une attaque qui peut potentiellement le stun
    /// </summary>
    /// <param name="Ammount"></param>
    void AddStun(float Ammount)
    {
        if (fTimerPostStun > 0)
            fCurrentStunLevel += Ammount;
        if (fCurrentStunLevel > enemy.stunJaugeMax)
        {
            IsStun();
        }
    }

    /// <summary>
    /// Déclenche le stun
    /// </summary>
    protected virtual void IsStun()
    {
        fCurrentStunLevel = 0;
        if (!bisStuned)
            fTimerPostStun = enemy.timeStunAllowedAfterFirst;
        fTimeRemainingStun = enemy.timeStunned;
        bisStuned = true;
    }

    /// <summary>
    /// Fonction qui gere les temps de stun
    /// </summary>
    protected void StunUpdate()
    {
        fTimerPostStun -= Time.deltaTime;
        fTimeRemainingStun -= Time.deltaTime;
        if (fTimeRemainingStun < 0)
            StopStun();
    }


    /// <summary>
    /// Arrête le stun
    /// </summary>
    protected virtual void StopStun()
    {
        fCurrentStunLevel = 0;
        fTimerPostStun = enemy.timeStunAllowedAfterFirst;
        bisStuned = false;
    }

    public virtual void Die(bool isSuicide, bool countsAsPlayerKill = true)
    {
        if (!isDead)
        {
            isDead = true;

            if(!isSuicide) GameObject.FindObjectOfType<C_Fx>().EnnemiDeath(transform.position);

            C_SequenceHandler handler = FindObjectOfType<C_SequenceHandler>();
            if (handler != null && countsAsPlayerKill)
            {
                handler.OnEnemyKill();
            }
            Destroy(this.gameObject);

        }

    }

}
