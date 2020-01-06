using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class C_CameraRail : MonoBehaviour
{

    #region VAR

    [Header("Cameras")]
    [SerializeField, Tooltip("Dummy qui bouge sur lequel va se fixer la cam")]
    GameObject CamDummy = null;
    [SerializeField, Tooltip("Camera render")]
    GameObject RenderingCam = null;
    [SerializeField, Tooltip("Camera Settings")]
    M_CamFB CamFeedBack = null;

    GameObject CamDummyValueFeedback;
    float CamDummyFov = 70;

    Vector3 vLookAtPos = Vector3.zero; // Vector3 qui va avoir un offset sur là où la camera devrait regarder
    Vector2 vRotateValue = Vector2.zero; // Valeur de rotation selon la position du curseur dans l'écran

    #region HeadBobingVar

    float fFrequency = 0; // Vitesse actuelle du headbobing
    float fAimedFrenquency = 0; // Vitesse visée
    float fFrequencyAccel = 1; // Acceleration
    float[] CurrentPurcentageStock; // Stock des pourcentages de chaque curve
    float[] CurrentInvertedStock; // Stock des inversions de chaque curve

    #endregion

    float CurrentRecoilValue = 0; // Recul valeur ref
    float CurrentRecoil = 0; // Recul actuel -> ref pow
    [SerializeField]
    M_Camera CamSettings = null;

    float Chargevalue = 0;
    [SerializeField]
    M_WeaponChargedFullFB CurrentPresetChargedWeapon = null;
    float fCurrentPurcentageFBCharged = 0;
    bool bFeedbackChargedStarted = false;

    float fCurrentFovModif = 0;

    [HideInInspector]
    public bool bFeedbckActivated = true;
    bool bFeedbackTransition = false;
    float fCurrentSpeedTransition = 0;

    bool bStepSoundPlayed = false;

    #endregion

    private void Awake()
    {

        if (CamDummy.GetComponent<Camera>())
        {
            CamDummy.GetComponent<Camera>().enabled = false;
        }
        if (RenderingCam.GetComponent<Camera>())
        {
            RenderingCam.GetComponent<Camera>().enabled = true;
        }
    }

    /// <summary>
    /// Fonction d'init
    /// </summary>
    private void Start()
    {


        // Changement des tailles des tableaux
        CurrentPurcentageStock = new float[CamFeedBack.CurvesAndValues.Length];
        CurrentInvertedStock = new float[CamFeedBack.CurvesAndValues.Length];
        // Stockage des valeurs de base
        for (int i = 0; i < CurrentPurcentageStock.Length; i++)
        {
            CurrentPurcentageStock[i] = CamFeedBack.CurvesAndValues[i].Decal;
            CurrentInvertedStock[i] = 1;
        }

        CamDummyValueFeedback = new GameObject();
    }

    public void FeedbackTransition(bool bValue, float fSpeedTransi)
    {
        bFeedbackTransition = bValue;
        if (bFeedbackTransition)
        {
            fCurrentSpeedTransition = fSpeedTransi;
        }
    }

    public void AddRecoil(float _RecoilValue)
    {
        CurrentRecoilValue += _RecoilValue;
        if (CurrentRecoilValue > CamSettings.RecoilMaxValue)
            CurrentRecoilValue = CamSettings.RecoilMaxValue;
    }



    /// <summary>
    /// Fonction lancé à chaque frame
    /// </summary>
    public void Update()
    {
        HandleFBAtCharge();

        if (Input.GetKeyDown(KeyCode.Space))
            AddRecoil(0.5f);

        CurrentRecoilValue -= Time.unscaledDeltaTime * CamSettings.RecoilRecover;
        if (CurrentRecoilValue < 0)
            CurrentRecoilValue = 0;

        CurrentRecoil = Mathf.Pow(CurrentRecoilValue, CamSettings.RecoilPow);

        if (fFrequency > CamFeedBack.fFrenquencyGoBackToZero)
            fFrequency = Mathf.MoveTowards(fFrequency, fAimedFrenquency, Time.deltaTime * fFrequencyAccel * fFrequency); // Changement de la frequence en fonction de la valeur "GoTo"
        else
            fFrequency = Mathf.MoveTowards(fFrequency, fAimedFrenquency, Time.deltaTime * fFrequencyAccel* CamFeedBack.fFrequencyDeccel); // Changement de la frequence en fonction de la valeur "GoTo"

        if (CurrentPurcentageStock[0] > CamFeedBack.fStepSoundPlay && !bStepSoundPlayed && bFeedbckActivated)
        {
            bStepSoundPlayed = true;
            CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "Step_0" + Random.Range(1, 5), false, 1f);
        }

        for (int i = 0; i < CurrentPurcentageStock.Length; i++)
        {
            CurrentPurcentageStock[i] += Time.deltaTime * CamFeedBack.CurvesAndValues[i].SpeedMultiplier * fFrequency; // Augmentation des pourcentages
            if (CurrentPurcentageStock[i] > 1)
            {
                if (i == 0)
                    bStepSoundPlayed = false;
                CurrentPurcentageStock[i] -= CurrentPurcentageStock[i] - CurrentPurcentageStock[i] % 1; // Reset des pourcentages si il depassent 1
                if (CamFeedBack.CurvesAndValues[i].IsInvertedAtEachLoop) // Inversion de certaines curves si le pourcentage depasse 1
                {
                    CurrentInvertedStock[i] *= -1;
                }
            }
        }


        // Reset de la position et de la rotation
        CamDummyValueFeedback.transform.position = CamDummy.transform.position;
        CamDummyValueFeedback.transform.rotation = CamDummy.transform.rotation;
        /// ----- GET VALUES ----- ///
        float[] CamFeedbackValues = GetValue(); // Get les values des steps
        vLookAtPos = Vector3.Lerp(vLookAtPos, CamDummy.transform.position + CamDummy.transform.forward * 5, Time.deltaTime * CamFeedBack.CamFollowSpeed); // Lerp du v3 vers la direction de la cam
        Quaternion targetRotation = Quaternion.LookRotation(vLookAtPos - CamDummy.transform.position, Vector3.up); // Regard de la cam

        /// ----- SET ROTATIONS ----- ///
        CamDummyValueFeedback.transform.rotation = targetRotation; // Oriente la cam
        CamDummyValueFeedback.transform.Rotate(0, vRotateValue.y, 0, Space.World); // Rotation de la cam selon le placement du curseur (en Y)
        CamDummyValueFeedback.transform.Rotate(vRotateValue.x, 0, 0, Space.Self); // Rotation de la cam selon le placement du curseur (en X)

        // Setup position selon recul
        CamDummyValueFeedback.transform.Translate(Vector3.back * CurrentRecoil, Space.Self);
        // Get de la valeur de rotation selon les mouvements horizontaux
        float vRotYByPosition = CamDummy.GetComponent<Camera>().WorldToScreenPoint(vLookAtPos).x - Screen.width / 2;
        vRotYByPosition = -vRotYByPosition * CamFeedBack.MaxRotateWhileMoving / (Screen.width / 2);
        // Rotations de la cam selon les mouvements horizontaux + rotate en Z des steps
        CamDummyValueFeedback.transform.Rotate(0, 0, -vRotYByPosition + CamFeedbackValues[1], Space.Self);
        // Setup position selon steps
        CamDummyValueFeedback.transform.Translate(Vector3.up * CamFeedbackValues[0], Space.World);

        fCurrentFovModif = Mathf.Lerp(fCurrentFovModif, fFrequency * CamFeedBack.fFovMultiplier, Time.deltaTime * CamFeedBack.fFovSpeed);
        // Change le FOV
        float fFovAddedByChargeFeedback = bFeedbackChargedStarted ? CurrentPresetChargedWeapon.CurveValue.Evaluate(fCurrentPurcentageFBCharged) * CurrentPresetChargedWeapon.FovMultiplier : 0;
        CamDummyFov = CamSettings.BaseFov + CamSettings.MaxFovDecal * GameObject.FindObjectOfType<C_WeaponMod>().GetChargeValue() + fFovAddedByChargeFeedback + fCurrentFovModif;



        if (!bFeedbckActivated)
        {
            if (bFeedbackTransition)
            {
                RenderingCam.transform.position = Vector3.Lerp(RenderingCam.transform.position, CamDummy.transform.position, Time.deltaTime * fCurrentSpeedTransition);
                RenderingCam.transform.rotation = Quaternion.Lerp(RenderingCam.transform.rotation, CamDummy.transform.rotation, Time.deltaTime * fCurrentSpeedTransition);
                RenderingCam.GetComponent<Camera>().fieldOfView = Mathf.Lerp (RenderingCam.GetComponent<Camera>().fieldOfView, CamDummy.GetComponent<Camera>().fieldOfView, Time.deltaTime * fCurrentSpeedTransition);
            }
            else
            {
                RenderingCam.transform.position = CamDummy.transform.position;
                RenderingCam.transform.rotation = CamDummy.transform.rotation;
                RenderingCam.GetComponent<Camera>().fieldOfView = CamDummy.GetComponent<Camera>().fieldOfView;
            }
        }
        else
        {
            if (bFeedbackTransition)
            {
                RenderingCam.transform.position = Vector3.Lerp(RenderingCam.transform.position, CamDummyValueFeedback.transform.position, Time.deltaTime * fCurrentSpeedTransition);
                RenderingCam.transform.rotation = Quaternion.Lerp(RenderingCam.transform.rotation, CamDummyValueFeedback.transform.rotation, Time.deltaTime * fCurrentSpeedTransition);
                RenderingCam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(RenderingCam.GetComponent<Camera>().fieldOfView, CamDummyFov, Time.deltaTime * fCurrentSpeedTransition);
            }
            else
            {
                RenderingCam.transform.position = CamDummyValueFeedback.transform.position;
                RenderingCam.transform.rotation = CamDummyValueFeedback.transform.rotation;
                RenderingCam.GetComponent<Camera>().fieldOfView = CamDummy.GetComponent<Camera>().fieldOfView;
            }
        }

        /*
        if (bFeedbackTransition)
        {
            fCurrentTransition = (fCurrentTransition < 1) ? fCurrentTransition + Time.deltaTime * fCurrentSpeedTransition : 1;
            fCurrentTransition = (fCurrentTransition > 0) ? fCurrentTransition - Time.deltaTime * fCurrentSpeedTransition : 0;
            RenderingCam.transform.position = Vector3.Lerp(RenderingCam.transform.position, CamDummy.transform.position, fCurrentTransition);
            RenderingCam.transform.rotation = Quaternion.Lerp(RenderingCam.transform.rotation, CamDummy.transform.rotation, fCurrentTransition);
        }   
        
        if (!bFeedbckActivated)
        {
            // Reset de la position et de la rotation
            RenderingCam.transform.position = Vector3.Lerp(RenderingCam.transform.position, CamDummy.transform.position, Time.deltaTime * 5);
            RenderingCam.transform.rotation = Quaternion.Lerp(RenderingCam.transform.rotation, CamDummy.transform.rotation, Time.deltaTime * 5);
            RenderingCam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(RenderingCam.GetComponent<Camera>().fieldOfView, CamDummy.GetComponent<Camera>().fieldOfView, Time.deltaTime * 5);
        }*/
    }

    private void HandleFBAtCharge()
    {
        float fChargedValuePast = Chargevalue;
        Chargevalue = GameObject.FindObjectOfType<C_WeaponMod>().GetChargeValue();
        if (Chargevalue == 1 && fChargedValuePast != 1)
        {
            bFeedbackChargedStarted = true;
            GameObject.FindObjectOfType<C_Camera>().AddShake(1);
        }

        if (Chargevalue == 1 && bFeedbackChargedStarted && fCurrentPurcentageFBCharged < 1)
        {
            fCurrentPurcentageFBCharged += Time.unscaledDeltaTime * CurrentPresetChargedWeapon.Speed;
        }
        else if (bFeedbackChargedStarted)
        {
            bFeedbackChargedStarted = false;
            fCurrentPurcentageFBCharged = 0;
        }
        if (fCurrentPurcentageFBCharged > 1)
        {
            GameObject.FindObjectOfType<C_Camera>().AddShake(1);
        }

        if (Chargevalue == 1)
        {
            GameObject.FindObjectOfType<C_Camera>().AddShake(10*Time.unscaledDeltaTime);
        }
        else if (Chargevalue != fChargedValuePast)
        {
            GameObject.FindObjectOfType<C_Camera>().AddShake(8*Time.unscaledDeltaTime);
        }

    }



    /// <summary>
    /// Setup les values de rotations de la cam selon le placement du curseur sur l'écran
    /// </summary>
    /// <param name="fMaxValue"></param>
    /// <param name="Pos"></param>
    public void DecalCurrentCamRotation(float fMaxValue, Vector2 Pos)
    {
        vRotateValue = new Vector2(-(Pos.y * (fMaxValue * 2) / Screen.height - fMaxValue), Pos.x * (fMaxValue * 2) / Screen.width - fMaxValue); // Get values
    }

    /// <summary>
    /// Permet de get les valeurs crées par le système
    /// </summary>
    /// <returns></returns>
    public float[] GetValue()
    {
        float[] tReturnedValues = new float[CamFeedBack.CurvesAndValues.Length];
        for (int i = 0; i < tReturnedValues.Length; i++)
        {
            tReturnedValues[i] = (CamFeedBack.CurvesAndValues[i].Curve.Evaluate(CurrentPurcentageStock[i]) * CurrentInvertedStock[i] * CamFeedBack.CurvesAndValues[i].MultipyValue) + CamFeedBack.CurvesAndValues[i].OffsetValue; // Calcul de la value
            if (fFrequency < CamFeedBack.fFrenquencyGoBackToZero)
            {
                tReturnedValues[i] = Mathf.Lerp(tReturnedValues[i], 0, (CamFeedBack.fFrenquencyGoBackToZero - fFrequency) / CamFeedBack.fFrenquencyGoBackToZero) + CamFeedBack.CurvesAndValues[i].OffsetValue;
            }
        }
        return tReturnedValues;
    }

    /// <summary>
    /// Permet de changer la frequence du headbobing
    /// </summary>
    /// <param name="Frequency"></param>
    /// <param name="Acceleration"></param>
    public void ChangeSpeedMoving(float Frequency, float Acceleration = -1)
    {
        fAimedFrenquency = Frequency;
        if (Acceleration > 0)
            fFrequencyAccel = Acceleration;
    }




}
