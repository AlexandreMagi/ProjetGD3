using System.Collections;
using System.Collections.Generic;
using Cinemachine.Utility;
using UnityEngine;

public class C_TimeScale : MonoBehaviour
{
    static C_TimeScale _instance;

    Vector2[] SlowMotions = new Vector2[0];
    bool bTimeStoped = false;

    void Awake()
    {
        _instance = this;
    }

    public static C_TimeScale Instance
    {
        get
        {
            return _instance;
        }
    }

    public void BulletCoroutineInit(GameObject hSend , GameObject ObjectHit, Vector3 vImpact, M_Bullet bullet)
    {
        StartCoroutine(BulletCoroutine(hSend, ObjectHit, vImpact, bullet));
    }

    IEnumerator BulletCoroutine(GameObject hSend, GameObject ObjectHit, Vector3 vImpact, M_Bullet bullet)
    {
        bTimeStoped = true;
        UpdateTimeScale();
        yield return new WaitForSeconds(bullet.fTimeStopAtImpact * Time.timeScale);
        C_Bullet bulComp = hSend ? hSend.GetComponent<C_Bullet>() : null;
        if(bulComp != null) bulComp.OnBulletHit(ObjectHit, vImpact);
        bTimeStoped = false;
        AddSlowMo(bullet.fSlowMoPower, bullet.fSlowMoDuration,0, bullet.fSlowMoProbability);
        yield break;
    }

    private void Update()
    {
        UpdateTimeScale();
    }

    public void ForceStopSlowMo()
    {
        StopCoroutine("SlowMo");
        Time.timeScale = 1;
    }

    void UpdateTimeScale()
    {
        float fCurrentSlowModPower = 0;
        if (bTimeStoped)
        {
            fCurrentSlowModPower = 0.999f;
        }
        else
        {
            for (int i = 0; i < SlowMotions.Length; i++)
            {
                SlowMotions[i].y -= Time.deltaTime / Time.timeScale;
                if (SlowMotions[i].y < 0)
                    SlowMotions[i] = new Vector2(0, 0);
                if (SlowMotions[i].x > fCurrentSlowModPower)
                    fCurrentSlowModPower = SlowMotions[i].x;
            }
        }
        Time.timeScale = 1 - fCurrentSlowModPower;
    }

    public void AddSlowMo (float fPower, float fDuration, float fDelay = 0, float fProbability = 1)
    {
        StartCoroutine(SlowMo(fPower, fDuration, fDelay, fProbability));
    }

    IEnumerator SlowMo(float fPower, float fDuration, float fDelay = 0, float fProbability = 1)
    {
        yield return new WaitForSeconds(fDelay / Time.timeScale);

        if (Random.Range(0f, 1f) < fProbability)
        {
            Vector2[] _SlowMotions = new Vector2[SlowMotions.Length + 1];
            for (int i = 0; i < SlowMotions.Length; i++)
            {
                _SlowMotions[i] = SlowMotions[i];
            }
            _SlowMotions[_SlowMotions.Length - 1] = new Vector2(fPower, fDuration);
            SlowMotions = _SlowMotions;
        }
        UpdateTimeScale();
        yield break;
    }


}
