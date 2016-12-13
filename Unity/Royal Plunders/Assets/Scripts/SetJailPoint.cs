using UnityEngine;
using System.Collections;

public class SetJailPoint : MonoBehaviour 
{
    public Transform respawnPosition; // where to respawn

    public GameObject alarmSystem; // the alarm system

    void Update()
    {
        GuardAITest guardScript = GetComponent<GuardAITest>(); // get the guard component
        if (guardScript) // if the component was found
        {
            if(guardScript.myState == GuardAITest.AIState.Chasing) // if the guard is chasing
            {
                // if the player is catchable
                if((guardScript.player.transform.position-transform.position).magnitude<1.5 && !IsInFront(guardScript.player.transform) && respawnPosition)
                {
                    Reset(); // reset the world
                    guardScript.player.transform.position = respawnPosition.position; // place the player back at spawn position

                    // if I am able to escort the target, do so
                    if(guardScript.myTarget)
                    {
                        guardScript.myState = GuardAITest.AIState.Escorting; // I am now escorting
                        alarmSystem.GetComponent<AlarmSystem>().ResetAlarm(); // and I reset the alarm
                    }
                    else // else do your duties
                    {
                        guardScript.myState = GuardAITest.AIState.Patrolling;
                    }
                }
            }
        }
    }

    // a simple dot product check for facing direction
    // returns false if in front of the guard
    private bool IsInFront(Transform playerTransform)
    {
        Vector3 displacement = playerTransform.position - transform.position;
        if (Vector3.Dot(displacement, transform.forward) > 0)
            return false;
        else
            return true;

    }

    // resets the game world
    private void Reset()
    {
        GameObject[] obj = FindObjectsOfType<GameObject>(); // get all the game objects
        foreach (GameObject g in obj) // and iterate over them
        {
            if (g.GetComponent<GuardAITest>()) // reset guards
                g.GetComponent<GuardAITest>().Reset();

            if (g.GetComponent<Chest>()) // reset chests
                g.GetComponent<Chest>().Reset();

            if (g.GetComponent<Holdable>()) // reset holdables
                g.GetComponent<Holdable>().Reset();

            if (g.GetComponent<TargetAITest>()) // reset targets
                g.GetComponent<TargetAITest>().Reset();

            if (g.GetComponent<GadgetManager>()) // reset gadget managers
                g.GetComponent<GadgetManager>().Reset();

            // destroy all deployed gadgets
            if (g.tag == "Cake" || g.tag == "Jazz" || g.tag == "Electro" || g.tag == "Popsicle" || g.tag == "NoiseMachine")
                Destroy(g);
        }
    }
}
