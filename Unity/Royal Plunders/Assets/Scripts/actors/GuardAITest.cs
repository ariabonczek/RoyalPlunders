using UnityEngine;
using System.Collections;

public class GuardAITest : MonoBehaviour {

    NavMeshAgent agent;

    public GameObject player;

    public GameObject pathPoints;

    public TargetAITest myTarget;

    public float ScanRotationSpeed;

    public int AngleOfScan;

    public bool ScanAtWaypoints;

    public bool useWaypointDirectionForScan;

    public float StunnedDuration;

    private float currentStunnedDuration;

    public float SleepingDuration;

    private float currentSleepingDuration;

    public float DistractedDuration;

    private float currentDistractedDuration;

    private bool disabled;

    private Vector3 escortLocation;
     
    private Vector3 basePoint;

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

    public enum AIState {Chasing, Suspcious,Patrolling,Escorting,Stunned,Sleeping,Distracted,Scanning};

    private enum ScanningState {TurningRight,TurningLeft};

    private ScanningState myScan;

    public AIState myState;

    private AIState prevState;

    private Vector3 scanForward;

    private Vector3 wierd;

    private Vector3 scanTarget;

    private bool suspicionRange;

    private bool suspiciousPrev;

    private bool suspiciousScan;

    private Vector3 suspicionPoint;

    private int destPoint = 0;

    // ray used for player detection checks
    private Ray ray;

    // raycast hit used for player detection checks
    private RaycastHit hit;

    // Use this for initialization
    void Start () {
        disabled = false;
        currentDistractedDuration = 0;
        currentSleepingDuration = 0;
        currentStunnedDuration = 0;
        suspiciousScan = false;
        agent = GetComponent<NavMeshAgent>();
        suspicionRange = false;
        agent.autoBraking = false;
        myState = AIState.Patrolling;
        suspiciousPrev = false;
        escortLocation = Vector3.zero;
        myTarget = null;
        basePoint = transform.position;
        myScan = ScanningState.TurningLeft;
        // setting up the array of points to use as the patrol
        if (pathPoints)
        {
            pathLocations = new Vector3[pathPoints.transform.childCount];
            for (int i = 0; i < pathPoints.transform.childCount; i++)
            {
                Transform Go = pathPoints.transform.GetChild(i);
                pathLocations[i] = Go.position;
            }
            agent.destination = GetNextPoint();
            if(!pathPoints)
            {
                agent.Stop();
            }
        }

    }
	
	// Update is called once per frame
	void Update () {

        disabled = false;

        AssessNegativeStatuses();

        DetermineAIState();

        ExecuteAIStateResult();
   
    }

    public void Stun()
    {
        prevState = myState;
        myState = AIState.Stunned;
    }

    public void Sleep()
    {
        prevState = myState;
        myState = AIState.Sleeping;
    }

    public void Distract()
    {
        prevState = myState;
        myState = AIState.Distracted;
    }

