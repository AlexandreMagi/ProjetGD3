using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class C_SequenceHandler : MonoBehaviour
{
    [SerializeField]
    List<M_Sequence> sequences = null;

    M_Sequence currentSequence = null;

    Camera cameraObj = null;

    static C_SequenceHandler _instance;

    CinemachineVirtualCamera currentVirtualCamera = null;

    CinemachineBlenderSettings blenderSettings;

    CinemachineBrain cameraBrain = null;

    float fElapsedTime = 0;
    float fDelayOnBlendSequence = 0;
    int sequenceIndex = 0;

    uint enemiesKilled = 0;
    uint bufferedKills = 0;

    bool readSequences = true;

    bool isWaitingTimer = false;

    Vector3 PastCamPos = Vector3.one;
    Vector3 NewCamPos = Vector3.one;

    //CinemachineVirtualCamera StockPreviousCam = null;

    void Start()
    {
        _instance = this;

        cameraObj = Camera.main;

        cameraBrain = GameObject.FindObjectOfType<CinemachineBrain>();

        currentSequence = sequences[0];

        blenderSettings = ScriptableObject.CreateInstance("CinemachineBlenderSettings") as CinemachineBlenderSettings;

    }

    public static C_SequenceHandler Instance
    {
        get
        {
            return _instance;
        }
    }
    
    public float GetPurcentageBetweenNextCam()
    {
        float MaxDist = Vector3.Distance(PastCamPos, NewCamPos);
        float CurrDist = Vector3.Distance(PastCamPos, GameObject.Find("Main Camera").transform.position);
        if (MaxDist > 0)
            return CurrDist / MaxDist;
        else
            return 1;
    }

    /// <summary>
    /// During the Update, the handler will check if the completion confitions are met to get to the next sequence.
    /// </summary>
    void Update()
    {

        if (currentVirtualCamera == null) {
            currentVirtualCamera = GameObject.Find("Main Camera").GetComponent<CinemachineBrain>().ActiveVirtualCamera as CinemachineVirtualCamera;


            blenderSettings.m_CustomBlends = new CinemachineBlenderSettings.CustomBlend[1];
        }



        //VERIFICATION DES SEQUENCES
        if (readSequences && !isWaitingTimer)
        {


            if (fDelayOnBlendSequence > 0)
            {
                fDelayOnBlendSequence -= Time.deltaTime;
            }
            else
            {
                //DECLENCHEMENT DU FEEDBACK DE CAM
                if (GameObject.FindObjectOfType<C_CameraRail>())
                    GameObject.FindObjectOfType<C_CameraRail>().ChangeSpeedMoving(0, 50);

                C_Charger[] _Chargeurs = GameObject.FindObjectsOfType<C_Charger>();
                for (int i = 0; i < _Chargeurs.Length; i++)
                {
                    _Chargeurs[i].PlayerArrivedToPosition();
                }
                C_Shooter[] _Shooter = GameObject.FindObjectsOfType<C_Shooter>();
                for (int i = 0; i < _Shooter.Length; i++)
                {
                    _Shooter[i].PlayerArrivedToPosition();
                }

                //CHECK AVANCEE TIMER SEQUENCE
                if (currentSequence.sequenceType == M_Sequence.SequenceType.Timer)
                {
                    fElapsedTime += Time.deltaTime;
                    if (fElapsedTime >= currentSequence.fTimeSequenceDuration)
                    {
                        NextSequence();

                        fElapsedTime = 0;
                    }

                }

                //CHECK AVANCEE KILL SEQUENCE
                if(currentSequence.sequenceType == M_Sequence.SequenceType.KillEnnemies)
                {
                    if(enemiesKilled >= currentSequence.nEnemiesToKillInSequence)
                    {
                        Invoke("NextSequence", currentSequence.nTimeBeforeNextSequenceOnKills);
                        isWaitingTimer = true;
                    }
                }
            }
            

        }
       
    }

    /// <summary>
    /// Passes to the next sequence. Does nothing is no sequence is left
    /// </summary>
    public void NextSequence()
    {
        if (currentSequence.cutsSlowMoOnEnd) C_TimeScale.Instance.ForceStopSlowMo();

        GameObject.FindObjectOfType<C_CameraRail>().bFeedbckActivated = currentSequence.bEnableCamFeedback;
        GameObject.FindObjectOfType<C_CameraRail>().FeedbackTransition(currentSequence.bEnableCamTransition, currentSequence.fSpeedTransition); 

        if (currentSequence.hasEventOnEnd)
        {
            switch (currentSequence.seqEvent)
            {
                case M_Sequence.SequenceEndEventType.SlowMo:

                    TriggerUtil.TriggerSlowMo(currentSequence.tTimeBeforeEvent, currentSequence.slowMoDuration, currentSequence.slowMoPower);

                    break;

                case M_Sequence.SequenceEndEventType.Activation:

                    TriggerUtil.TriggerActivation(currentSequence.tTimeBeforeEvent, currentSequence.affected, currentSequence.isActivation);

                    break;

                case M_Sequence.SequenceEndEventType.Sound:

                    TriggerUtil.TriggerSound(currentSequence.tTimeBeforeEvent, currentSequence.soundPlayed, currentSequence.volume);

                    break;

                case M_Sequence.SequenceEndEventType.Animation:

                    TriggerUtil.TriggerAnimationsFromTags(currentSequence.tTimeBeforeEvent, currentSequence.tagsAnimated);

                    break;

                default:
                    break;
            }
        }

        if(sequenceIndex < sequences.Count-1)
        {
            sequenceIndex++;
            isWaitingTimer = false;

            currentSequence = sequences[sequenceIndex];

            //Debug.Log(currentVirtualCamera);

            //CREATION DU NOUVEAU BLEND
            CinemachineBlendDefinition blendDef = new CinemachineBlendDefinition
            {
                m_Style = currentSequence.animationStyle,
                m_Time = currentSequence.fAnimationTime
            };

            //SETUP BLEND
            CinemachineBlenderSettings.CustomBlend blend = new CinemachineBlenderSettings.CustomBlend
            {
                m_From = currentVirtualCamera.Name,
                m_To = currentSequence.vCamTargetName,
                m_Blend = blendDef
            };

            blenderSettings.m_CustomBlends[0] = blend;

            cameraBrain.m_CustomBlends = blenderSettings;


            //CHANGEMENT DE CAM
            currentVirtualCamera.Priority = 10;
            PastCamPos = currentVirtualCamera.transform.position;
            currentVirtualCamera = GameObject.Find(currentSequence.vCamTargetName).GetComponent<CinemachineVirtualCamera>();
            currentVirtualCamera.Priority = 11;
            NewCamPos = currentVirtualCamera.transform.position;

            //APPEL DE FONCTIONS DANS LES CHARGEURS
            C_Charger[] _Chargeurs = GameObject.FindObjectsOfType<C_Charger>();
            for (int i = 0; i < _Chargeurs.Length; i++)
            {
                _Chargeurs[i].PlayerChangePosition();
            }
            C_Shooter[] _Shooter = GameObject.FindObjectsOfType<C_Shooter>();
            for (int i = 0; i < _Shooter.Length; i++)
            {
                _Shooter[i].PlayerChangePosition();
            }

            fDelayOnBlendSequence = currentSequence.fAnimationTime + (currentSequence.sequenceType == M_Sequence.SequenceType.Timer ? currentSequence.fTimeSequenceDuration : 0);
            enemiesKilled = 0;


            //DECLENCHEMENT DU FEEDBACK DE CAM
            if (GameObject.FindObjectOfType<C_CameraRail>())
                GameObject.FindObjectOfType<C_CameraRail>().ChangeSpeedMoving(Vector3.Distance(PastCamPos, NewCamPos) / 5 / fDelayOnBlendSequence, 50);

            if (currentSequence.sequenceType == M_Sequence.SequenceType.KillEnnemies)
            {
                if (currentSequence.bAcceptsBufferKill)
                {
                    enemiesKilled = bufferedKills;
                  
                }
                bufferedKills = 0;
            }
        }
        else
        {
            Debug.Log("No sequence left");

            readSequences = false;
        }
    }

    /// <summary>
    /// Called when an Enemy dies by the player
    /// </summary>
    public void OnEnemyKill()
    {
        if(currentSequence.sequenceType == M_Sequence.SequenceType.KillEnnemies)
        {
            enemiesKilled++;

           //Debug.Log(enemiesKilled);
        }
        else
        {   
            bufferedKills++;
        }
    }
}


