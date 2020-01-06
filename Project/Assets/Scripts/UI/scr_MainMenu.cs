using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class scr_MainMenu : MonoBehaviour
{

    // STOCKAGE DES OBJETS DE LA SCENE
    [SerializeField]
    Image FonduNoir = null;
    [SerializeField]
    GameObject hLogoRoot = null;
    [SerializeField]
    GameObject hEtpaLogo = null;
    [SerializeField]
    GameObject hMenuRoot = null;
    [SerializeField]
    GameObject hOptions = null;
    [SerializeField]
    GameObject hQuit = null;

    [SerializeField]
    GameObject hOptionsRoot = null;
    [SerializeField]
    GameObject OptionOne = null;
    [SerializeField]
    GameObject OptionTwo = null;
    [SerializeField]
    GameObject OptionThree = null;
    [SerializeField]
    GameObject QuitOptionButton = null;

    // BARRE DE CHARGEMENT
    [SerializeField]
    Slider hSlider = null;

    // ALPHA FONDU NOIR
    float fCurrentAlpha = 1;
    int fDirAlpha = 1;
    [SerializeField]
    float fTimeTransition = 1;

    bool bHasArrivedToMenu = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MenuCoroutineInit());
    }

    // Update is called once per frame
    void Update()
    {

        fCurrentAlpha += Time.deltaTime / fTimeTransition * fDirAlpha;
        if (fCurrentAlpha > 1)
        {
            fCurrentAlpha = 1;
            fDirAlpha = 0;
        }
        if (fCurrentAlpha < 0)
        {
            fCurrentAlpha = 0;
            fDirAlpha = 0;
        }
        FonduNoir.color = new Color(0, 0, 0, fCurrentAlpha);

        if (Input.anyKeyDown)
            Skep();

    }


    // LOGO PANGOBLIN ET ETPA
    IEnumerator MenuCoroutineInit()
    {
        // Ligne pour attendre (temps)
        yield return new WaitForSeconds(1f);
        // Ligne pour fondu noir (Direction, Temps)
        FonduInit(-1, 0.5f);
        // Apparition du logo Pangoblin
        hLogoRoot.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        FonduInit(1, 0.5f);
        yield return new WaitForSeconds(0.8f);
        hLogoRoot.SetActive(false);
        hEtpaLogo.SetActive(true);
        FonduInit(-1, 0.5f);
        yield return new WaitForSeconds(2.5f);
        FonduInit(1, 0.5f);


        yield return new WaitForSeconds(1);
        hEtpaLogo.SetActive(false);
        FonduInit(-1, 0.5f);

        // Lance un suite d'action
        StartCoroutine(MainMenuArrive());
        yield break;
    }

    public void Skep()
    {
        if (!bHasArrivedToMenu)
        {
            StopAllCoroutines();
            StartCoroutine(SkipToMenu());
            bHasArrivedToMenu = true;
            Debug.Log("Skip");
        }
    }

    IEnumerator SkipToMenu()
    {
        FonduInit(1, 0.5f);
        yield return new WaitForSeconds(0.5f);
        hLogoRoot.SetActive(false);
        hEtpaLogo.SetActive(false);
        FonduInit(-1, 0.5f);
        // Lance un suite d'action
        StartCoroutine(MainMenuArrive());
        yield break;
    }

    // Arrivée du menu
    IEnumerator MainMenuArrive()
    {
        bHasArrivedToMenu = true;
        //Change la scale (scale, vitesse)
        hMenuRoot.GetComponent<scr_UI_Idle>().ChangeGlobalScale(1, 5);
        yield return new WaitForSeconds(0.5f);
        hOptions.GetComponent<scr_UI_Idle>().ChangeGlobalScale(1, 5);
        yield return new WaitForSeconds(0.5f);
        hQuit.GetComponent<scr_UI_Idle>().ChangeGlobalScale(1, 5);

        yield break;
    }

    // Fonction qui fait disparaitre le menu et apparaitre les options
    IEnumerator ClickOnOption()
    {
        hOptions.GetComponent<scr_UI_Idle>().ChangeGlobalScale(0, 5);
        yield return new WaitForSeconds(.5f);
        hMenuRoot.GetComponent<scr_UI_Idle>().ChangeGlobalScale(0, 5);
        yield return new WaitForSeconds(.5f);
        hQuit.GetComponent<scr_UI_Idle>().ChangeGlobalScale(0, 5);
        hOptionsRoot.GetComponent<scr_UI_Idle>().ChangeGlobalScale(1, 5);
        yield return new WaitForSeconds(.5f);
        OptionOne.GetComponent<scr_UI_Idle>().ChangeGlobalScale(1, 5);
        yield return new WaitForSeconds(.5f);
        OptionTwo.GetComponent<scr_UI_Idle>().ChangeGlobalScale(1, 5);
        yield return new WaitForSeconds(.5f);
        OptionThree.GetComponent<scr_UI_Idle>().ChangeGlobalScale(1, 5);
        yield return new WaitForSeconds(1f);
        QuitOptionButton.GetComponent<scr_UI_Idle>().ChangeGlobalScale(1, 5);
        yield break;
    }

    // Fonction qui fait disparaitre les interface de l'option
    IEnumerator QuitOption()
    {
        QuitOptionButton.GetComponent<scr_UI_Idle>().ChangeGlobalScale(0, 5);
        OptionOne.GetComponent<scr_UI_Idle>().ChangeGlobalScale(0, 5);
        OptionTwo.GetComponent<scr_UI_Idle>().ChangeGlobalScale(0, 5);
        OptionThree.GetComponent<scr_UI_Idle>().ChangeGlobalScale(0, 5);
        yield return new WaitForSeconds(.5f);
        hOptionsRoot.GetComponent<scr_UI_Idle>().ChangeGlobalScale(0, 5);
        yield return new WaitForSeconds(.5f);
        StartCoroutine(MainMenuArrive());
        yield break;
    }

    // Fonction qui lance la prochaine scene
    IEnumerator LauchGame()
    {
        FonduInit(1, 1);
        yield return new WaitForSeconds(1);
        StartCoroutine(LoadingScreen());
        //SceneManager.LoadScene("LD_01", LoadSceneMode.Single);
        yield break;
    }

    /// <summary>
    /// PAS TOUCHE
    /// </summary>

    public void LaunchGameFunc()
    {
        StartCoroutine(LauchGame());
    }
    public void QuitOptionFunc()
    {
        StartCoroutine(QuitOption());
    }

    public void QuitFunc()
    {
        Application.Quit();
    }
    // BARRE DE CHARGEMENT ET CHANGEMENT DE SCENE
    AsyncOperation async;
    IEnumerator LoadingScreen()
    {
        hSlider.gameObject.SetActive(true);
        async = SceneManager.LoadSceneAsync("LD_01", LoadSceneMode.Single);
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

    void FonduInit(int Dir, float Time)
    {
        fDirAlpha = Dir;
        fTimeTransition = Time;
    }

    public void OptionFunc()
    {
        StartCoroutine(ClickOnOption());
    }

}
