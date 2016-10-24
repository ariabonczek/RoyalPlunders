using UnityEngine;
using System.Collections;

public class TestCatchScript : MonoBehaviour {

    public GameObject ReturnPoint;

    public GameObject player;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (player && ReturnPoint && (transform.position - player.transform.position).magnitude < 2)
        {
            player.transform.position = ReturnPoint.transform.position;
            GetComponent<GuardAITest>().myState = GuardAITest.AIState.Patrolling;
        }
	}
}
