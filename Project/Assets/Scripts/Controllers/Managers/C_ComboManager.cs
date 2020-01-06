using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_ComboManager : MonoBehaviour
{
    static C_ComboManager _instance;

    int currentCombo = 0;

    float fPercentComboLeft = 0;

    [SerializeField]
    int comboRequiredToShowCombo = 1;

    [SerializeField]
    float tTimeBeforeComboStartDecrease = .2f;
    float tTimeElapsedBeforeLastCombo = 0;

    [SerializeField]
    float tTimeComboOneToZero = 4;

    [SerializeField]
    float fComboMultiplierToDecrease = .95f;

    // Start is called before the first frame update
    void Start()
    {
        _instance = this;

        Invoke("UpdateCombo",.1f); 
    }

    public static C_ComboManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public void AddCombo(int combo)
    {
        if(combo > 0)
        {
            currentCombo += combo;

            fPercentComboLeft = 1;

            tTimeElapsedBeforeLastCombo = 0;

            FindObjectOfType<C_Fx>().ComboSplash();

            UpdateCombo();
        }
        if (GameObject.FindObjectOfType<scr_ShakeUIMultiplier>())
            GameObject.FindObjectOfType<scr_ShakeUIMultiplier>().AddScore(1);
    }

    public void MaintainCombo()
    {
        fPercentComboLeft = 100;

        tTimeElapsedBeforeLastCombo = 0;

        UpdateCombo();
    }

    /// <summary>
    /// Stops the current combo
    /// </summary>
    /// <param name="natual">Means if the combo is forced to drop or if it dropped with time</param>
    public void BreakCombo(bool natual)
    {
        currentCombo = 0;

        fPercentComboLeft = 0;

        UpdateCombo();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentCombo > 0)
        {
            if(tTimeElapsedBeforeLastCombo < tTimeBeforeComboStartDecrease)
            {
                tTimeElapsedBeforeLastCombo += Time.deltaTime;
            }
            else
            {
                fPercentComboLeft -= Time.deltaTime / (tTimeComboOneToZero * Mathf.Pow(fComboMultiplierToDecrease, (currentCombo - 1)));

                if(fPercentComboLeft <= 0)
                {
                    BreakCombo(false);
                }

                UpdateCombo();
            }

            //Debug.Log(fPercentComboLeft);
        }
    }

    void UpdateCombo()
    {
        C_Ui.Instance.UpdateCombo(currentCombo, fPercentComboLeft, comboRequiredToShowCombo);
    }
}
