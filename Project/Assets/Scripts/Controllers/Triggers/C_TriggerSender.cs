using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ShowWhenAttribute;
public class C_TriggerSender : MonoBehaviour
{
    [SerializeField]
    TriggerType typeTrigger = 0;

    [Tooltip("C'est un bug si ça s'affiche alors que c'est pas un type spawner, vivez avec <3")]
    [ShowWhen("typeTrigger", Condition.Equals, (int) TriggerType.Spawner), SerializeField]
    C_SpawnerTrigger[] spawners = null;

    [SerializeField, ShowWhen("typeTrigger", Condition.Equals, (int)TriggerType.SlowMo)]
    float slowMoPower = 0;
    [SerializeField, ShowWhen("typeTrigger", Condition.Equals, (int)TriggerType.SlowMo)]
    float slowMoDuration = 0;

    [SerializeField, ShowWhen("typeTrigger", Condition.Equals, (int)TriggerType.Activation)]
    bool isActivation = false;
    [SerializeField, ShowWhen("typeTrigger", Condition.Equals, (int)TriggerType.Activation)]
    Activable affected = 0;

    [SerializeField, ShowWhen("typeTrigger", Condition.Equals, (int)TriggerType.Sound)]
    string soundPlayed = "";

    [Tooltip("ENTRE 0 ET 1 LE SON")]
    [SerializeField, ShowWhen("typeTrigger", Condition.Equals, (int)TriggerType.Sound)]
    float volume = 1;

    [SerializeField, ShowWhen("typeTrigger", Condition.Equals, (int)TriggerType.Animator)]
    Animator[] animated = null;

    [SerializeField]
    float tTimeBeforeStart = 0;

    bool timerStarted = false;


    [SerializeField, ShowWhen("typeTrigger", Condition.Equals, (int)TriggerType.Shake)]
    float ShakeValue = 0;

    void OnTriggerStay(Collider other)
    {
        StartTrigger();
    }

    void StartTrigger()
    {
        if (!timerStarted)
        {
            timerStarted = true;
            DoTrigger();

        }

    }

    void DoTrigger()
    {
        switch (typeTrigger)
        {
            case TriggerType.Spawner:
                TriggerUtil.TriggerSpawners(tTimeBeforeStart, spawners);
                this.gameObject.SetActive(false);
                break;

            case TriggerType.SlowMo:
                TriggerUtil.TriggerSlowMo(tTimeBeforeStart, slowMoDuration, slowMoPower);
                this.gameObject.SetActive(false);
                break;

            case TriggerType.Activation:
                TriggerUtil.TriggerActivation(tTimeBeforeStart, affected, isActivation);
                this.gameObject.SetActive(false);
                break;

            case TriggerType.Sound:
                TriggerUtil.TriggerSound(tTimeBeforeStart, soundPlayed, volume);
                this.gameObject.SetActive(false);
                break;

            case TriggerType.Animator:
                TriggerUtil.TriggerAnimators(tTimeBeforeStart, animated);
                this.gameObject.SetActive(false);
                break;

            case TriggerType.Shake:
                TriggerUtil.TriggerShake(tTimeBeforeStart, ShakeValue);
                this.gameObject.SetActive(false);
                break;

            default:
                break;
        }
        
    }

    void OnDrawGizmos()
    {
        
        Gizmos.color = Color.red;
        foreach (C_SpawnerTrigger spawner in spawners)
        {
            Gizmos.DrawLine(this.transform.position, spawner.transform.position);
        }
        
    }

    public enum TriggerType
    {
        Spawner = 0,
        Animator = 1, 
        SlowMo = 2,
        Activation = 3,
        Sound = 4,
        Shake = 5,
        Other = 9
    }

    public enum Activable
    {
        BaseWeapon = 0,
        Orb = 1,
        Both = 2,
        Other = 9
    }
}

public static class TriggerUtil
{

    //SPAWNERS
    public static void TriggerSpawners(float tTimeBeforeStart, C_SpawnerTrigger[] spawners)
    {
        C_Main.Instance.StartCoroutine(TriggerSpawnersCoroutine(tTimeBeforeStart, spawners));
    }
    
    static IEnumerator TriggerSpawnersCoroutine(float tTimeBeforeStart, C_SpawnerTrigger[] spawners)
    {
        yield return new WaitForSecondsRealtime(tTimeBeforeStart);

        foreach (C_SpawnerTrigger spawner in spawners)
        {
            spawner.StartSpawn();
        }

        yield break;
    }

