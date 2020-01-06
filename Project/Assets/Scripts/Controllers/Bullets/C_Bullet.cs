using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Bullet : MonoBehaviour
{
    [HideInInspector]
    public M_Bullet bullet = null;

    GameObject MainCam = null;

    Vector3 vPosStart;

    ushort nNbBounces = 0;
    ushort nNbImpact = 0;
    ushort nNbPierces = 0;

    /// <summary>
    /// Must be called at the creation of the bullet. Sets up the direction it must go and applies weapon modifiers.
    /// After being called, it kills the bullet in 5 seconds.
    /// </summary>
    /// <param name="positionSpawn"></param>
    /// <param name="mousePosition"></param>
    /// <param name="fImprecision"></param>
    /// <param name="WeaponIndex"></param>
    public void OnCreation(Vector3 positionSpawn, Vector2 mousePosition, float fImprecision, int weaponIndex)
    {
        /*if (WeaponIndex == 1)
            bullet = bulletSpecial;
        else
            bullet = bulletNormal;
        */

        //bullet = new M_Bullet();

        //Au lieu d'instancier une bullet, on lit un pré-set de balle.
        transform.position = positionSpawn;
        vPosStart = positionSpawn;

        MainCam = Camera.main.gameObject;

        OnShootBullet(mousePosition, fImprecision);

        Invoke("Kill", 2f);
    }

    /// <summary>
    /// Called by OnCreation. Reads the bullet parameters and act depending on these parameters
    /// </summary>
    /// <param name="mousePosition"></param>
    /// <param name="fImprecision"></param>
    protected void OnShootBullet(Vector2 mousePosition, float fImprecision)
    {
        Vector3 vImprecision = new Vector3(Random.Range(-fImprecision, fImprecision), Random.Range(-fImprecision, fImprecision), Random.Range(-fImprecision, fImprecision));
        Ray rRayBullet = MainCam.GetComponent<Camera>().ScreenPointToRay(mousePosition);
        rRayBullet.direction += vImprecision;

        if (bullet.bIsInstant)
        {
            //Shoot raycast
            RaycastHit hit;
            Physics.Raycast(rRayBullet, out hit, Mathf.Infinity, bullet.layerMaskHit);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.GetComponent<C_TriggerShootDetection>() && bullet.BulletName == "Shotgun")
                {
                    hit.collider.gameObject.GetComponent<C_TriggerShootDetection>().OnAnimDetection();
                }else if (hit.collider.gameObject.GetComponent<C_TriggerShootDetection>() && bullet.BulletName == "Base")
                {
                    hit.collider.gameObject.GetComponent<C_TriggerShootDetection>().OnAnimDetectionBase();
                }

                if (hit.collider.gameObject.GetComponent<C_GatherableOrb>())
                {
                    hit.collider.gameObject.GetComponent<C_GatherableOrb>().PlayerShootOnObjet(bullet.nDamage);
                }

                if (hit.collider.gameObject.GetComponent<C_ShooterBullet>())
                {
                    if(hit.distance < 2f)
                    {
                        //Perfect
                        C_Ui.Instance.ActivateCloseCall("PERFECT", 3f);
                        C_ComboManager.Instance.AddCombo(1);
                    }
                    else if(hit.distance < 5f)
                    {
                        C_Ui.Instance.ActivateCloseCall("Close call", 2f);
                        //Close call
                    }

                    hit.collider.gameObject.GetComponent<C_ShooterBullet>().HitBullet();
                    hit.collider.gameObject.GetComponent<C_ShooterBullet>().KillBullet();
                }
                else
                {
                    //Là c'est si on touche un truc
                    if (bullet.bActivateStopTimeAtImpact && GameObject.FindObjectOfType<C_Main>().CheckIfHitedIsSomethingImportant(hit.collider.gameObject))
                    {

                        GameObject.FindObjectOfType<C_TimeScale>().BulletCoroutineInit(this.gameObject, hit.collider.gameObject, hit.point, bullet);
                        //StartCoroutine(StopTimeAtImpact(hit.collider.gameObject, hit.point));
                    }
                    else
                    {
                        this.OnBulletHit(hit.collider.gameObject, hit.point);
                    }
                }
            }
            else
            {
                Destroy(this);
            }
        }
        else
        {
            //Shoot projectile
            transform.rotation = Quaternion.LookRotation(rRayBullet.direction, Vector3.up);
            GetComponent<Rigidbody>().AddForce(rRayBullet.direction * bullet.fImpulse, ForceMode.Impulse);
            GetComponent<Rigidbody>().useGravity = bullet.bAffectedByGravity;
            GetComponent<Rigidbody>().mass = bullet.fBulletMass;
            GetComponent<SphereCollider>().enabled = true;
            Invoke("SpawnMesh", .07f);
        }
    }

    /// <summary>
    /// Called by the <c>Rigidbody</c> when the bullet hits a layer it interacts with. Checks for needed feedbacks, then proceeds to OnBulletHit
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<C_ShooterBullet>())
        {
            other.gameObject.GetComponent<C_ShooterBullet>().HitBullet();
            other.gameObject.GetComponent<C_ShooterBullet>().KillBullet();
        }
        else
        {
            if (bullet.bActivateStopTimeAtImpact && GameObject.FindObjectOfType<C_Main>().CheckIfHitedIsSomethingImportant(other.gameObject))
            {
                //StartCoroutine(StopTimeAtImpact(other.gameObject, transform.position));
                GameObject.FindObjectOfType<C_TimeScale>().BulletCoroutineInit(this.gameObject, other.gameObject, transform.position, bullet);
            }
            else
            {
                this.OnBulletHit(other.gameObject, transform.position);
            }
        }
    }

    /// <summary>
    /// Called after the <c>Rigidbody</c> hit a layer, and after OnTriggerEnter. Depending on the bullet type, it either make it bounce, pierce, or just inflict damage.
    /// </summary>
    /// <param name="collideTo"></param>
    /// <param name="hitPoint"></param>
    public virtual void OnBulletHit(GameObject collideTo, Vector3 hitPoint)
    {
        int BulletDammage = bullet.nDamage;
        float BulletStun = bullet.StunValue;
        float fDist = Vector3.Distance(Camera.main.transform.position, hitPoint);

        if (bullet.bDammageFadeWithDistance)
            BulletDammage = fDist > bullet.fDistanceDammageFade ? 0 : bullet.nDamage - Mathf.CeilToInt(fDist * bullet.nDamage / bullet.fDistanceDammageFade);
        if (bullet.bStunFadeWithDistance)
            BulletStun = fDist > bullet.fDistanceStunFade ? 0 : bullet.StunValue - (fDist * bullet.StunValue / bullet.fDistanceStunFade);

        //Debug.Log(fDist * bullet.nDamage / bullet.fDistanceDammageFade);

        FindObjectOfType<C_Main>().OnBulletHit(collideTo, hitPoint, bullet.bExplosionAtImpact, bullet.fExplosionRange, bullet.bActivateStopTimeAtImpact, BulletDammage, bullet.fExplosionForce, bullet.ShakePerExplosion, BulletStun, bullet.BulletName, bullet.bExplosionDealsDamage);

        C_ShootTrigger trig = collideTo.GetComponent<C_ShootTrigger>();
        if (trig != null)
            trig.OnBulletTrigger();

        //Debug.Log("bullet hit");

        if (Random.Range(0f, 1f) < bullet.fSlowMoProbability)
            GameObject.FindObjectOfType<C_TimeScale>().AddSlowMo(bullet.fSlowMoPower, bullet.fSlowMoDuration);

        if (bullet.bBouncingBullet)
        {

            nNbImpact++;
            if (nNbImpact >= bullet.nImpactBeforeDie)
            {
                Kill();
            }

        }
        else if (bullet.bPiercingBullet && (bullet.piercedLayers == (bullet.piercedLayers | (1 << collideTo.layer))) && (nNbPierces < bullet.nPiercingsMax || bullet.nPiercingsMax == -1))
        {
            nNbPierces++;

            //On fait de la balistique c'est drôle :)
            Vector3 shotDirection = (hitPoint - MainCam.transform.position).normalized;
            Vector3 finalPiercePos = hitPoint;

            do
            {
                finalPiercePos += (shotDirection * bullet.pierceStep);
                transform.position = finalPiercePos;
                
            } while (Physics.Raycast(finalPiercePos, shotDirection, bullet.pierceStep, bullet.piercedLayers));

            transform.position = finalPiercePos + shotDirection*bullet.pierceStep;


            if (Physics.Raycast(transform.position, shotDirection, out RaycastHit hit, Mathf.Infinity, bullet.layerMaskHit))
            {
                this.OnBulletHit(hit.collider.gameObject, hit.point);
            }
        }
        else{
            if(bullet.bBouncingBullet && nNbBounces < bullet.nImpactBeforeDie)
            {
                nNbBounces++;

                Invoke("BoostBulletBounce", .05f);
            }
            else
            {
                Kill();
            }
        }
    }

    /// <summary>
    /// Accelerates the bullet in her current direction.
    /// </summary>
    void BoostBulletBounce()
    {
        GetComponent<Rigidbody>().AddForce(this.transform.forward * bullet.nBounceForce, ForceMode.Impulse);
    }

    /// <summary>
    /// Forces to destroy the bullet.
    /// </summary>
    void Kill()
    {
        //Forces a bullet to die -- Prevent memory leak on the long run
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Makes the mesh in prefab appear if necessary.
    /// </summary>
    void SpawnMesh()
    {
        //this.GetComponentInChildren<MeshRenderer>().enabled = true;
        //this.GetComponent<TrailRenderer>().enabled = true;
        transform.GetComponentInChildren<ParticleSystem>().Play();
    }
}
