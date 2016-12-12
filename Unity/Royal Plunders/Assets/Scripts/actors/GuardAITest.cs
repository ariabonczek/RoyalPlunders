using UnityEngine;
using System.Collections;

public class GuardAITest : MonoBehaviour {

    // this guard's nav agent
    NavMeshAgent agent;

    // the player
    public GameObject player;

    // the containing object for all the locations that makup this guards patrol route
    public GameObject pathPoints;

    // the starting location used for resetting
    private Vector3 originalPosition;

    // the starting forward used for resetting
    private Vector3 originalForward;

    // the target this guard is set to protect
    public TargetAITest myTarget;

    // variables to track chase time, allowing guards to have a minumum amount of time chasing the player
    public float minimumChaseTime;

    private float currentChaseTime;

    // the multiplier applied to the guard's speed when pursuing the player
    public float chaseSpeedMultiplier;

    // the distance a guard will veer from the target when escorting them back
    public float PlayerAggroDistanceWhileEscorting;

    // the speed that a guard will rotate to scan an environment
    public float ScanRotationSpeed;

    // the distance at which guards will alert nearby guards if they are in chase mode
    public float GuardToGuardAlertRadius;

    // the original point a guard was at if they are chasing a player away during an escort
    private Vector3 pointAtChaseAway;

    // tracking if the guard can use sound detection at this time
    private bool canHear;

    // the angle of view that the guard will rotate to the left and right to scan
    public int AngleOfScan;

    // Whether the guard will scan at each stop on their patrol
    public bool ScanAtWaypoints;

    // The three variables used to track if a guard has been distracted so recently that they temporarily cannot be again
    private bool canBeDistracted;

    public float DistractionCooldown;

    private float currentDistractionCooldown;

    // whether guards should scan from whatever direction they are facing when they reach each patrol waypoint or use the waypoint's direction
    public bool useWaypointDirectionForScan;

    // the two variables tracking how long a guard is stunned for
    public float StunnedDuration;

    private float currentStunnedDuration;

    //the two variables tracking how long a guard is sleeping for 
    public float SleepingDuration;

    private float currentSleepingDuration;

    //the two variables tracking how long a guard is distracted
    public float DistractedDuration;

    private float currentDistractedDuration;

    // this variable is used by the distracted, sleeping, and stunned states to keep the guard from moving normally in those states
    private bool disabled;

    // the location the guard needs to escort the target back to
    private Vector3 escortLocation;
     
    // the location used as a singular "patrol point" if none are actually set
    private Vector3 basePoint;

    // the angle a guard can see from their forward to the left and right
    public float angleOfView;

    // these variables cover the range at which a player if they are moving at each respective speed
    public float sneakWalkDetectionRange;

    public float walk1DetectionRange;

    public float walk2DetectionRange;

    public float walk3DetectionRange;

    public float walk4DetectionRange;

    public float runDetectionRange;

    public float playerTriggerNoiseRange;

    // this variable is used to track how long a guard will keep up a chase if a player has disappeared from view
    public float playerOutOfSightTimer;

    // the distance at which the guard will chase the player
    public float PlayerSpotDistance;

    // the distance at which the guard will see a cake to seek out
    public float CakeSpotDistance;

    // the distance from their original path destination where the guard will cut off a chase.
    public float MaxDistanceFromPoint;

    // the points for the guards patrol
    private Vector3[] pathLocations;

    // the enum that covers the guard's base AI state machine
    // Chasing- Aware of the player and moving to detain them actively until they are out of range and have pursued long enough or if the player goes out of range for too long
    // Suspicious- Heard a sound at a certain location and is moving to the location of the sound
    // Patrolling- Moving between patrol points unaware of player presence entirely
    // Escorting- The targ et has seen the player and come to this guard, who will escort them back while keeping an eye out for the player
    // Stunned- Temporarily rendered incapacitated by the player. Cannot move, hear, or see the player 
    // Sleeping- Functionality equivalent to stunned, but can be activated usign a seperate means and differing timer
    // Distracted- Intrigued by something and will deviate from, the normal patrol. Can still hear or see the player
    // Scanning- Looking to the left and right in search of the player, but still in patrol mode. Triggered at interval in patrol
    // Caking- Have arrived at cake and is eating it.
    public enum AIState {Chasing, Suspcious,Patrolling,Escorting,Stunned,Sleeping,Distracted,Scanning,Caking};

