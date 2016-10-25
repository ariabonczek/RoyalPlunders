using UnityEngine;
using System.Collections;

public class SetJailPoint : MonoBehaviour 
{
    public Transform respawnPosition;

    void Update()
    {
        GuardAITest guardScript = GetComponent<GuardAITest>();
        if (guardScript)
        {
            if(guardScript.myState == GuardAITest.AIState.Chasing)
            {
                if((guardScript.player.transform.position-transform.position).magnitude<1.5 && !IsInFront(guardScript.player.transform) && respawnPosition)
                {
                    Reset();
                    guardScript.player.transform.position = respawnPosition.position;
                    if(guardScript.myTarget)
                    {
                        guardScript.myState = GuardAITest.AIState.Escorting;
                    }
                    else
                    {
                        guardScript.myState = GuardAITest.AIState.Patrolling;
                    }
                }
            }
        }
    }

    private bool IsInFront(Transform playerTransform)
    {
        Vector3 displacement = playerTransform.position - transform.position;
        if (Vector3.Dot(displacement, transform.forward) > 0)
            return false;
        else
            return true;

    }

    private void Reset()
    {
        GameObject[] obj = FindObjectsOfType<GameObject>();
        foreach (GameObject g in obj)
        {
            if (g.GetComponent<GuardAITest>())
                g.GetComponent<GuardAITest>().Reset();

            if (g.GetComponent<Chest>())
                g.GetComponent<Chest>().Reset();

            if (g.GetComponent<Holdable>())
                g.GetComponent<Holdable>().Reset();

            if (g.GetComponent<TargetAITest>())
                g.GetComponent<TargetAITest>().Reset();
        }
    }
}
