using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scr_ShakeUIMultiplier : MonoBehaviour
{
    [SerializeField]
    GameObject ScoreDisplay = null;

    float fTraumaLevel = 0;
    float fShake = 0;

    [SerializeField]
    float fPow = 2.5f;
    [SerializeField]
    float fTimeMaxShake = 2;
    [SerializeField]
    float fMaxAngle = 5;
    [SerializeField]
    float fMaxScale = 1f;
    [SerializeField]
    bool bIndependantFromTimeScale = true;
    Vector2 vSpeedOscillationRange = new Vector2(50, 100);
    Quaternion[] ValueShake;

    [SerializeField]
    float fTraumaValueForaOne = 0.2f;

    bool bArcadeMod = true;

    // Start is called before the first frame update
    void Start()
    {
        ValueShake = new Quaternion[4];
        for (int i = 0; i < ValueShake.Length; i++)
        {
            ValueShake[i] = new Quaternion(0, 1, Random.Range(vSpeedOscillationRange.x, vSpeedOscillationRange.y), 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (bArcadeMod)
        {
            // -- Independance du TimeScale
            float fDeltaTime = Time.deltaTime;
            if (bIndependantFromTimeScale)
                fDeltaTime = Time.deltaTime / Time.timeScale;
            // --- Value
            fTraumaLevel -= fDeltaTime / fTimeMaxShake;
            if (fTraumaLevel < 0)
                fTraumaLevel = 0;
            fShake = Mathf.Pow(fTraumaLevel, fPow);


            // --- Oscillation de valeurs fait à la main pour fluidifier
            for (int i = 0; i < ValueShake.Length; i++)
            {
                ValueShake[i].x += fDeltaTime * ValueShake[i].z * ValueShake[i].w;
                if (ValueShake[i].x * ValueShake[i].w > ValueShake[i].y)
                {
                    ValueShake[i].x += (ValueShake[i].x * ValueShake[i].w - ValueShake[i].y) * -ValueShake[i].w * 2;
                    ValueShake[i].y = 1;
                    ValueShake[i].z = Random.Range(vSpeedOscillationRange.x, vSpeedOscillationRange.y);
                    ValueShake[i].w *= -1;
                }
            }

            // --- Display Shake
            float fAngle = fMaxAngle * fShake * ValueShake[0].x;
            float fScale = fMaxScale * fShake * ValueShake[1].x;
            ScoreDisplay.transform.rotation = new Quaternion(0, 0, 0, 0);
            ScoreDisplay.transform.Rotate(0, 0, fAngle + 90);
            ScoreDisplay.transform.localScale = Vector3.one + new Vector3(fScale, fScale, fScale);

        }

    }

    public void AddScore(float Value)
    {
        fTraumaLevel += Value * fTraumaValueForaOne / 1;

        if (fTraumaLevel > 1)
            fTraumaLevel = 1;
    }

}
