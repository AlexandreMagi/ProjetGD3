using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class C_PostProcessSettings : MonoBehaviour
{

    [SerializeField] VolumeProfile m_volumeProfile = null;



    [SerializeField] Vignette m_vignetteEffect = null;
    LensDistortion m_lensEffect;

    [Header("Vignette effect")]
    [SerializeField]
    float fVignetteIntensityIdle = 0.1f;
    [SerializeField]
    float fVignetteIntensityDamage = 0.46f;
    [SerializeField]
    float fVignetteIntensityIncreaseSpeed = 0f;

    [Header("Lens effect")]
    [SerializeField]
    float fLensIntensityIdle = 0f;
    [SerializeField]
    float fLensIntensityDamage = -25f;
    [SerializeField]
    float fLensIntensityIncreaseSpeed = 0f;

    [Header("Vignette color effect")]
    [SerializeField]
    Color color = Color.black;
    [SerializeField]
    Color colorDamage = Color.red;

    [Header("Other")]
    [SerializeField]
    public bool isTakingDamage = false;

    [SerializeField]
    public float fTimeToWaitOnDamages = 2f;

    void Start()
    {


        //m_vignetteEffect = m_volumeProfile.components

        //m_vignetteEffect = m_volumeProfile.components.GetType(UnityEngine.Rendering.Universal.Vignette);

        //m_vignetteEffect = //ScriptableObject.CreateInstance<Vignette>();
        m_vignetteEffect.intensity.Override(fVignetteIntensityIdle);
        m_vignetteEffect.color.Override(color);

        m_lensEffect = ScriptableObject.CreateInstance<LensDistortion>();
        m_lensEffect.intensity.Override(fLensIntensityIdle);

    }

    void Update()
    {

        LensUpdate();
        VignetteDamageUpdate();

    }

    void VignetteDamageUpdate()
    {
        if (isTakingDamage)
        {

            if (m_vignetteEffect.intensity.value <= fVignetteIntensityDamage)
            {

                m_vignetteEffect.intensity.value = fVignetteIntensityDamage;
                m_vignetteEffect.color.Override(colorDamage);

            }
            else
            {
                m_vignetteEffect.intensity.value = fVignetteIntensityDamage / fVignetteIntensityIncreaseSpeed + m_vignetteEffect.intensity.value - Time.deltaTime;

            }

        }
        else if (!isTakingDamage)
        {
            if (m_lensEffect.intensity.value >= fLensIntensityIdle)
            {

                m_vignetteEffect.intensity.value = fVignetteIntensityIdle;
                m_vignetteEffect.color.Override(color);

            }
            else
            {
                m_vignetteEffect.intensity.value = -fVignetteIntensityDamage / fVignetteIntensityIncreaseSpeed + m_vignetteEffect.intensity.value + Time.deltaTime;
            }
        }
    }

    void LensUpdate()
    {

        if (isTakingDamage)
        {

            if (m_lensEffect.intensity.value <= fLensIntensityDamage)
            {

                m_lensEffect.intensity.value = fLensIntensityDamage;

            }
            else
            {
                m_lensEffect.intensity.value = fLensIntensityDamage / fLensIntensityIncreaseSpeed + m_lensEffect.intensity.value - Time.deltaTime;

            }

        }
        else if (!isTakingDamage)
        {

            if (m_lensEffect.intensity.value >= fLensIntensityIdle)
            {


                m_lensEffect.intensity.value = fLensIntensityIdle;

            }
            else
            {
                m_lensEffect.intensity.value = -fLensIntensityDamage / fLensIntensityIncreaseSpeed + m_lensEffect.intensity.value + Time.deltaTime;

            }

        }
    }
}
