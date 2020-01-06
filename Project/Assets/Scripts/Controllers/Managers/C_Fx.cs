using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Fx : MonoBehaviour
{
    public ParticleSystem fxBasicImpact;
    public ParticleSystem fxWallsImpact;
    public ParticleSystem fxExplosionBullet;
    public ParticleSystem fxOrbe;
    public ParticleSystem fxOrbeRepulse;
    public ParticleSystem fxEnnemiDeath;
    public ParticleSystem fxPlayerDamages;
    public ParticleSystem fxOrbReady;
    public ParticleSystem fxMiddleZone;
    public ParticleSystem fxZeroG;
    public ParticleSystem fxStun;
    public ParticleSystem fxReenableShoot;
    public ParticleSystem fxChargedShot;
    public ParticleSystem fxChargedShotReleased;
    public ParticleSystem fxTriggerShoot;
    public ParticleSystem fxExplosionBulletTwo;
    public ParticleSystem fxBigEnnemiDied;
    public ParticleSystem fxOrbAvailable;
    public ParticleSystem fxShooterDebrisAnim;
    public ParticleSystem fxOrbGatherableExplosion;
    public ParticleSystem fxOrbGatherableExplosionFinal;
    public ParticleSystem fxBoxDestruction;
    public ParticleSystem fxGatherOrb;
    public ParticleSystem fxCollectiblesShoot;
    public ParticleSystem fxSmokeExplosion;
    public ParticleSystem fxSmokeExplosionStatue;
    public ParticleSystem fxDebrisFromCeilling;
    public ParticleSystem fxDebrisStatue;
    public ParticleSystem fxComboBar;
    public ParticleSystem fxLoseLife;

    private void Start()
    {
        fxOrbReady = Instantiate(fxOrbReady);
    }
    public void BoxDestruction(Vector3 vPos)
    {
        ParticleSystem Clone = Instantiate(fxBoxDestruction, vPos, Quaternion.identity);
        Clone.Play();
    }

    public void LoseLife()
    {
        fxLoseLife.Play();
    }

    public void ComboSplash()
    {
        fxComboBar.Play();
    }

    public void DebrisStatue()
    {
        fxDebrisStatue.Play();
    }

    public void SmokeExplosionStatue()
    {
        fxSmokeExplosionStatue.Play();
    }

    public void DebrisCeilling()
    {
        fxDebrisFromCeilling.Play();
    }

    public void SmokeExplosion()
    {
        fxSmokeExplosion.Play();
    }

    public void Collectibles(Vector3 vPos)
    {
        ParticleSystem Clone = Instantiate(fxCollectiblesShoot, vPos, Quaternion.identity);
        Clone.Play();
    }

    public void PlayerTakesOrbe()
    {
        fxMiddleZone.Stop();
        ParticleSystem Clone = Instantiate(fxOrbe, new Vector3(fxMiddleZone.transform.position.x, fxMiddleZone.transform.position.y + 2, fxMiddleZone.transform.position.z), Quaternion.identity);
        Clone.Play();
    }
    public void ShooterShootDebris(Vector3 vPos)
    {
        ParticleSystem Clone = Instantiate(fxShooterDebrisAnim, vPos, Quaternion.identity);
        Clone.Play();
    }
    public void ReenableShoot()
    {
        fxReenableShoot.Play();
    }
    public void ChargedShotFunc()
    {
        fxChargedShot.Play();
    }
    public void OrbAvailable()
    {
        fxOrbAvailable.Play();
    }
    public void ChargedShotReleasedFunc()
    {
        fxChargedShotReleased.Play();
    }

    public void PlayerTakesDamages(Vector3 vPos, Quaternion rPos)
    {
        ParticleSystem Clone = Instantiate(fxPlayerDamages, vPos, rPos);
        Clone.Play();
    }

    public void OrbGatherableExplosion(Vector3 vPos)
    {
        ParticleSystem Clone = Instantiate(fxOrbGatherableExplosion, vPos, Quaternion.identity);
        Clone.Play();
    }
    public void GatherOrb(Vector3 vPos)
    {
        ParticleSystem Clone = Instantiate(fxGatherOrb, vPos, Quaternion.identity);
        Clone.Play();
    }
    public void OrbGatherableExplosionFinal(Vector3 vPos)
    {
        ParticleSystem Clone = Instantiate(fxOrbGatherableExplosionFinal, vPos, Quaternion.identity);
        Clone.Play();
    }
    public void TriggerShoot(Vector3 vPos)
    {
        ParticleSystem Clone = Instantiate(fxTriggerShoot, vPos, Quaternion.identity);
        Clone.Play();
    }
    public void ImpactOnEnnemi (Vector3 vPos)
    {
        ParticleSystem Clone = Instantiate(fxBasicImpact, vPos, Quaternion.identity);
        Clone.Play();
    }
    public void BigEnnemiDied (Vector3 vPos)
    {
        ParticleSystem Clone = Instantiate(fxBigEnnemiDied, vPos, Quaternion.identity);
        Clone.Play();
    }
    public void ImpactOnWalls(Vector3 vPos)
    {
        ParticleSystem Clone = Instantiate(fxWallsImpact, vPos, Quaternion.identity);
        Clone.Play();
    }
    public void BulletExplosion (Vector3 vPos, float Size)
    {
        var main = fxExplosionBullet.main;
        main.startSize = Size;
        ParticleSystem Clone = Instantiate(fxExplosionBullet, vPos, Quaternion.identity);
        Clone.Play();
    }
    public void EnnemiDeath(Vector3 vPos)
    {
        ParticleSystem Clone = Instantiate(fxEnnemiDeath, vPos, Quaternion.identity);
        Clone.Play();
    }
    public void OrbeRepulse(Vector3 vPos)
    {
        ParticleSystem Clone = Instantiate(fxOrbeRepulse, vPos, Quaternion.identity);
        Clone.Play();
    }

    public void ZeroG(Vector3 vPos)
    {
        ParticleSystem Clone = Instantiate(fxZeroG, vPos, Quaternion.identity);
        Clone.Play();
    }

    public void ShooterBulletExplosion(Vector3 vPos, float Size)
    {
        var main = fxExplosionBulletTwo.main;
        main.startSize = Size;
        ParticleSystem Clone = Instantiate(fxExplosionBulletTwo, vPos, Quaternion.identity);
        Clone.Play();
    }

    public void GravityOrbFx(Vector3 vPos)
    {
        ParticleSystem Clone = Instantiate(fxOrbe, vPos, Quaternion.identity);
        Clone.Play();
    }

    public void StunFx(Transform Parent, float fStunTime)
    {
        var main = fxStun.main;
        main.startLifetime = fStunTime;
        ParticleSystem Clone = Instantiate(fxStun, Parent);
        Clone.Play();
    }

    public void OrbReady(bool bReady)
    {
        if (bReady && !fxOrbReady.isPlaying)
            fxOrbReady.Play();
        else if (!bReady)
            fxOrbReady.Stop();
    }

}
