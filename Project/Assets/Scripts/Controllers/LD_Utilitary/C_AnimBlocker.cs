using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_AnimBlocker : MonoBehaviour
{
    public bool isBlocked = true;

    bool played = false;
    [SerializeField]
    bool startsNextSequenceOnUnlock = false;

    [SerializeField]
    List<C_GravityAffected> blockers;

    void Start()
    {
        blockers = new List<C_GravityAffected>();

        isBlocked = CheckBlock();
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.layer != 12)
        {
            C_GravityAffected affect = other.GetComponent<C_GravityAffected>();

            if (!blockers.Contains(affect))
            {
                blockers.Add(affect);
            }

            isBlocked = CheckBlock();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != 12)
        {
            C_GravityAffected affect = other.GetComponent<C_GravityAffected>();

            if (blockers.Contains(affect))
            {
                blockers.Remove(affect);
            }

            isBlocked = CheckBlock();

            if (startsNextSequenceOnUnlock && !played && !isBlocked)
            {
                C_SequenceHandler.Instance.NextSequence();
                played = true;
            }

        }

    }

    bool CheckBlock()
    {
        return (blockers.Count != 0);
    }
}
