using UnityEngine;
using System.Collections;

public class TargetAITest : MonoBehaviour {

    NavMeshAgent agent;

    public GameObject player;

    public float angleOfView;

    private Vector3 basePosition;

    private Vector3 originalPosition;

    private Vector3 originalForward;

    public float sneakWalkDetectionRange;

    public float slowWalkDetectionRange;

    public float fastWalkDetectionRange;

    public float runDetectionRange;

    public float rotationSpeed;

    public enum AIState { Running, Suspcious, Unaware, WaitingForSafety, BeingEscorted };

    public AIState myState;

    private bool suspicionRange;

    private GameObject alertedGuard;

    // the distance at which the guard will chase the player
    public float PlayerSpotDistance;

    private Vector3 suspicionPoint;

    // ray used for player detection checks
    private Ray ray;

    // raycast hit used for player detection checks
    private RaycastHit hit;


    // Use this for initialization
    void Start () {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        originalForward = transform.forward;
        originalPosition = transform.position;
        myState = AIState.Unaware;
        basePosition = this.transform.position;
    }

    // Update is called once per frame
    void Update () {
        DetermineAIState();

        ExecuteAIStateResult();
    }

    //
    // This function covers the decision process for the AI entering its states
    //
    void DetermineAIState()
    {
        // chase the player if they are in range and you are not too far from your patrol route
        if (player)
        {
            // doing a line cast check to see if there are any obstacles between the AI and the player
            ray = new Ray(transform.position, player.transform.position - transform.position);

            if (Physics.Raycast(ray, out hit, PlayerSpotDistance) && PlayerInView())
            {
                GameObject targetObj = hit.collider.gameObject;
                if (targetObj == player || (targetObj.transform.parent && targetObj.transform.parent.gameObject == player))
                {
                    if (myState != AIState.WaitingForSafety)
                    {
                        if (suspicionRange)
                            myState = AIState.Suspcious;
                        else
                            myState = AIState.Running;
                        return;
                    }
                }
            }
        }

        if(myState == AIState.Running)
        {
            return;
        }

        // chase the player if they are in range and making a loud enough sound
        NoiseMakerScript noiseScript = player.GetComponent<NoiseMakerScript>();
        if (noiseScript)
        {
            if (myState != AIState.WaitingForSafety)
            {
                switch (noiseScript.GetSoundLevel())
                {
                    case 1:
                        if (Vector3.Distance(transform.position, player.transform.position) < sneakWalkDetectionRange && myState != AIState.Running)
                        {
                            myState = AIState.Suspcious;
                            suspicionPoint = player.transform.position;
                        }
                        break;
                    case 2:
                        if (Vector3.Distance(transform.position, player.transform.position) < slowWalkDetectionRange && myState != AIState.Running)
                        {
                            myState = AIState.Suspcious;
                            suspicionPoint = player.transform.position;
                        }
                        break;
                    case 3:
                        if (Vector3.Distance(transform.position, player.transform.position) < fastWalkDetectionRange && myState != AIState.Running)
                        {
                            myState = AIState.Suspcious;
                            suspicionPoint = player.transform.position;
                        }
                        break;
                    case 4:
                        if (Vector3.Distance(transform.position, player.transform.position) < runDetectionRange && myState != AIState.Running)
                        {
                            myState = AIState.Suspcious;
                            suspicionPoint = player.transform.position;
                        }
                        break;
                    default:

                        break;
                }

                if (Vector3.Distance(transform.position, suspicionPoint) < 2 && myState == AIState.Suspcious)
                {
                    myState = AIState.Unaware;
                }
            }
        }
    }

    //
    // This segment covers the proper resulting actions for the AI to take given its current state
    //
    void ExecuteAIStateResult()
    {
        // while in the state of Running the player
        if (myState == AIState.Running)
        {
            transform.GetChild(0).position = transform.position + new Vector3(0, 2, 0);
            transform.GetChild(2).position = transform.position + new Vector3(0, -100, 0);
            transform.GetChild(1).position = transform.position + new Vector3(0, -100, 0);
            agent.Resume();
            if (!alertedGuard)
                alertedGuard = NearestGuard();
            MoveTowards(alertedGuard.transform.position);

            if(Vector3.Distance(transform.position, alertedGuard.transform.position) <3)
            {
                alertedGuard.GetComponent<GuardAITest>().myState = GuardAITest.AIState.Escorting;
                alertedGuard.GetComponent<GuardAITest>().myTarget = this;
                myState = AIState.WaitingForSafety;
            }
        }
        else if (myState == AIState.Suspcious)
        {
            transform.GetChild(1).position = transform.position + new Vector3(0, 2, 0);
            transform.GetChild(0).position = transform.position + new Vector3(0, -100, 0);
            transform.GetChild(2).position = transform.position + new Vector3(0, -100, 0);

            agent.Stop();
            RotateTowards(suspicionPoint);
        }
        else if (myState == AIState.Unaware)
        {
            transform.GetChild(2).position = transform.position + new Vector3(0, 2, 0);
            transform.GetChild(0).position = transform.position + new Vector3(0, -100, 0);
            transform.GetChild(1).position = transform.position + new Vector3(0, -100, 0);
            agent.Stop();
        }
        else if (myState == AIState.WaitingForSafety)
        {
            transform.GetChild(1).position = transform.position + new Vector3(0, 2, 0);
            transform.GetChild(0).position = transform.position + new Vector3(0, -100, 0);
            transform.GetChild(2).position = transform.position + new Vector3(0, -100, 0);
            agent.Resume();
            agent.destination = alertedGuard.transform.position;
        }
    }

    GameObject NearestGuard()
    {
        Object[] obj = GameObject.FindObjectsOfType(typeof(GameObject));
        float dist = 999;
        GameObject closestGuard = null;
        foreach (object o in obj)
        {
            GameObject g = (GameObject)o;
            if (g.tag == "Guard" && g.GetComponent<GuardAITest>())
            {
                if ((transform.position - g.transform.position).magnitude < dist)
                {
                    closestGuard = g;
                    dist = (transform.position - closestGuard.transform.position).magnitude;
                }
            }
        }
        return closestGuard;
    }

    private void MoveTowards(Vector3 target)
    {
        agent.SetDestination(target);
    }

    private void RotateTowards(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    bool PlayerInView()
    {
        if (player)
        {
            if (Vector3.Distance(transform.position, player.transform.position) > PlayerSpotDistance * 2)
                return false;

            if (Vector3.Distance(transform.position, player.transform.position) > PlayerSpotDistance)
                suspicionRange = true;
            else
                suspicionRange = false;

            // using the dot product formular where dot = |a||b|cos(theta) and solving for theta
            Vector2 forward = new Vector2(transform.forward.x, transform.forward.z);
            Vector2 toPlayer = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.z - transform.position.z);

            // the forward is already normalized, and by normalizing toPlayer, we eliminate the magnitude part of the equation
            float dot = Vector2.Dot(forward.normalized, toPlayer.normalized);

            // now we have cos(theta) for dot, we just need to get the arc cosine to get theta.
            if (dot > 0 && Mathf.Acos(dot) <= (Mathf.Deg2Rad * angleOfView / 2))
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

    public Vector3 GetTargetBasePosition()
    {
        return basePosition;
    }

    public void Reset()
    {
        transform.position = originalPosition;
        transform.forward = originalForward;
        myState = AIState.Unaware;
    }
}
