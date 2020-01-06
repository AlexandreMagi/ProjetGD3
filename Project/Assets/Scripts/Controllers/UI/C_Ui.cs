using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class C_Ui : MonoBehaviour
{
    public Canvas myCanvas;
    public GameObject hCrossHairPoint;
    public GameObject hCrossHairCircle;
    public GameObject hCrossHairStraightLine;
    public GameObject hCrossHairCannotShoot;
    public GameObject hOrbReady;

    public TextMeshProUGUI hTextCombo;
    public TextMeshProUGUI hTextCloseCall;

    public Slider AccurateLifeSlider;
    public Slider FeedbackLifeSlider;
    public Slider hComboBar;

    public Slider LoadBarOrb;
    public Slider LoadBarSpecialMun;
    float fOffsetLoaderOrb= 0;
    float fOffsetLoaderMun = 0;
    float fOffsetLoaderOrbOffset= 0.025f;
    float fOffsetLoadeMunOffset= 0.01f;
    private Color LoadOrbColorBase = Color.white;
    private Color LoadMunColorBase = Color.white;

    float MaxSizeOrb = 1.8f;
    float CurrentOrbFeedBackSize = 0f;

    [HideInInspector]
    public M_Ui UiCurrentPreset;
    [SerializeField]
    bool bDisplayChangeWeaponUI = false;
    //[SerializeField]
    //M_Ui[] StockPresets = new M_Ui[0];
    //int CurrentIndex = 0;

    public Image UiWeaponIcon = null;

    Vector2 vPointOffsetSizeDeltaWhileShoot;
    Vector2 vCircleOffsetSizeDeltaWhileShoot;
    Vector2 vStraightOffsetSizeDeltaWhileShoot;

    float PointCurrentRotate = 0;
    float CircleCurrentRotate = 0;
    float StraightCurrentRotate = 0;

    C_Main mainController;

    C_Fx fxManager;

    C_WeaponMod weaponModsManager;

    //GameObject hCurrentWeapon = null;
    int nDir = 1;
    float fMaxScale = 3f;
    float fMinScale = 1f;

    float fMaxReadyScale = 1.2f;
    int nReadyDir = -1;
    public GameObject WeaponReady;
    //public GameObject MachinGunReady;

    float LifeMax = 0;
    float CurrentLife = 0;
    float LifeFeedback = 0;

    float Chargevalue = 0;

    [SerializeField]
    M_WeaponChargedFullFB CurrentPresetChargedWeapon = null;
    float fCurrentPurcentageFBCharged = 0;
    bool bFeedbackChargedStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        //UiCurrentPreset = StockPresets[CurrentIndex];
        //hCurrentWeapon = UiPrimaryWeapon.gameObject;
        WeaponReady.SetActive(bDisplayChangeWeaponUI);
        UiWeaponIcon.gameObject.SetActive(bDisplayChangeWeaponUI);
        Cursor.visible = false;
        mainController = GetComponent<C_Main>();
        LoadBarOrb.gameObject.transform.SetParent(hCrossHairCircle.transform);
        LoadBarSpecialMun.gameObject.transform.SetParent(hCrossHairCircle.transform);
        LoadOrbColorBase = LoadBarOrb.transform.GetChild(1).GetComponentInChildren<Image>().color;
        LoadMunColorBase = LoadBarSpecialMun.transform.GetChild(1).GetComponentInChildren<Image>().color;
        fxManager = GetComponent<C_Fx>();
        weaponModsManager = FindObjectOfType<C_WeaponMod>();
        Vector2 vHealthCur = GameObject.FindObjectOfType<C_Player>().GetLife();
        LifeMax = vHealthCur.y;
        CurrentLife = vHealthCur.x;
        LifeFeedback = vHealthCur.x;
    }

    public static C_Ui Instance { get; private set; }

    // Update is called once per frame
    void Update()
    {
        HandleFBAtCharge();

        if (GameObject.FindObjectOfType<C_WeaponMod>().GetOrbReady())
        {
            if (CurrentOrbFeedBackSize < MaxSizeOrb)
            {
                CurrentOrbFeedBackSize += Time.unscaledDeltaTime * 5;
            }
            else
            {
                CurrentOrbFeedBackSize = MaxSizeOrb;
            }
        }
        else
        {
            if (CurrentOrbFeedBackSize > 0)
            {
                CurrentOrbFeedBackSize -= Time.unscaledDeltaTime * 5;
            }
            else
            {
                CurrentOrbFeedBackSize = 0;
            }
        }


        float fDeltaTimePerso = Time.deltaTime;
        if (Time.timeScale > 0.005f)
            fDeltaTimePerso = Time.deltaTime / Time.timeScale;

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, mainController.GetControllerPos(), myCanvas.worldCamera, out pos);

        float fSizeAddedByFeedback = bFeedbackChargedStarted ? CurrentPresetChargedWeapon.CurveValue.Evaluate(fCurrentPurcentageFBCharged) * CurrentPresetChargedWeapon.UiSizeMultiplier : 0;

        // --- Point
        vPointOffsetSizeDeltaWhileShoot = Vector2.Lerp(vPointOffsetSizeDeltaWhileShoot, Vector2.zero, fDeltaTimePerso * UiCurrentPreset.PointReculRetourViseur);
        hCrossHairPoint.transform.position = myCanvas.transform.TransformPoint(pos);
        hCrossHairPoint.transform.rotation = Camera.main.transform.rotation;
        PointCurrentRotate += UiCurrentPreset.PointRotationSpeed * fDeltaTimePerso;
        hCrossHairPoint.transform.Rotate(0, 0, PointCurrentRotate + Mathf.Lerp(0,UiCurrentPreset.PointChargedRotation, Chargevalue));
        hCrossHairPoint.GetComponent<RectTransform>().sizeDelta = Vector2.one * Mathf.Sin((Time.time + UiCurrentPreset.PointOffsetIdle) * Mathf.Lerp(UiCurrentPreset.PointSpeedIdle,UiCurrentPreset.PointChargedSpeed,Chargevalue)) * UiCurrentPreset.PointAmplitudeIdle + Vector2.one * (Mathf.Lerp(UiCurrentPreset.PointBaseSizeViseur, UiCurrentPreset.PointChargedSize, Chargevalue)/*+ fSizeAddedByFeedback*/) + vPointOffsetSizeDeltaWhileShoot;
        if (Chargevalue == 1)
        {
            hCrossHairPoint.GetComponent<Image>().color = UiCurrentPreset.PointChargedColor;
        }
        else
        {
            hCrossHairPoint.GetComponent<Image>().color = Color.Lerp(hCrossHairPoint.GetComponent<Image>().color, UiCurrentPreset.PointBaseColor, fDeltaTimePerso * UiCurrentPreset.PointSpeedTransitionHit);
        }
            

        // --- Circle
        vCircleOffsetSizeDeltaWhileShoot = Vector2.Lerp(vCircleOffsetSizeDeltaWhileShoot, Vector2.zero, fDeltaTimePerso * UiCurrentPreset.CircleReculRetourViseur);
        hCrossHairCircle.transform.position = myCanvas.transform.TransformPoint(pos);
        hCrossHairCircle.transform.rotation = Camera.main.transform.rotation;
        CircleCurrentRotate += UiCurrentPreset.CircleRotationSpeed * fDeltaTimePerso;
        hCrossHairCircle.transform.Rotate(0, 0, CircleCurrentRotate + Mathf.Lerp(0, UiCurrentPreset.CircleChargedRotation, Chargevalue));
        hCrossHairCircle.GetComponent<RectTransform>().sizeDelta = Vector2.one * Mathf.Sin((Time.time + UiCurrentPreset.CircleOffsetIdle) * Mathf.Lerp(UiCurrentPreset.CircleSpeedIdle, UiCurrentPreset.CircleChargedSpeed, Chargevalue)) * Mathf.Lerp(UiCurrentPreset.CircleAmplitudeIdle, UiCurrentPreset.CircleChargedAmplitude, Chargevalue) + Vector2.one * (Mathf.Lerp(UiCurrentPreset.CircleBaseSizeViseur, UiCurrentPreset.CircleChargedSize, Chargevalue)+ fSizeAddedByFeedback) + vCircleOffsetSizeDeltaWhileShoot;
        if (Chargevalue == 1)
            hCrossHairCircle.GetComponent<Image>().color = UiCurrentPreset.CircleChargedColor;
        else
            hCrossHairCircle.GetComponent<Image>().color = Color.Lerp(hCrossHairCircle.GetComponent<Image>().color, UiCurrentPreset.CircleBaseColor, fDeltaTimePerso * UiCurrentPreset.CircleSpeedTransitionHit);

        // --- Straight Lines
        vStraightOffsetSizeDeltaWhileShoot = Vector2.Lerp(vStraightOffsetSizeDeltaWhileShoot, Vector2.zero, fDeltaTimePerso * UiCurrentPreset.StraightReculRetourViseur);
        hCrossHairStraightLine.transform.position = myCanvas.transform.TransformPoint(pos);
        hCrossHairStraightLine.transform.rotation = Camera.main.transform.rotation;
        StraightCurrentRotate += UiCurrentPreset.StraightRotationSpeed * fDeltaTimePerso;
        hCrossHairStraightLine.transform.Rotate(0, 0, StraightCurrentRotate + Mathf.Lerp(0, UiCurrentPreset.StraightChargedRotation, Chargevalue));
        hCrossHairStraightLine.GetComponent<RectTransform>().sizeDelta = Vector2.one * Mathf.Sin((Time.time + UiCurrentPreset.StraightOffsetIdle) * Mathf.Lerp(UiCurrentPreset.StraightSpeedIdle, UiCurrentPreset.StraightChargedSpeed, Chargevalue)) * Mathf.Lerp(UiCurrentPreset.StraightAmplitudeIdle, UiCurrentPreset.StraightChargedAmplitude, Chargevalue) + Vector2.one * (Mathf.Lerp(UiCurrentPreset.StraightBaseSizeViseur, UiCurrentPreset.StraightChargedSize, Chargevalue)+ fSizeAddedByFeedback) + vStraightOffsetSizeDeltaWhileShoot;
        if (Chargevalue == 1)
            hCrossHairStraightLine.GetComponent<Image>().color = UiCurrentPreset.StraightChargedColor;
        else
            hCrossHairStraightLine.GetComponent<Image>().color = Color.Lerp(hCrossHairStraightLine.GetComponent<Image>().color, UiCurrentPreset.StraightBaseColor, fDeltaTimePerso * UiCurrentPreset.StraightSpeedTransitionHit);

        Vector2 vHealthCur = GameObject.FindObjectOfType<C_Player>().GetLife();
        CurrentLife = vHealthCur.x;
        LifeFeedback = Mathf.MoveTowards(LifeFeedback, CurrentLife, Time.deltaTime / Time.timeScale * LifeMax);
        AccurateLifeSlider.value = CurrentLife / LifeMax;
        FeedbackLifeSlider.value = LifeFeedback / LifeMax;


        if (C_Main.Instance._playerCanOrb())
        {
            hOrbReady.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(hOrbReady.GetComponent<RectTransform>().sizeDelta, Vector2.one * (Mathf.Lerp(UiCurrentPreset.CircleBaseSizeViseur, UiCurrentPreset.CircleChargedSize, Chargevalue) + fSizeAddedByFeedback + 10) * 0.78f * CurrentOrbFeedBackSize, Time.unscaledDeltaTime * 5);
        }
        else
        {
            hOrbReady.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(hOrbReady.GetComponent<RectTransform>().sizeDelta, Vector2.zero, Time.unscaledDeltaTime * 5);
        }


        /*
        if (GameObject.FindObjectOfType<C_Player>().GetWeaponIndex() == 0)
        {
            //Do Thing
            hCurrentWeapon = UiPrimaryWeapon.gameObject;
            UiPrimaryWeapon.color = Color.white;
            UiSecondaryWeapon.color = Color.grey;

            SniperReady.transform.localScale = Vector3.Lerp(SniperReady.transform.localScale, Vector3.zero, Time.deltaTime * 8 / Time.timeScale);
            ScaleReadyUp(MachinGunReady);

            UiSecondaryWeapon.transform.localScale = Vector3.Lerp(UiSecondaryWeapon.transform.localScale, Vector3.one * fMinScale, Time.deltaTime*3 / Time.timeScale);
        }
        else
        {
            //Do Other Thing
            hCurrentWeapon = UiSecondaryWeapon.gameObject;
            UiSecondaryWeapon.color = Color.white;
            UiPrimaryWeapon.color = Color.grey;

            MachinGunReady.transform.localScale = Vector3.Lerp(MachinGunReady.transform.localScale, Vector3.zero, Time.deltaTime * 8 / Time.timeScale);
            ScaleReadyUp(SniperReady);

            UiPrimaryWeapon.transform.localScale = Vector3.Lerp(UiPrimaryWeapon.transform.localScale, Vector3.one * fMinScale, Time.deltaTime *3 / Time.timeScale);
        }
        ScaleUp(hCurrentWeapon);
        */

        ScaleUp(UiWeaponIcon.gameObject);
        ScaleReadyUp(WeaponReady);


        // ###################################################################### GESTION DU PLACEMENT ET DES COULEURS DES BARRES ###################################################################### //
        float Value = GameObject.FindObjectOfType<C_WeaponMod>().GetSliderStrongerBullet();
        if (Value == 666)
            LoadBarSpecialMun.gameObject.SetActive(false);
        else
        {
            LoadBarSpecialMun.value = Value;
            LoadBarSpecialMun.gameObject.transform.position = LoadBarSpecialMun.transform.parent.position;
            fOffsetLoaderMun = Mathf.Lerp(fOffsetLoaderMun, LoadBarSpecialMun.transform.parent.GetComponent<RectTransform>().sizeDelta.x / 4000 + fOffsetLoadeMunOffset, Time.deltaTime * 4);
            LoadBarSpecialMun.gameObject.transform.Translate(Vector3.up * fOffsetLoaderMun, Space.Self);
            if (Value == 1)
            {
                LoadBarSpecialMun.transform.GetChild(1).GetComponentInChildren<Image>().color = LoadMunColorBase;
            }
            else
            {
                LoadBarSpecialMun.transform.GetChild(1).GetComponentInChildren<Image>().color = LoadMunColorBase * Color.grey;
            }
        }

        Value = weaponModsManager.GetSliderOrb();
        LoadBarOrb.value = Value;
        if (Value == 1)
        {
            LoadBarOrb.transform.GetChild(1).GetComponentInChildren<Image>().color = LoadOrbColorBase;
            fxManager.OrbReady(true);
        }
        else
        {
            LoadBarOrb.transform.GetChild(1).GetComponentInChildren<Image>().color = LoadOrbColorBase * Color.grey;
            fxManager.OrbReady(false);
        }
        LoadBarOrb.gameObject.transform.position = LoadBarOrb.transform.parent.position;
        fOffsetLoaderOrb = Mathf.Lerp(fOffsetLoaderOrb, LoadBarOrb.transform.parent.GetComponent<RectTransform>().sizeDelta.x / 4000 + fOffsetLoaderOrbOffset, Time.deltaTime * 4);
        LoadBarOrb.gameObject.transform.Translate(Vector3.up * fOffsetLoaderOrb, Space.Self);



    }
    private void HandleFBAtCharge()
    {
        float fChargedValuePast = Chargevalue;
        Chargevalue = GameObject.FindObjectOfType<C_WeaponMod>().GetChargeValue();
        if (Chargevalue == 1 && fChargedValuePast != 1)
        {
            bFeedbackChargedStarted = true;
        }

        if (Chargevalue == 1 && bFeedbackChargedStarted)
        {
            fCurrentPurcentageFBCharged += Time.unscaledDeltaTime * CurrentPresetChargedWeapon.Speed;
        }
        else if (bFeedbackChargedStarted)
        {
            bFeedbackChargedStarted = false;
            fCurrentPurcentageFBCharged = 0;
        }
    }

    public void ChangeSprites(Sprite One, Sprite Two, Sprite Three)
    {
        hCrossHairPoint.GetComponent<Image>().sprite = One;
        hCrossHairCircle.GetComponent<Image>().sprite = Two;
        hCrossHairStraightLine.GetComponent<Image>().sprite = Three;
    }

    public void CannotShoot(bool canShoot)
    {
        if (canShoot)
        {
            hCrossHairCannotShoot.gameObject.SetActive(false);
            hCrossHairPoint.gameObject.SetActive(true);
        }
        else
        {
            hCrossHairCannotShoot.gameObject.SetActive(true);
            FindObjectOfType<C_Fx>().ReenableShoot();
            hCrossHairPoint.gameObject.SetActive(false);
        }
    }

    void ScaleReadyUp (GameObject Object)
    {
        if (nReadyDir == 1)
        {
            Object.transform.localScale = Vector3.Lerp(Object.transform.localScale, fMaxReadyScale * Vector3.one, Time.deltaTime * 8 / Time.timeScale);
            if (Mathf.Abs(fMaxReadyScale - Object.transform.localScale.x) < 0.1f)
                nReadyDir = 0;
        }
        else if (nReadyDir == 0)
        {
            Object.transform.localScale = Vector3.Lerp(Object.transform.localScale, Vector3.one, Time.deltaTime * 0.8f / Time.timeScale);
            if (Mathf.Abs(1 - Object.transform.localScale.x) < 0.05f)
                nReadyDir = -1;
        }
        else
            Object.transform.localScale = Vector3.Lerp(Object.transform.localScale, Vector3.zero, Time.deltaTime * 8 / Time.timeScale);
    }

    void ScaleUp (GameObject Object)
    {
        if (nDir == 1)
        {
            Object.transform.localScale = Vector3.Lerp(Object.transform.localScale, fMaxScale*Vector3.one, Time.deltaTime * 8 / Time.timeScale);
            if (Mathf.Abs(fMaxScale - Object.transform.localScale.x) < 0.1f)
                nDir = -1;
        }
        else
            Object.transform.localScale = Vector3.Lerp(Object.transform.localScale, fMinScale*Vector3.one, Time.deltaTime *3/ Time.timeScale);
    }

    public void Shoot()
    {
        vPointOffsetSizeDeltaWhileShoot += UiCurrentPreset.PointReculTir * Vector2.one;

        vCircleOffsetSizeDeltaWhileShoot += UiCurrentPreset.CircleReculTir * Vector2.one;

        vStraightOffsetSizeDeltaWhileShoot += UiCurrentPreset.StraightReculTir * Vector2.one;
    }

    public void HitSomethingImportant()
    {
        hCrossHairPoint.GetComponent<Image>().color = UiCurrentPreset.PointHitColor;
        vPointOffsetSizeDeltaWhileShoot += UiCurrentPreset.PointHitSizeBonus * Vector2.one;

        hCrossHairCircle.GetComponent<Image>().color = UiCurrentPreset.CircleHitColor;
        vCircleOffsetSizeDeltaWhileShoot += UiCurrentPreset.CircleHitSizeBonus * Vector2.one;

        hCrossHairStraightLine.GetComponent<Image>().color = UiCurrentPreset.StraightHitColor;
        vStraightOffsetSizeDeltaWhileShoot += UiCurrentPreset.StraightHitSizeBonus * Vector2.one;
    }
    /*
    public void ChangePreset(int precisedIndex = -1)
    {
        if (precisedIndex != -1)
        {
            CurrentIndex = precisedIndex;
        }
        else
        {
            CurrentIndex++;
            if (CurrentIndex >= StockPresets.Length)
                CurrentIndex = 0;
        }
        UiCurrentPreset = StockPresets[CurrentIndex];
        nDir = 1;
        nReadyDir = 1;

    }
    */
    public void ChangePreset(string sName, Sprite sLogoSprite)
    {
        if (bDisplayChangeWeaponUI)
        {
            WeaponReady.GetComponent<Text>().text = sName + " IS READY!";
            UiWeaponIcon.sprite = sLogoSprite;
            nDir = 1;
            nReadyDir = 1;
        }
    }

    public void UpdateCombo(int currentCombo, float comboLeft, int comboRequiredToShow)
    {
        if(currentCombo >= comboRequiredToShow && comboLeft > 0)
        {
            hComboBar.gameObject.SetActive(true);

            hComboBar.value = comboLeft;

            hTextCombo.text = "x " + currentCombo;
        }
        else
        {
            hComboBar.gameObject.SetActive(false);
        }
       
    }

    public void ActivateCloseCall(string callName, float tTimeOnScreen)
    {
        hTextCloseCall.text = callName;
        StopCoroutine("DoCloseCall");
        StartCoroutine(DoCloseCall(tTimeOnScreen));
    }

    private IEnumerator DoCloseCall(float tTimeOnScreen)
    {
        float tTimeSpawn = tTimeOnScreen / 10;
        float tTimeSlide = tTimeOnScreen * .8f;
        float tElapsedTime = 0;
        int currentState = 0;

        hTextCloseCall.rectTransform.anchoredPosition = new Vector3(-2000, -300, 0);


        while (true)
        {
            if (currentState == 0)
            {
                hTextCloseCall.rectTransform.anchoredPosition = Vector3.Lerp(new Vector3(-2000, -300, 0), new Vector3(-100, -300, 0), tElapsedTime/tTimeSpawn);

                if(tElapsedTime >= tTimeSpawn)
                {
                    currentState++;
                }
            }
            else if(currentState == 1)
            {
                hTextCloseCall.rectTransform.anchoredPosition = Vector3.Lerp(new Vector3(-100, -300, 0), new Vector3(100, -300, 0), tElapsedTime / (tTimeSpawn+tTimeSlide));

                if (tElapsedTime >= tTimeSpawn+tTimeSlide)
                {
                    currentState++;
                }
            }
            else if (currentState == 2)
            {
                hTextCloseCall.rectTransform.anchoredPosition = Vector3.Lerp(new Vector3(100, -300, 0), new Vector3(2000, -300, 0), tElapsedTime / tTimeOnScreen);

                if (tElapsedTime >= tTimeOnScreen)
                {
                    hTextCloseCall.rectTransform.anchoredPosition = new Vector3(2000, -300, 0);
                    yield break;
                }
            }

            tElapsedTime += Time.deltaTime;

            yield return new WaitForEndOfFrame();

        }

    }
}