    private void RotateTowards(Vector3 direction)
    {
       Quaternion lookRotation = Quaternion.LookRotation(direction);
       transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * ScanRotationSpeed);
    }

    private Vector3 GetScanTarget(bool leftTarget)
    {
        if(AngleOfScan==0)
        {
            return (transform.position + transform.forward);
        }
        float angle = AngleOfScan;
        if (leftTarget)
            angle = -angle;

        Debug.Log(scanForward);

        Vector3 target = Quaternion.AngleAxis(angle, transform.up) * scanForward;
        return (target);
    } 

    void AssessNegativeStatuses()
    {
        switch(myState)
        {
            case AIState.Scanning:
                agent.Stop();
                RotateTowards(scanTarget);
                if (Vector3.Angle(transform.forward, scanTarget) < 3)
                {
                    if (myScan == ScanningState.TurningLeft)
                    {
                        scanTarget = GetScanTarget(false);
                        myScan = ScanningState.TurningRight;
                    }
                    else
                    {
                        myState = prevState;
                        GotoNextPoint();
                        agent.Resume();
                        suspiciousScan = false;
                        myScan = ScanningState.TurningLeft;
                    }
                }

            break;

            case AIState.Stunned:
                currentStunnedDuration += Time.deltaTime;
                if(currentStunnedDuration>StunnedDuration)
                {
                    currentStunnedDuration = 0;
                    myState = prevState;
                    agent.Resume();
                }
                else
                {
                    disabled = true;
                }
                break;

            case AIState.Sleeping:
                currentSleepingDuration += Time.deltaTime;
                if(currentSleepingDuration>SleepingDuration)
                {
                    currentSleepingDuration = 0;
                    myState = prevState;
                    agent.Resume();
                }
                else
                {
                    disabled = true;
                }
                break;

            case AIState.Distracted:
                currentDistractedDuration += Time.deltaTime;
                if(currentDistractedDuration>DistractedDuration)
                {
                    currentDistractedDuration = 0;
                    myState = prevState;
                    agent.Resume();
                }
                else
                {
                    agent.Stop();
                }
                break;
        }
    }

    //
    // This segment covers the proper resulting actions for the AI to take given its current state
    //
    void ExecuteAIStateResult()
    {

        // exiting if this AI is temporarily disabled
        if (disabled)
        {
            agent.Stop();
            return;
        }

        // while in the state of Chasing the player
        if (myState == AIState.Chasing)
        {
            agent.Resume();
            transform.GetChild(0).position = transform.position + new Vector3(0, 2, 0);
            transform.GetChild(2).position = transform.position + new Vector3(0, -100, 0);
            transform.GetChild(1).position = transform.position + new Vector3(0, -100, 0);
            agent.destination = player.transform.position;
        }
        else if (myState == AIState.Suspcious)
        {
            agent.Resume();
            transform.GetChild(1).position = transform.position + new Vector3(0, 2, 0);
            transform.GetChild(0).position = transform.position + new Vector3(0, -100, 0);
            transform.GetChild(2).position = transform.position + new Vector3(0, -100, 0);
            if (!suspiciousPrev)
            {
                suspicionPoint = player.transform.position;
            }
            agent.destination = suspicionPoint;
        }
        else if (myState == AIState.Patrolling)
        {
            transform.GetChild(2).position = transform.position + new Vector3(0, 2, 0);
            transform.GetChild(0).position = transform.position + new Vector3(0, -100, 0);
            transform.GetChild(1).position = transform.position + new Vector3(0, -100, 0);
            agent.destination = GetNextPoint();

            if (myTarget)
            {
                myState = AIState.Escorting;
            }
        }
        else if (myState == AIState.Escorting)
        {
            agent.Resume();
            transform.GetChild(2).position = transform.position + new Vector3(0, 2, 0);
            transform.GetChild(0).position = transform.position + new Vector3(0, -100, 0);
            transform.GetChild(1).position = transform.position + new Vector3(0, -100, 0);
            if (escortLocation == Vector3.zero)
            {
                escortLocation = myTarget.GetComponent<TargetAITest>().GetTargetBasePosition();
            }
            agent.destination = escortLocation;
            if (Vector3.Distance(transform.position, escortLocation) < 2)
            {
                myTarget.myState = TargetAITest.AIState.Unaware;
                myTarget = null;
                escortLocation = Vector3.zero;
                myState = AIState.Patrolling;
            }
        }

        // proceed to next patrol point when in range of current point
        if (Vector3.Distance(transform.position, GetNextPoint()) < 1 && myState == AIState.Patrolling)
        {
            if(ScanAtWaypoints)
            {
                DoScan();
                return;
            }
            GotoNextPoint();
        }
        suspiciousPrev = (myState == AIState.Suspcious);
    }

    private void DoScan()
    {
        prevState = myState;
        myState = AIState.Scanning;
        if (useWaypointDirectionForScan && !suspiciousScan)
        {
            scanForward = pathPoints.transform.GetChild(destPoint).transform.forward;
        }
        else
        {
            scanForward = transform.forward;
        }
        scanTarget = GetScanTarget(true);
        return;
    }

    //
    // This function covers the decision process for the AI entering its states
    //
    void DetermineAIState()
    {
        if (disabled)
            return;

        Debug.DrawLine(this.transform.position, player.transform.position);
        // chase the player if they are in range and you are not too far from your patrol route
        if (player && PlayerInView() && Vector3.Distance(transform.position, GetNextPoint()) <= MaxDistanceFromPoint)
        {
            // doing a line cast check to see if there are any obstacles between the AI and the player
            ray = new Ray(transform.position, player.transform.position - transform.position);

            if (Physics.Raycast(ray, out hit, PlayerSpotDistance))
            {
                GameObject targetObj = hit.collider.gameObject;
                if (targetObj == player || (targetObj.transform.parent && targetObj.transform.parent.gameObject == player))
                {
                    if (suspicionRange && myState != AIState.Chasing)
                        myState = AIState.Suspcious;
                    else
                        myState = AIState.Chasing;
                }
            }
            return;
        }
        else if (Vector3.Distance(transform.position, GetNextPoint()) > MaxDistanceFromPoint && myState != AIState.Escorting)
        {
            myState = AIState.Patrolling;
        }

        // chase the player if they are in range and making a loud enough sound
        NoiseMakerScript noiseScript = player.GetComponent<NoiseMakerScript>();
        if (noiseScript)
        {
                if (Vector3.Distance(transform.position, GetNextPoint()) <= MaxDistanceFromPoint)
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
                    suspiciousScan = true;
                    DoScan();
                }
        }
    }

    Vector3 GetNextPoint()
    {
        if(pathPoints)
        {
            return pathLocations[destPoint];
        }
        else
        {
            return basePoint;
        }
    }

    void GotoNextPoint()
    {
        // Returns if no points have been set up
        if (!pathPoints)
        {
            agent.Stop();
            return;
        }

        // iterating through points on the patrol
        destPoint = (destPoint + 1) % pathLocations.Length;
       
        // Set the agent to go to the currently selected destination.
        agent.destination = pathLocations[destPoint];
    }

    bool PlayerInView()
    {
        if(player)
        {
            if (Vector3.Distance(transform.position, player.transform.position) > PlayerSpotDistance * 2)
                return false;

            if (Vector3.Distance(transform.position, player.transform.position) > PlayerSpotDistance)
                suspicionRange = true;
            else
                suspicionRange = false;

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
