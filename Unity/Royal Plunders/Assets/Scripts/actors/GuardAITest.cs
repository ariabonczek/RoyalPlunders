using UnityEngine;
using System.Collections;

public class GuardAITest : MonoBehaviour {

    NavMeshAgent agent;

    public GameObject player;

    public GameObject pathPoints;

    public float angleOfView;

    public float sneakWalkDetectionRange;

    public float slowWalkDetectionRange;

    public float fastWalkDetectionRange;

    public float runDetectionRange;

    // the distance at which the guard will chase the player
    public float PlayerSpotDistance;

    // the distance from their original path destination where the guard will cut off a chase.
    public float MaxDistanceFromPoint;

    // the points for the guards patrol
    private Vector3[] pathLocations;

    public enum AIState {Chasing, Suspcious,Patrolling};

    public AIState myState;

    private bool suspiciousPrev;

    private Vector3 suspicionPoint;

    private int destPoint = 0;

    // ray used for player detection checks
    private Ray ray;

    // raycast hit used for player detection checks
    private RaycastHit hit;

    // Use this for initialization
    void Start () {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        myState = AIState.Patrolling;
        suspiciousPrev = false;
        // setting up the array of points to use as the patrol
        if(pathPoints)
        {
            pathLocations = new Vector3[pathPoints.transform.childCount];
            for (int i = 0; i < pathPoints.transform.childCount; i++)
            {
                Transform Go = pathPoints.transform.GetChild(i);
                pathLocations[i] = Go.position;
            }
            agent.destination = pathLocations[destPoint];
        }

    }
	
	// Update is called once per frame
	void Update () {

        DetermineAIState();

        ExecuteAIStateResult();
   
    }

    //
    // This segment covers the proper resulting actions for the AI to take given its current state
    //
    void ExecuteAIStateResult()
    {
        // while in the state of chasing the player
        if (myState == AIState.Chasing)
        {
            transform.GetChild(0).position = transform.position + new Vector3(0, 2, 0);
            transform.GetChild(2).position = transform.position + new Vector3(0, -100, 0);
            transform.GetChild(1).position = transform.position + new Vector3(0, -100, 0);
            agent.destination = player.transform.position;
        }
        else if (myState == AIState.Suspcious)
        {
            transform.GetChild(1).position = transform.position + new Vector3(0, 2, 0);
            transform.GetChild(0).position = transform.position + new Vector3(0, -100, 0);
            transform.GetChild(2).position = transform.position + new Vector3(0, -100, 0);
            if (!suspiciousPrev)
            {
                suspicionPoint = player.transform.position;
            }
            agent.destination = suspicionPoint;
        }
        else
        {
            transform.GetChild(2).position = transform.position + new Vector3(0, 2, 0);
            transform.GetChild(0).position = transform.position + new Vector3(0, -100, 0);
            transform.GetChild(1).position = transform.position + new Vector3(0, -100, 0);
            agent.destination = pathLocations[destPoint];
        }

        // proceed to next patrol point when in range of current point
        if (pathPoints && Vector3.Distance(transform.position, pathLocations[destPoint]) < 1)
        {
            GotoNextPoint();
        }
        suspiciousPrev = (myState == AIState.Suspcious);
    }

    //
    // This function covers the decision process for the AI entering its states
    //
    void DetermineAIState()
    {
        Debug.DrawLine(this.transform.position, player.transform.position);
        // chase the player if they are in range and you are not too far from your patrol route
        if (player && PlayerInView() && Vector3.Distance(transform.position, pathLocations[destPoint]) <= MaxDistanceFromPoint)
        {
            // doing a line cast check to see if there are any obstacles between the AI and the player
            ray = new Ray(transform.position, player.transform.position - transform.position);

            if (Physics.Raycast(ray, out hit, PlayerSpotDistance))
            {
                GameObject targetObj = hit.collider.gameObject;
                if (targetObj == player || (targetObj.transform.parent && targetObj.transform.parent.gameObject == player))
                {
                    myState = AIState.Chasing;
                }
            }
            return;
        }
        else if (Vector3.Distance(transform.position, pathLocations[destPoint]) > MaxDistanceFromPoint)
        {
            myState = AIState.Patrolling;
        }

        // chase the player if they are in range and making a loud enough sound
        NoiseMakerScript noiseScript = player.GetComponent<NoiseMakerScript>();
        if (noiseScript)
        {
                if (Vector3.Distance(transform.position, pathLocations[destPoint]) <= MaxDistanceFromPoint)
                {
                    switch (noiseScript.GetSoundLevel())
                    {
                        case 1:
                            if (Vector3.Distance(transform.position, player.transform.position) < sneakWalkDetectionRange && myState != AIState.Chasing)
                                myState = AIState.Suspcious;
                            break;
                        case 2:
                            if (Vector3.Distance(transform.position, player.transform.position) < slowWalkDetectionRange && myState != AIState.Chasing)
                                myState = AIState.Suspcious;
                            break;
                        case 3:
                            if (Vector3.Distance(transform.position, player.transform.position) < fastWalkDetectionRange && myState != AIState.Chasing)
                                myState = AIState.Suspcious;
                            break;
                        case 4:
                            if (Vector3.Distance(transform.position, player.transform.position) < runDetectionRange && myState != AIState.Chasing)
                                myState = AIState.Suspcious;
                            break;
                        default:

                            break;
                    }
                }
                if(Vector3.Distance(transform.position, suspicionPoint) < 2 && myState ==AIState.Suspcious)
                {
                    myState = AIState.Patrolling;
                }
        }
    }

    void GotoNextPoint()
    {
        // Returns if no points have been set up
        if (pathLocations.Length==0)
            return;

        // iterating through points on the patrol
        destPoint = (destPoint + 1) % pathLocations.Length;
       
        // Set the agent to go to the currently selected destination.
        agent.destination = pathLocations[destPoint];
    }

    bool PlayerInView()
    {
        if(player)
        {
            if (Vector3.Distance(transform.position, player.transform.position) > PlayerSpotDistance)
                return false;

            // using the dot product formular where dot = |a||b|cos(theta) and solving for theta
            Vector2 forward = new Vector2(transform.forward.x, transform.forward.z);
            Vector2 toPlayer = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.z- transform.position.z);

            // the forward is already normalized, and by normalizing toPlayer, we eliminate the magnitude part of the equation
            float dot = Vector2.Dot(forward.normalized, toPlayer.normalized);

            // now we have cos(theta) for dot, we just need to get the arc cosine to get theta.
            if(dot>0 && Mathf.Acos(dot) <=(Mathf.Deg2Rad*angleOfView/2))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        return false;
    }
}
