using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegScript : MonoBehaviour
{

    //[SerializeField]
    GameObject hAimedPoint = null;
    //[SerializeField]
    GameObject hCurrentPoint = null;
    //[SerializeField]
    GameObject hRayCastPoint = null;
    [SerializeField]
    StepData sdStepData = null;

    float fDistanceForStep = .8f;
    float fStepAdvance = .3f;
    float fRandomStartPos = .7f;

    float fTimeToStep = 0.2f;
    float fCurrentStepPurcentage = 0;

    float fRotateWhileStepMultiplier = 20;

    bool bTakingAStep = false;

    Quaternion qCurrentRot = Quaternion.identity;

    // Start is called before the first frame update
    void Start()
    {
        hAimedPoint = new GameObject();
        hCurrentPoint = new GameObject();
        hRayCastPoint = new GameObject();
        hRayCastPoint = Instantiate(hRayCastPoint, transform.parent);

        hAimedPoint = Instantiate(hAimedPoint, transform.parent);
        hAimedPoint.transform.position = transform.position + transform.forward * 1;
        hAimedPoint.transform.position = new Vector3(hAimedPoint.transform.position.x, transform.parent.position.y, hAimedPoint.transform.position.z);

        hRayCastPoint.transform.position = hAimedPoint.transform.position;
        hAimedPoint.transform.position = transform.position + transform.forward * 1;

        hCurrentPoint = Instantiate(hCurrentPoint);
        hCurrentPoint.transform.position = hAimedPoint.transform.position + new Vector3(Random.Range(-fRandomStartPos, fRandomStartPos), transform.parent.position.y, Random.Range(-fRandomStartPos, fRandomStartPos));
        qCurrentRot = transform.rotation;
    }

    void UpdateAimedPos()
    {
        transform.rotation = qCurrentRot;
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(hRayCastPoint.transform.position - transform.position), out hit, 1))
        {
            if (hit.collider)
                hAimedPoint.transform.position = hit.point;

            Debug.Log(hit.transform.gameObject.name);
        }
        else
        {
            hAimedPoint.transform.position = hRayCastPoint.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {

        //UpdateAimedPos();
        if (Vector3.Distance(hAimedPoint.transform.position, hCurrentPoint.transform.position) > fDistanceForStep)
        {
            hCurrentPoint.transform.LookAt(hAimedPoint.transform, Vector3.up);
            hCurrentPoint.transform.position = hAimedPoint.transform.position + hCurrentPoint.transform.forward * fStepAdvance;
            fCurrentStepPurcentage = 0;
            bTakingAStep = true;
            qCurrentRot = transform.rotation;
        }

        Vector3 vPos = hCurrentPoint.transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(vPos - transform.position, Vector3.up);

        if (!bTakingAStep)
        {
            transform.rotation = targetRotation;
        }
        else
        {
            fCurrentStepPurcentage += Time.deltaTime * 1 / fTimeToStep;
            transform.rotation = Quaternion.Lerp(qCurrentRot, targetRotation, fCurrentStepPurcentage);
            transform.Rotate(-sdStepData.StepCurve.Evaluate(fCurrentStepPurcentage) * fRotateWhileStepMultiplier, 0, 0, Space.Self);
            if (fCurrentStepPurcentage >= 1) 
            {
                bTakingAStep = false;
            }
        }

    }
}