    // the enum for the scanning states which delineate whether the guard is checking left or right.
    private enum ScanningState {TurningRight,TurningLeft};

    // the actual scanning state for the guard
    private ScanningState myScan;

    // the actual AI State fort he guard
    public AIState myState;

    // the previous AI state for the guard to use for transition logic
    private AIState prevState;

    // the direction the scan is toward
    private Vector3 scanForward;

    // the current amount of time the player has been out of sight (used for losing line of sight when the player goes out of range
    private float playerOutOfSightCurrent;

    // something a guard will seek to investigate (i.e. scan towards)
    private Vector3 scanTarget;

    // the range at which a guard will go into suspicion mode if they see the player (if they are not so close as to initiate chase)
    private bool suspicionRange;

    // the previous state for suspicion
    private bool suspiciousPrev;

    // the base speed for the guard, used to track it when the guard increases in speed for chases
    private float originalSpeed;

    // Whether the guard is currently scanning
    private bool suspiciousScan;

    // the reference to the current cake target
    private GameObject cakeTarget;

    // the source of the sound that the guard is investigating
    public Vector3 suspicionPoint;

    // the index tracker for the current point in patrols being tracked
    private int destPoint = 0;

    // ray used for player detection checks
    private Ray ray;

    // raycast hit used for player detection checks
    private RaycastHit hit;

    // Use this for initialization
    void Start () {
        playerOutOfSightCurrent = 0;
        if (playerOutOfSightTimer <= 0)
            playerOutOfSightTimer = .1f;
        disabled = false;
        currentDistractedDuration = 0;
        currentSleepingDuration = 0;
        currentStunnedDuration = 0;
        originalPosition = transform.position;
        originalSpeed = GetComponent<NavMeshAgent>().speed;
        suspiciousScan = false;
        agent = GetComponent<NavMeshAgent>();
        suspicionRange = false;
        originalForward = transform.forward;
        cakeTarget = null;
        agent.autoBraking = false;
        myState = AIState.Patrolling;
        canHear = true;
        suspiciousPrev = false;
        canBeDistracted = true;
        currentDistractionCooldown = 0;
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

        CanHear();
       
        RunDistractionCooldownTimer();

        AssessNegativeStatuses();

        DetermineAIState();

        ExecuteAIStateResult();

        if(myState == AIState.Chasing)
        {
            GetComponent<NavMeshAgent>().speed = originalSpeed * chaseSpeedMultiplier;
        }
        else
        {
            GetComponent<NavMeshAgent>().speed = originalSpeed;
        }
   
    }

    public void Stun()
    {
        if (myState == AIState.Chasing)
            return;
        if (myState != AIState.Stunned && myState != AIState.Distracted && myState != AIState.Sleeping)
            prevState = myState;
        myState = AIState.Stunned;
    }

    public void Sleep()
    {
        if (myState != AIState.Stunned && myState != AIState.Distracted && myState != AIState.Sleeping)
            prevState = myState;
        myState = AIState.Sleeping;
    }

    public void Distract()
    {
        if (canBeDistracted && myState != AIState.Stunned && myState != AIState.Escorting)
        {
            if (myState != AIState.Stunned && myState != AIState.Distracted && myState != AIState.Sleeping)
                prevState = myState;
            myState = AIState.Distracted;
        }
    }

