using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_ShooterBullet : MonoBehaviour
{

    M_ShooterBullet bullet = null;
    float bulletSpeed = 0;
    float bulletRotationSpeed = 0;
    Vector3 RandomCurve = Vector3.one;
    Vector3 vPosStart;
    Vector3 PosAtLastFrame;
    GameObject hPlayer =null;
    Transform vPosEnd;
    GameObject hDummyIndicator = null;
    [SerializeField]
    GameObject hMesh = null;
    [SerializeField]
    GameObject hCircle = null;

    [SerializeField]
    Material mMatDesactivated = null;

    [SerializeField]
    ParticleSystem VFX_Trail = null;

    float fAmplitudeMissile = 1;
    bool bOnGravity = false;
    float AmplitudeShake = 0.3f;
    float ShakeSpeedLerp = 5;

    float fTimerEActivateCollider = 0;
    bool bCanCollideWithOthersBullet = false;

    public void OnCreation(GameObject Player, Vector3 EnnemiPos, float Amplitude, M_ShooterBullet bulletSettings)
    {
        hPlayer = Player;
        vPosEnd = Player.transform;
        vPosStart = EnnemiPos;
        fAmplitudeMissile = Amplitude;
        bullet = bulletSettings;
        bulletSpeed = bullet.BulletSpeed + Random.Range(-bullet.RandomSpeedAdded, bullet.RandomSpeedAdded);
        bulletRotationSpeed = bullet.RotationSpeed * Mathf.Sign(Random.Range(-1f, 1f));

        hDummyIndicator = new GameObject();
        hDummyIndicator.transform.position = vPosStart;
        hDummyIndicator.transform.LookAt (vPosEnd, Vector3.up);
        RandomCurve = new Vector3(Random.Range(bulletSettings.RandomFrom.x, bulletSettings.RandomTo.x), Random.Range(bulletSettings.RandomFrom.y, bulletSettings.RandomTo.y), Random.Range(bulletSettings.RandomFrom.z, bulletSettings.RandomTo.z));
        if (bulletSettings.bRandomRotationAtStart)
            hDummyIndicator.transform.Rotate(0, 0, Random.Range(0,360));

        ParticleSystem pr;
        pr = Instantiate(VFX_Trail, hMesh.transform);

        hCircle = Instantiate(hCircle);

    }

    // Update is called once per frame
    void Update()
    {

        if (fTimerEActivateCollider < bullet.TimeBeforeCollisionAreActived)
        {
            fTimerEActivateCollider += Time.deltaTime;
        } 
        else if (!bCanCollideWithOthersBullet)
        {
            bCanCollideWithOthersBullet = true;
        }

        if (hDummyIndicator != null && !bOnGravity)
        {
            hDummyIndicator.transform.Translate(0, 0, bulletSpeed * Time.deltaTime, Space.Self);
            float MaxDistance = Vector3.Distance(vPosEnd.position, vPosStart);
            float Curr = Vector3.Distance(vPosStart, hDummyIndicator.transform.position);

            hDummyIndicator.transform.Rotate(0, 0, Time.deltaTime * bullet.BulletRotation.Evaluate(Curr / MaxDistance) * bullet.RotationSpeed);
            transform.rotation = hDummyIndicator.transform.rotation;
            transform.position = hDummyIndicator.transform.position;
            hMesh.transform.position = Vector3.Lerp(hMesh.transform.position, hDummyIndicator.transform.position + new Vector3(Random.Range(-AmplitudeShake, AmplitudeShake), Random.Range(-AmplitudeShake, AmplitudeShake), Random.Range(-AmplitudeShake, AmplitudeShake)), Time.deltaTime* ShakeSpeedLerp);

            float ValueMax = bullet.BulletTrajectory.Evaluate(Curr / MaxDistance) * fAmplitudeMissile;
            transform.Translate(RandomCurve * ValueMax, Space.Self);

            Vector3 relativePos = transform.position - PosAtLastFrame;
            Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
            transform.rotation = rotation;

            PosAtLastFrame = transform.position;

            GameObject.FindObjectOfType<C_Camera>().AddShake(5 * Time.deltaTime);

            if (Curr / MaxDistance >= 1)
            {
                HitBullet();
                KillBullet();
            }

            hCircle.transform.position = hPlayer.transform.position + Vector3.Normalize(transform.position - vPosEnd.position) * 0.5f;
            hCircle.transform.localScale = Vector3.one * bullet.CircleScale.Evaluate(Curr / MaxDistance) * bullet.CircleScaleMultiplier;
            hCircle.transform.LookAt (hPlayer.transform, Vector3.up);
            hCircle.transform.GetChild(0).GetComponent<MeshRenderer>().material.SetColor ("_BaseColor", Color.Lerp(Color.yellow, Color.red, (Curr / MaxDistance)));
            hMesh.GetComponent<MeshRenderer>().material.SetColor ("_BaseColor", Color.Lerp(Color.yellow, Color.red, (Curr / MaxDistance)));
            hMesh.GetComponent<MeshRenderer>().material.SetColor ("_EmissionColor", Color.Lerp(Color.yellow, Color.red, (Curr / MaxDistance)));

        }
    }

    public void HitBullet()
    {
        if (Vector3.Distance(Camera.main.transform.position, this.transform.position) < 1)
            GameObject.FindObjectOfType<C_Player>().TakeDamage(bullet.BulletDammage);


        GameObject.FindObjectOfType<C_Fx>().ShooterBulletExplosion(this.transform.position, bullet.ExplosionRange * 1.3f);

        Collider[] tHits = Physics.OverlapSphere(this.transform.position, bullet.ExplosionRange);

        foreach (Collider hVictim in tHits)
        {
            if (hVictim.GetComponent<C_BulletAffected>())
            {
                hVictim.gameObject.GetComponent<C_BulletAffected>().OnBulletHit(bullet.BulletDammage, bullet.StunValue, bullet.BulletName);
                hVictim.gameObject.GetComponent<C_BulletAffected>().OnSoloHitPropulsion(transform.position, bullet.ForceAppliedOnImpact, bullet.BulletName);
            }
        }
        CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "SE_Shooter_Explosion", false, 0.7f);

    }

    public void KillBullet()
    {
        C_ShootTrigger triggerShoot = GetComponent<C_ShootTrigger>();

        if(triggerShoot != null)
        {
            triggerShoot.OnBulletTrigger();
        }

        Destroy(hCircle);
        Destroy(this.gameObject);
    }

    public void OnGravityPull()
    {
        bOnGravity = true;
        GetComponent<Rigidbody>().useGravity = true;
        hMesh.GetComponent<MeshRenderer>().material = mMatDesactivated;
        Destroy(hCircle);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (!collision.gameObject.GetComponent<C_ShooterBullet>())
        //{
        if (collision.gameObject.GetComponent<C_BulletAffected>() || collision.gameObject.GetComponent<C_ShooterBullet>() && bCanCollideWithOthersBullet)
        {
            GameObject.FindObjectOfType<C_Fx>().ShooterBulletExplosion(this.transform.position, 4);
            HitBullet();
            KillBullet();
            //}
        }
    }

}
