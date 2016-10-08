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
                if((guardScript.player.transform.position-transform.position).magnitude<1.5 && respawnPosition)
                {
                    guardScript.player.transform.position = respawnPosition.position;
                    guardScript.myState = GuardAITest.AIState.Patrolling;
                }
            }
        }
    }
}