    private void CanHear()
    {
        canHear = true;
        GameObject[] obj = GameObject.FindGameObjectsWithTag("Jazz");
        foreach (GameObject g in obj)
        {
            if (g.GetComponent<Jazz>().InRange(this.gameObject) && myState == AIState.Distracted)
            {
                Debug.Log("OH HEY CXOOL");
                canHear = false;
            }
        }
    }

    private void RotateTowards(Vector3 direction)
    {
       Quaternion lookRotation = Quaternion.LookRotation(direction);
       transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * ScanRotationSpeed);
    }

    private void RunDistractionCooldownTimer()
    {
        if(!canBeDistracted)
        {
            currentDistractedDuration += Time.deltaTime;
            if(currentDistractionCooldown>= DistractionCooldown)
            {
                canBeDistracted = true;
                currentDistractionCooldown = 0;
            }
        }
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
                    transform.GetChild(3).position = transform.position + new Vector3(0, -100, 0);
                    transform.GetChild(0).position = transform.position + new Vector3(0, -100, 0);
                    transform.GetChild(2).position = transform.position + new Vector3(0, -100, 0);
                    transform.GetChild(1).position = transform.position + new Vector3(0, -100, 0);
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
                if (currentDistractedDuration > DistractedDuration)
                {
                    currentDistractedDuration = 0;

                    if (prevState != AIState.Distracted)
                        myState = prevState;
                    else
                        myState = AIState.Patrolling;
                    if (cakeTarget)
                    {
                        myState = AIState.Patrolling;
                        Destroy(cakeTarget);
                    }
                    canBeDistracted = false;
                    agent.Resume();
                }
                else
                {
                    transform.GetChild(3).position = transform.position + new Vector3(0, 2, 0);
                    transform.GetChild(0).position = transform.position + new Vector3(0, -100, 0);
                    transform.GetChild(2).position = transform.position + new Vector3(0, -100, 0);
                    transform.GetChild(1).position = transform.position + new Vector3(0, -100, 0);
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
            currentChaseTime += Time.deltaTime;
            agent.Resume();
            transform.GetChild(0).position = transform.position + new Vector3(0, 2, 0);
            transform.GetChild(2).position = transform.position + new Vector3(0, -100, 0);
            transform.GetChild(1).position = transform.position + new Vector3(0, -100, 0);
            transform.GetChild(3).position = transform.position + new Vector3(0, -100, 0);
            agent.destination = player.transform.position;
            // if the target is with the guard then after a certain distance the guard will return to escorting them
            if (myTarget && (Vector3.Distance(transform.position, player.transform.position) > PlayerAggroDistanceWhileEscorting + 2))
            {
                myState = AIState.Escorting;
            }
            AlertNearbyGuards();
            //doing a raycast check to see if the player is out of view
            if (CanSeePlayer())
            {
                playerOutOfSightCurrent = 0;
            }
            else
            {
                playerOutOfSightCurrent += Time.deltaTime;
            }

            if(playerOutOfSightCurrent >= playerOutOfSightTimer)
            {
                myState = AIState.Suspcious;
                playerOutOfSightCurrent = 0;
                suspicionPoint = player.transform.position;
            }
        }
        else if (myState == AIState.Suspcious)
        {
            agent.Resume();
            transform.GetChild(1).position = transform.position + new Vector3(0, 2, 0);
            transform.GetChild(0).position = transform.position + new Vector3(0, -100, 0);
            transform.GetChild(2).position = transform.position + new Vector3(0, -100, 0);
            transform.GetChild(3).position = transform.position + new Vector3(0, -100, 0);
            if (!suspiciousPrev)
            {
                suspicionPoint = player.transform.position;
            }
            agent.destination = suspicionPoint;
            playerOutOfSightCurrent = 0;
        }
        else if (myState == AIState.Patrolling)
        {
            agent.Resume();
            transform.GetChild(2).position = transform.position + new Vector3(0, 2, 0);
            transform.GetChild(0).position = transform.position + new Vector3(0, -100, 0);
            transform.GetChild(1).position = transform.position + new Vector3(0, -100, 0);
            transform.GetChild(3).position = transform.position + new Vector3(0, -100, 0);
            agent.destination = GetNextPoint();
            if (myTarget)
            {
                myState = AIState.Escorting;
            }
            playerOutOfSightCurrent = 0;
        }
        else if (myState == AIState.Escorting)
        {
            agent.Resume();
            transform.GetChild(2).position = transform.position + new Vector3(0, 2, 0);
            transform.GetChild(0).position = transform.position + new Vector3(0, -100, 0);
            transform.GetChild(1).position = transform.position + new Vector3(0, -100, 0);
            transform.GetChild(3).position = transform.position + new Vector3(0, -100, 0);
            if (escortLocation == Vector3.zero)
            {
                escortLocation = myTarget.GetComponent<TargetAITest>().GetTargetBasePosition();
            }
            agent.destination = escortLocation;
            if (Vector3.Distance(transform.position, player.transform.position) < PlayerAggroDistanceWhileEscorting)
            {
                pointAtChaseAway = transform.position;
                StartChasing();
            }
            if (Vector3.Distance(transform.position, escortLocation) < 1.2f)
            {
                myTarget.myState = TargetAITest.AIState.Unaware;
                myTarget = null;
                escortLocation = Vector3.zero;
                myState = AIState.Patrolling;
            }
            playerOutOfSightCurrent = 0;
        }
        else if (myState == AIState.Caking)
        {
            agent.Resume();
            transform.GetChild(3).position = transform.position + new Vector3(0, 2, 0);
            transform.GetChild(0).position = transform.position + new Vector3(0, -100, 0);
            transform.GetChild(2).position = transform.position + new Vector3(0, -100, 0);
            transform.GetChild(1).position = transform.position + new Vector3(0, -100, 0);
            if (!cakeTarget)
            {
                myState = AIState.Patrolling;
                return;
            }
            agent.destination = cakeTarget.transform.position;
            if (Vector3.Distance(transform.position, cakeTarget.transform.position) < 1f)
            {
                agent.Stop();
                Distract();
            }
            playerOutOfSightCurrent = 0;
        }

        // proceed to next patrol point when in range of current point
        Vector3 diffVec = (GetNextPoint() - transform.position);
        diffVec.y = 0;
        if (diffVec.magnitude < 1.0f && myState == AIState.Patrolling)
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
        if(myState != AIState.Stunned && myState != AIState.Distracted && myState != AIState.Sleeping)
            prevState = myState;
        myState = AIState.Scanning;
        if (useWaypointDirectionForScan && !suspiciousScan)
        {
            if (!pathPoints)
            {
                scanForward = originalForward;
            }
            else
            {
                scanForward = pathPoints.transform.GetChild(destPoint).transform.forward;
            }
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

        // check to see if they notice a cake
        if (CakeInView() && myState != AIState.Chasing && myState != AIState.Escorting)
        {
            //prevState = myState;
            myState = AIState.Caking;
            return;
        }

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
                        StartChasing();
                }
            }
            return;
        }
        else if (Vector3.Distance(transform.position, GetNextPoint()) > MaxDistanceFromPoint && myState != AIState.Escorting && (myState != AIState.Chasing || currentChaseTime >= minimumChaseTime))
        {
            myState = AIState.Patrolling;
        }

        // chase the player if they are in range and making a loud enough sound
        NoiseMakerScript noiseScript = player.GetComponent<NoiseMakerScript>();
        if (noiseScript && canHear && myState != AIState.Escorting)
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
                            if (Vector3.Distance(transform.position, player.transform.position) < walk1DetectionRange && myState != AIState.Chasing)
                                myState = AIState.Suspcious;
                            break;
                        case 3:
                            if (Vector3.Distance(transform.position, player.transform.position) < walk2DetectionRange && myState != AIState.Chasing)
                                myState = AIState.Suspcious;
                            break;
                        case 4:
                            if (Vector3.Distance(transform.position, player.transform.position) < walk3DetectionRange && myState != AIState.Chasing)
                                myState = AIState.Suspcious;
                            break;
                        case 5:
                            if (Vector3.Distance(transform.position, player.transform.position) < walk4DetectionRange && myState != AIState.Chasing)
                                myState = AIState.Suspcious;
                            break;
                        case 6:
                            if (Vector3.Distance(transform.position, player.transform.position) < runDetectionRange && myState != AIState.Chasing)
                                myState = AIState.Suspcious;
                            break;
                        case 7:
                            if (Vector3.Distance(transform.position, player.transform.position) < playerTriggerNoiseRange && myState != AIState.Chasing)
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

    public void HearsSound(GameObject obj)
    {
        if (canHear && myState != AIState.Escorting)
        {
            myState = AIState.Suspcious;
            suspicionPoint = obj.transform.position;
        }
    }

    bool CanSeePlayer()
    {
        // doing a line cast check to see if there are any obstacles between the AI and the player
        ray = new Ray(transform.position, player.transform.position - transform.position);

        if (Physics.Raycast(ray, out hit, PlayerSpotDistance))
        {
            GameObject targetObj = hit.collider.gameObject;
            if (targetObj == player || (targetObj.transform.parent && targetObj.transform.parent.gameObject == player))
            {
                return true;
            }
        }
        return false;
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

    void AlertNearbyGuards()
    {
        var objects = GameObject.FindGameObjectsWithTag("Guard");
        foreach (var obj in objects)
        {
            GameObject go = obj.gameObject;
            if(Vector3.Distance(transform.position, go.transform.position)<GuardToGuardAlertRadius)
            {
                go.GetComponent<GuardAITest>().StartChasing();
            }
        }
    }

    bool CakeInView()
    {
        GameObject[] obj = GameObject.FindGameObjectsWithTag("Cake");
        foreach (GameObject g in obj)
        {
            if (Vector3.Distance(transform.position, g.transform.position) > CakeSpotDistance * 2)
                return false;

            if (Vector3.Distance(transform.position, g.transform.position) > CakeSpotDistance)
                suspicionRange = true;
            else
                suspicionRange = false;

            // using the dot product formular where dot = |a||b|cos(theta) and solving for theta
            Vector2 forward = new Vector2(transform.forward.x, transform.forward.z);
            Vector2 toPlayer = new Vector2(g.transform.position.x - transform.position.x, g.transform.position.z - transform.position.z);

            // the forward is already normalized, and by normalizing toPlayer, we eliminate the magnitude part of the equation
            float dot = Vector2.Dot(forward.normalized, toPlayer.normalized);

            // now we have cos(theta) for dot, we just need to get the arc cosine to get theta.
            if (dot > 0 && Mathf.Acos(dot) <= (Mathf.Deg2Rad * angleOfView / 2))
            {
                // doing a line cast check to see if there are any obstacles between the AI and the player
                ray = new Ray(transform.position, g.transform.position - transform.position);

                if (Physics.Raycast(ray, out hit, CakeSpotDistance))
                {
                    GameObject targetObj = hit.collider.gameObject;
                    if (targetObj == g || (targetObj.transform.parent && targetObj.transform.parent.gameObject == g))
                    {
                        cakeTarget = g;
                        return true;
                    }
                    else
                    {
                        Debug.Log("CAKE OBSTAKLE: " + targetObj);
                    }
                }
                return false;
            }
            else
            {

                return false;
            }

        }
        return false;
    }

    public void StartChasing()
    {
        if(myState != AIState.Chasing)
            currentChaseTime = 0;
        myState = AIState.Chasing;
    }

    public void Reset()
    {
        myState = AIState.Patrolling;
        transform.forward = originalForward;
        transform.position = originalPosition;
        destPoint = 0;
    }
}
