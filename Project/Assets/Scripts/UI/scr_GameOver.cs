using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class scr_GameOver : MonoBehaviour
{

    [SerializeField]
    private GameObject GameOverRoot = null;
    [SerializeField]
    private GameObject Background = null;
    [SerializeField]
    private GameObject Fondu = null;
    [SerializeField]
    private GameObject Others = null;
    [SerializeField]
    private Slider hSlider = null;

    float fCurrentAlpha = 0;
    float fMaxAlpha = 0.5f;
    float fTimeTransition = 4;
    int fDirAlpha = 0;
    bool bGameOver = false;
    bool bGameRestart = false;
    bool bGameDoNotTouchAnything = false;

    // Start is called before the first frame update
    void Start()
    {
        GameOverRoot.SetActive(false);
        hSlider.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (bGameOver && !bGameDoNotTouchAnything)
        {
            if (fCurrentAlpha == fMaxAlpha)
            {
                Others.GetComponent<scr_UI_Idle>().ChangeGlobalScale(1, 2);
            }
            else
            {
                // --- ALPHA FONDU NOIR
                fCurrentAlpha += Time.deltaTime / Time.timeScale / fTimeTransition * fDirAlpha;
                if (fCurrentAlpha > fMaxAlpha)
                {
                    fCurrentAlpha = fMaxAlpha;
                    fDirAlpha = 0;
                }
                if (fCurrentAlpha < 0)
                {
                    fCurrentAlpha = 0;
                    fDirAlpha = 0;
                }
                Background.GetComponent<Image>().color = new Color(0, 0, 0, fCurrentAlpha);
            }


        }
    }

    public void GameOver()
    {
        if (!bGameOver)
        {
            CustomSoundManager.Instance.StopAllSound();
            CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "GameOver_Sound", false, 1);
            GameObject.FindObjectOfType<C_TimeScale>().AddSlowMo(0.999f, 500);
            GameOverRoot.SetActive(true);
            bGameOver = true;
            Background.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            Others.GetComponent<scr_UI_Idle>().ChangeGlobalScale(0, 200);
            fDirAlpha = 1;
        }
    }

    public void GameRestart(bool Prio)
    {
        if (!bGameDoNotTouchAnything && (Prio || bGameOver) && !bGameRestart)
        {
            CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "RestartSound", false, 1);
            fCurrentAlpha = 0;
            fMaxAlpha = 1;
            fTimeTransition = 4;
            Background = Fondu;
            fDirAlpha = 1;
            StartCoroutine(LoadingScreen());
        }
    }

    // BARRE DE CHARGEMENT ET CHANGEMENT DE SCENE
    AsyncOperation async;
    IEnumerator LoadingScreen()
    {
        yield return new WaitForSecondsRealtime(fTimeTransition);
        FindObjectOfType<C_Player>().ResetPlayerBlood();
        bGameDoNotTouchAnything = true;
        hSlider.gameObject.SetActive(true);

        

        async = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        async.allowSceneActivation = false;

        while (async.isDone == false)
        {
            hSlider.value = async.progress;
            if (async.progress == 0.9f)
            {
                hSlider.value = 1f;
                async.allowSceneActivation = true;
            }
            yield return null;
        }
    }

    public bool GetGameOver()
    {
        return bGameOver;
    }

}
