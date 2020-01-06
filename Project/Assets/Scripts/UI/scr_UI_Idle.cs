using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_UI_Idle : MonoBehaviour
{
    ///-----FLOAT
    ///Différence entre les 2
    [SerializeField] float fAmplitude = 0.05f;
    ///Vitesse
    [SerializeField] float fSpeed = 1;
    ///Taille de Référence
    [SerializeField] float fScaleRef = 1;
    ///Taille visée
    [SerializeField] float fScaleRefGoTo = 1;
    ///Delay d'apparition
    [SerializeField] float fDelay = 0;
    ///Gère la vitesse de transition entre le grossisement et la réduction
    [SerializeField] float fSpeedTransitionLerp = 10;
    ///Si l'idle est independant au timescale
    [SerializeField] bool bIdleIndependantToTimeScale = false;
    ///Si le changement de taille est independant au timescale
    [SerializeField] bool bIndependantToTimeScale = false;

    private void Awake()
    {
        transform.localScale = Vector3.one * fScaleRef;
    }

    /// <summary>
    /// UPDATE OU SE JOUE LES ANIMATIONS ET LES TRANSITIONS POUR LES INTERFACES
    /// </summary>
    void Update()
    {
        
        ///Changement répétitif de grossissement et rétrécissement
        transform.localScale = Vector3.one * fScaleRef + Vector3.one * Mathf.Sin(((bIdleIndependantToTimeScale ? Time.realtimeSinceStartup : Time.time) + fDelay) * fSpeed) * (fAmplitude * fScaleRef);
        ///Changement global de taille
        fScaleRef = Mathf.Lerp(fScaleRef, fScaleRefGoTo, (bIndependantToTimeScale ? Time.deltaTime / Time.timeScale : Time.deltaTime) * fSpeedTransitionLerp);
    }


    /// <summary>
    /// Fonction qui permet de réduire la taille de façon fluide
    /// </summary>
    public void ChangeGlobalScale(float Size = 1, float TransitionSpeed = 1)
    {
        fScaleRefGoTo = Size;
        fSpeedTransitionLerp = TransitionSpeed;
    }

}