using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_BulletAffected : MonoBehaviour
{

    [SerializeField]
    private M_BulletResistance[] Resistances = new M_BulletResistance[0];

    /// <summary>
    /// Called when an object affected by bullets gets hit by one
    /// </summary>
    /// <param name="bulletDamage"></param>
    public void OnBulletHit(int bulletDamage, float bulletStun, string sBulletName)
    {
        if (gameObject.GetComponent<C_Enemy>() != null)
        {
            for (int i = 0; i < Resistances.Length; i++)
            {
                if (Resistances[i].BulletPreset.BulletName == sBulletName)
                {
                    bulletDamage = Mathf.RoundToInt(bulletDamage * Resistances[i].DammageMultiplier);
                    bulletStun = bulletStun * Resistances[i].StunMultiplier;
                }
            }
            gameObject.GetComponent<C_Enemy>().TakeDamage(bulletDamage, false,bulletStun);
        }

        //si autre type, ex : environnement, continuer pareil
    }

    /// <summary>
    /// Called when an object affected by bullets explosions gets hit by one
    /// </summary>
    /// <param name="positionHit"></param>
    /// <param name="explosionForce"></param>
    /// <param name="explosionRadius"></param>
    public void OnExplosionAffect(Vector3 positionHit, float explosionForce, float explosionRadius, string sBulletName)
    {
        for (int i = 0; i < Resistances.Length; i++)
        {
            if (Resistances[i].BulletPreset.BulletName == sBulletName)
            {
                explosionForce = explosionForce * Resistances[i].RecoilMultiplier;
            }
        }
        this.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, positionHit, explosionRadius);
    }

    /// <summary>
    /// Called when an object is hit by a single explosive bullet. Propels it forward
    /// </summary>
    /// <param name="positionHit"></param>
    /// <param name="forceApplied"></param>
    public void OnSoloHitPropulsion(Vector3 positionHit, float forceApplied, string sBulletName)
    {
        for (int i = 0; i < Resistances.Length; i++)
        {
            if (Resistances[i].BulletPreset.BulletName == sBulletName)
            {
                forceApplied = forceApplied * Resistances[i].RecoilMultiplier;
            }
        }
        this.GetComponent<Rigidbody>().AddForceAtPosition(Vector3.Normalize(transform.position - positionHit) * forceApplied, positionHit, ForceMode.Impulse);
    }
}