    //SLOW MOTION
    public static void TriggerSlowMo(float tTimeBeforeStart,  float duration, float force)
    {
        C_Main.Instance.StartCoroutine(TriggerSlowMoCoroutine(tTimeBeforeStart, duration, force));
    }

    static IEnumerator TriggerSlowMoCoroutine(float tTimeBeforeStart, float duration, float force)
    {
        yield return new WaitForSecondsRealtime(tTimeBeforeStart);

        C_TimeScale.Instance.AddSlowMo(force, duration);

        yield break;
    }

    //ANIMATIONS
    public static void TriggerAnimators(float tTimeBeforeStart, Animator[] animators)
    {
        C_Main.Instance.StartCoroutine(TriggerAnimatorsCoroutine(tTimeBeforeStart, animators));
    }

    static IEnumerator TriggerAnimatorsCoroutine(float tTimeBeforeStart, Animator[] animators)
    {
        yield return new WaitForSecondsRealtime(tTimeBeforeStart);

        foreach (Animator anim in animators)
        {
            anim.SetTrigger("MakeAction");
        }

        yield break;
    }

    //WEAPON ACTIVATIONS
    public static void TriggerActivation(float tTimeBeforeStart, C_TriggerSender.Activable affected, bool isActivation)
    {
        C_Main.Instance.StartCoroutine(TriggerActivationCoroutine(tTimeBeforeStart, affected, isActivation));
    }

    static IEnumerator TriggerActivationCoroutine(float tTimeBeforeStart, C_TriggerSender.Activable activable, bool state)
    {
        yield return new WaitForSecondsRealtime(tTimeBeforeStart);

        C_Main.Instance.SetControlState(activable, state);

        yield break;
    }

    //SOUND EFFECT
    public static void TriggerSound(float tTimeBeforeStart, string soundPlayed, float volume)
    {
        C_Main.Instance.StartCoroutine(TriggerSoundCoroutine(tTimeBeforeStart, soundPlayed, volume));
    }

    static IEnumerator TriggerSoundCoroutine(float tTimeBeforeStart, string soundName, float volume)
    {
        yield return new WaitForSecondsRealtime(tTimeBeforeStart);

        CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, soundName, false, volume);

        yield break;
    }

    //SHAKE TRIGGER
    public static void TriggerShake(float tTimeBeforeStart, float shakeForce)
    {
        C_Main.Instance.StartCoroutine(TriggerShakeCoroutine(tTimeBeforeStart, shakeForce));
    }

    static IEnumerator TriggerShakeCoroutine(float tTimeBeforeStart, float shakeForce)
    {
        yield return new WaitForSecondsRealtime(tTimeBeforeStart);

        C_Main.Instance.GetComponent<C_Camera>().AddShake(shakeForce);

        yield break;
    }


    //ANIMATION TRIGGER TAG
    public static void TriggerAnimationsFromTags(float tTimeBeforeStart, string[] tags)
    {
        C_Main.Instance.StartCoroutine(TriggerAnimationsFromTagsCoroutine(tTimeBeforeStart, tags));
    }

    static IEnumerator TriggerAnimationsFromTagsCoroutine(float tTimeBeforeStart, string[] tags)
    {
        yield return new WaitForSecondsRealtime(tTimeBeforeStart);

        List<Animator> animTag = new List<Animator>();
        Animator anim;
        GameObject[] goTab;

        foreach (string tag in tags)
        {
            goTab = GameObject.FindGameObjectsWithTag(tag);

            foreach(GameObject go in goTab)
            {
                anim = go.GetComponent<Animator>();
                if(anim != null)
                {
                    animTag.Add(anim);
                }
               
            }
        }

        foreach(Animator anima in animTag)
        {
            anima.SetTrigger("MakeAction");
        }

        yield break;
    }

    //ANIMATION TRIGGER W/O TAGS
    public static void TriggerAnimations(float tTimeBeforeStart, Animator[] anims)
    {
        C_Main.Instance.StartCoroutine(TriggerAnimationsCoroutine(tTimeBeforeStart, anims));
    }

    static IEnumerator TriggerAnimationsCoroutine(float tTimeBeforeStart, Animator[] anims)
    {
        yield return new WaitForSecondsRealtime(tTimeBeforeStart);

        foreach (Animator anima in anims)
        {
            anima.SetTrigger("MakeAction");
        }

        yield break;
    }
}
