using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Guard : MonoBehaviour, IPickup
{
    enum GuardState
    {
        Oblivious,
        Investigation,
        Aggro,
        Distracted,
        KnockedOut
    }

    public const float INVESTIGATE_TRANSITION_TIME = 0.75f;
    public const float SAW_PLAYER_TIME = 1.0f;
    public const float FOV_TIME = 2.0f;
    public const float ATTACK_SPEED = 2.6f;
    public const float CHASE_SPEED = 10.0f;

    [SerializeField]
    private GuardState state;
    bool transition = false;

    public bool IsKnockedOut
    {
        get { return state == GuardState.KnockedOut; }
    }

    [SerializeField]
    List<Transform> path;

    [SerializeField]
    bool loopPath = false;

    [SerializeField]
    private bool alerted = false;

    [SerializeField]
    private float viewConeAngle = 45f;

    [SerializeField]
    private float knockoutAngle = 210f;

    public float KnockoutAngle
    {
        get { return knockoutAngle; }
    }

    [SerializeField]
    private float visionRange = 25f;

    [SerializeField]
    private float hearingRange = 10f;

    Quaternion returnToRotation;

    int pathIndex = 0;
    int pathDirection = 1;

    // Looking
    Quaternion passStart;
    Quaternion lookingStart;
    Quaternion goal;

    [SerializeField]
    bool shouldLookAround = false;  
    bool isLookingAround = false;
    bool finishedLook = false;

    [SerializeField]
    private float lookingTimePerDirection = 2.0f;
    private float lookingTimer = 0.0f;

    [SerializeField]
    private float waitTimeBetweenWaypoints = 2.0f;
    float waitTimer;

    private bool sawPlayerRecently = false;
    private float sawPlayerTimer = 0.0f;
    private Quaternion sawPlayerStart;
    private Vector3 lastKnownPlayerPosition;

    // Investigate
    private float investigateTransitionTimer;
    bool pathRequested = false;
    bool isCheckingLastKnownPosition = false;
    private Vector3 lookForPlayerStart;
    bool isReturningToPatrol = false;

    struct AggroTarget
    {
        public IAttackable target;
        public bool stillAlive;
    }
    AggroTarget aggroTarget;
    private float fovTimer = 0.0f;
    private float attackTimer = 0.0f;
    [SerializeField]
    private float attackRange = 5.0f;
    private bool recentlyHit = false;
    private bool recentlySeen = false;

    // KnockedOut
    public const float KNOCKOUT_TIME = 20.0f;
    private float knockoutTimer = 0.0f;
    private bool isHeldByPlayer = false;
    private Transform parentCache;

    // Distracted
    Cake distractionTarget;

    NavMeshAgent agent;
    Material material;
    Rigidbody _rigidbody;

    Color startColor;

    [SerializeField]
    Color investigateColor;

    [SerializeField]
    Color aggroColor;

    [SerializeField]
    Color koColor;

    // Use this for initialization
    void Start()
    {
        state = GuardState.Oblivious;
        agent = GetComponent<NavMeshAgent>();
        material = GetComponent<Renderer>().material;
        _rigidbody = GetComponent<Rigidbody>();
        startColor = material.color;

        if (path.Count > 0)
            agent.SetDestination(path[pathIndex].position);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (state)
        {
            case GuardState.Oblivious:
                UpdateOblivious();
                break;
            case GuardState.Investigation:
                UpdateInvestigation();
                break;
            case GuardState.Aggro:
                UpdateAggro();
                break;
            case GuardState.Distracted:
                UpdateDistracted();
                break;
            case GuardState.KnockedOut:
                UpdateKnockedOut();
                break;
        }

        DebugDrawCones();
    }

    bool IsInCone(Vector3 v)
    {
        float a = Vector3.Angle(transform.forward, (transform.position - v).normalized);
        if (a > 180 - viewConeAngle * 0.5)
        {
            return true;
        }

        return false;
    }

    void UpdateOblivious()
    {
        if (isLookingAround)
        {
            LookAround();
        }
        else
        {
            PathMotion();
        }

        RaycastHit hit;
        Physics.Raycast(transform.position, (Player.instance.transform.position - transform.position).normalized, out hit);

        // If through Raycasting we've found the player...
        if(hit.collider.tag == "Player" && hit.distance <= visionRange && IsInCone(hit.point))
        {
            // If this is the first time we've seen the player in a while...
            if(!sawPlayerRecently)
            {
                agent.Stop();
                agent.velocity = Vector3.zero;
                sawPlayerStart = transform.rotation;
            }
            sawPlayerRecently = true;
        }
        else
        {
            if(sawPlayerRecently)
            {
                agent.Resume();
                transform.rotation = sawPlayerStart;
            }
            sawPlayerRecently = false;
        }

        if (sawPlayerRecently)
        {
            sawPlayerTimer += Time.fixedDeltaTime;

            material.color = Color.Lerp(startColor, investigateColor, sawPlayerTimer / SAW_PLAYER_TIME);

            if (sawPlayerTimer >= SAW_PLAYER_TIME)
            {
                transition = true;
                state = GuardState.Investigation;
                investigateTransitionTimer = 0.0f;
                sawPlayerTimer = 0.0f;
                lastKnownPlayerPosition = Player.instance.transform.position;
                returnToRotation = transform.rotation;
                transform.rotation = Quaternion.LookRotation((Player.instance.transform.position - transform.position).normalized, Vector3.up);
                lookForPlayerStart = transform.position;
            }
        }
        else
        {
            foreach (Abracadaver a in Player.instance.abracadavers)
            {
                Physics.Raycast(transform.position, (a.transform.position - transform.position).normalized, out hit);

                if (hit.collider == a.GetComponent<Collider>() && hit.distance <= visionRange && IsInCone(hit.point))
                {
                    AggroTarget t;
                    t.target = a;
                    t.stillAlive = true;
                    aggroTarget = t;
                    recentlySeen = true;
                    recentlyHit = false;
                    state = GuardState.Aggro;
                    lookForPlayerStart = transform.position;
                    returnToRotation = transform.rotation;
                    agent.SetDestination(a.transform.position);
                    return;
                }
            }

            foreach (Cake c in Player.instance.cakes)
            {
                RaycastHit hit2;
                Physics.Raycast(transform.position, (c.transform.position - transform.position).normalized, out hit2);

                if (hit2.collider == c.GetComponent<Collider>() && hit2.distance <= visionRange && IsInCone(hit2.point))
                {
                    distractionTarget = c;
                    state = GuardState.Distracted;
                    lookForPlayerStart = transform.position;
                    returnToRotation = transform.rotation;
                    agent.SetDestination(c.transform.position);
                    return;
                }
            }

            material.color = startColor;
            sawPlayerTimer = 0.0f;
        }
    }

    void TransitionToInvestigate()
    {
        investigateTransitionTimer += Time.fixedDeltaTime;

        material.color = Color.Lerp(investigateColor, aggroColor, investigateTransitionTimer / INVESTIGATE_TRANSITION_TIME);

        if (investigateTransitionTimer > INVESTIGATE_TRANSITION_TIME)
        {
            investigateTransitionTimer = 0.0f;

            // Visibility calculation
            // Temporarily a raycast

            RaycastHit hit;
            Physics.Raycast(transform.position, (Player.instance.transform.position - transform.position).normalized, out hit);

            if (hit.collider.tag == "Player" && hit.distance <= visionRange && IsInCone(hit.point))
            {
                recentlySeen = true;
                recentlyHit = false;
                state = GuardState.Aggro;
                AggroTarget t;
                t.target = Player.instance;
                t.stillAlive = true;
                aggroTarget = t;
                material.color = aggroColor;
                lookForPlayerStart = transform.position;
            }
            else
            {
                investigateTransitionTimer = 0.0f;
                transition = false;
                material.color = investigateColor;
                isCheckingLastKnownPosition = true;
                agent.SetDestination(lastKnownPlayerPosition);
            }
        }

       
    }

    void UpdateInvestigation()
    {
        if (transition)
        {
            TransitionToInvestigate();

            return;
        }

        RaycastHit hit;
        Physics.Raycast(transform.position, (Player.instance.transform.position - transform.position).normalized, out hit);

        if (hit.collider.tag == "Player" && hit.distance <= visionRange && IsInCone(hit.point))
        {
            agent.Stop();
            agent.ResetPath();
            agent.velocity = Vector3.zero;

            transition = true;
            isCheckingLastKnownPosition = false;

            transform.rotation = Quaternion.LookRotation((Player.instance.transform.position - transform.position).normalized, Vector3.up);
            lastKnownPlayerPosition = Player.instance.transform.position;

            return;
        }

        // Investigation state
        if (isCheckingLastKnownPosition)
        {
            agent.Resume();

            if (!agent.pathPending &&agent.remainingDistance <= agent.stoppingDistance)
            {
                isCheckingLastKnownPosition = false;
            }

            return;
        }

        if(!finishedLook && !isLookingAround)
        {
            isLookingAround = true;

            lookingStart = transform.rotation;
            passStart = lookingStart;
            goal = lookingStart * Quaternion.AngleAxis(viewConeAngle * 0.5f, Vector3.up);
        }

        if(isLookingAround)
        {
            LookAround();
        }
        else if(!isReturningToPatrol)
        { 
            waitTimer += Time.fixedDeltaTime;

            if (waitTimer > waitTimeBetweenWaypoints)
            {
                waitTimer = 0.0f;

                isReturningToPatrol = true;
                agent.SetDestination(lookForPlayerStart);
                agent.Resume();
            }
        }

        if (isReturningToPatrol)
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                isReturningToPatrol = false;
                finishedLook = false;
                state = GuardState.Oblivious;
                transform.rotation = returnToRotation;
            }
        }
    }

    void UpdateAggro()
    {
        agent.speed = CHASE_SPEED;

        if(!aggroTarget.stillAlive)
        {
            fovTimer = 0.0f;
            state = GuardState.Investigation;
            transition = false;
            agent.speed = 2.0f;
            return;
        }

        GameObject o = null;
        if(aggroTarget.target is Player)
        {
            Player p = aggroTarget.target as Player;
            o = p.gameObject;
        }

        if(aggroTarget.target is Abracadaver)
        {
            Abracadaver a = aggroTarget.target as Abracadaver;
            o = a.gameObject;
        }

        // Chase the attackable
        agent.Resume();
        agent.SetDestination(o.transform.position + (transform.position - o.transform.position).normalized * 2);

        // Attack Speed timer
        if (recentlyHit)
        {
            attackTimer += Time.fixedDeltaTime;

            if(attackTimer >= ATTACK_SPEED)
            {
                recentlyHit = false;
                attackTimer = 0.0f;
            }
        }

        // Field of View Timer
        if(!recentlySeen)
        {
            fovTimer += Time.fixedDeltaTime;

            if(fovTimer >= FOV_TIME)
            {
                fovTimer = 0.0f;
                state = GuardState.Investigation;
                agent.speed = 2.0f;
                transition = false;
                return;
            }
        }

        // Try to raycast to the attackable
        RaycastHit hit;
        Physics.Raycast(transform.position, (o.transform.position - transform.position).normalized, out hit);

        // If we hit what we were looking for...
        if (hit.collider == o.GetComponent<Collider>() && hit.distance <= visionRange && IsInCone(hit.point))
        {
            recentlySeen = true;

            // If we're in range to hit it AND we can attack...
            if (hit.distance <= attackRange && !recentlyHit)
            {
                recentlyHit = true;
                if (!aggroTarget.target.TakeDamage())
                {
                    aggroTarget.stillAlive = false;
                    fovTimer = 0.0f;
                    state = GuardState.Investigation;
                    agent.speed = 2.0f;

                    transition = false;
                    return;
                }
            }
        }
        else
        {
            if (recentlySeen == true)
            {
                lastKnownPlayerPosition = agent.destination;
            }
            recentlySeen = false;
        }
    }

    void UpdateDistracted()
    {
        // Chase the attackable
        agent.Resume();
        
        if(Vector3.Distance(transform.position, distractionTarget.transform.position) <= attackRange)
        {
            if(!distractionTarget.TakeABite())
            {
                state = GuardState.Investigation;
                transition = false;
                return;
            }
        }

    }

    public void TransitionToKnockoutState()
    {
        agent.Stop();
        agent.velocity = Vector3.zero;

        state = GuardState.KnockedOut;
        transform.rotation = returnToRotation;
        knockoutTimer = 0.0f;
        agent.updatePosition = false;
    }

    void UpdateKnockedOut()
    {
        if (isHeldByPlayer)
            return;

        knockoutTimer += Time.fixedDeltaTime;

        material.color = Color.Lerp(koColor, Color.white, knockoutTimer / KNOCKOUT_TIME);

        if (knockoutTimer > KNOCKOUT_TIME)
        {
            agent.nextPosition = transform.position;
            agent.updatePosition = true;
            state = GuardState.Oblivious;
            transform.rotation = returnToRotation;
            _rigidbody.isKinematic = true;
        }
    }

    void IPickup.Pickup(Player player)
    {
        _rigidbody.useGravity = false;
        isHeldByPlayer = true;
        parentCache = transform.parent;
        transform.position = player.transform.position + new Vector3(0.0f, 10.0f, 0.0f);
        transform.SetParent(player.transform);
    }

    void IPickup.Throw(Player player)
    {
        isHeldByPlayer = false;
        _rigidbody.isKinematic = false;
        _rigidbody.useGravity = true;
        _rigidbody.AddForce(player.transform.forward * Player.instance.ThrowForce, ForceMode.Impulse);
        //agent.velocity = player.transform.forward * Player.instance.ThrowForce;
        transform.SetParent(parentCache);
        agent.Resume();
    }

    void LookAround()
    {
        agent.Stop();
        lookingTimer += Time.fixedDeltaTime;

        float halfLookTime = lookingTimePerDirection * 0.5f;

        if (lookingTimer <= halfLookTime) // First look left, 1/2 time
        {
            float t = lookingTimer / halfLookTime;

            transform.rotation = Quaternion.Lerp(passStart, goal, t);

            if (lookingTimer + Time.fixedDeltaTime >= halfLookTime)
            {
                passStart = goal;
                goal = lookingStart * Quaternion.AngleAxis(-viewConeAngle * 0.5f, Vector3.up);
            }
        }
        else if (lookingTimer <= lookingTimePerDirection + halfLookTime) // Look right, full time
        {
            float t = (lookingTimer - halfLookTime) / lookingTimePerDirection;

            transform.rotation = Quaternion.Lerp(passStart, goal, t);

            if (lookingTimer + Time.fixedDeltaTime > lookingTimePerDirection + halfLookTime)
            {
                passStart = goal;
                goal = lookingStart * Quaternion.AngleAxis(viewConeAngle * 0.5f, Vector3.up);
            }
        }
        else if (lookingTimer <= lookingTimePerDirection * 2 + halfLookTime) // Look left full time
        {
            float t = (lookingTimer - halfLookTime - lookingTimePerDirection) / lookingTimePerDirection;

            transform.rotation = Quaternion.Lerp(passStart, goal, t);

            if (lookingTimer + Time.fixedDeltaTime > lookingTimePerDirection * 2 + halfLookTime)
            {
                passStart = goal;
                goal = lookingStart;
            }
        }
        else if (lookingTimer <= lookingTimePerDirection * 3 + halfLookTime) // Look middle, 1/2 time
        {
            float t = (lookingTimer - lookingTimePerDirection * 2 - halfLookTime) / halfLookTime;

            transform.rotation = Quaternion.Lerp(passStart, goal, t);
        }
        else
        {
            isLookingAround = false;
            lookingTimer = 0.0f;
            finishedLook = true;
        }
    }

    void PathMotion()
    {
        // Pathing motion
        if (path.Count == 0)
            return;

        agent.Resume();

        // If we've reached the next position in the path, set the new destination
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (shouldLookAround && !finishedLook)
            {
                isLookingAround = true;

                lookingStart = transform.rotation;
                passStart = lookingStart;
                goal = lookingStart * Quaternion.AngleAxis(viewConeAngle * 0.5f, Vector3.up);
            }
            
            // If we've decided to not look around or are done looking around...
            if (!isLookingAround)
            {
                waitTimer += Time.fixedDeltaTime;

                if (waitTimer > waitTimeBetweenWaypoints)
                {
                    waitTimer = 0.0f;
                    finishedLook = false;

                    if (loopPath)
                    {
                        if (pathIndex == path.Count - 1)
                            pathIndex = 0;
                        else
                        {
                            ++pathIndex;
                        }
                    }
                    else
                    {
                        pathIndex += pathDirection;

                        if (pathIndex == path.Count - 1 || pathIndex == 0)
                        {
                            pathDirection *= -1;
                        }
                    }
                   
                    agent.SetDestination(path[pathIndex].position);
                }
            }
        }
    }

    void DebugDrawCones()
    {
        RaycastHit hit;

        Physics.Raycast(transform.position, (Quaternion.AngleAxis(viewConeAngle * 0.5f, Vector3.up) * (transform.forward)), out hit);
        Debug.DrawRay(transform.position, (Quaternion.AngleAxis(viewConeAngle * 0.5f, Vector3.up) * (transform.forward)) * Mathf.Min(hit.distance, visionRange), Color.red);

        Physics.Raycast(transform.position, (Quaternion.AngleAxis(-viewConeAngle * 0.5f, Vector3.up) * (transform.forward)), out hit);
        Debug.DrawRay(transform.position, (Quaternion.AngleAxis(-viewConeAngle * 0.5f, Vector3.up) * (transform.forward)) * Mathf.Min(hit.distance, visionRange), Color.red);

        Physics.Raycast(transform.position, (Quaternion.AngleAxis(-knockoutAngle * 0.5f, Vector3.up) * (-transform.forward)) * Mathf.Min(hit.distance, Player.KNOCKOUT_RANGE), out hit);
        Debug.DrawRay(transform.position, (Quaternion.AngleAxis(-knockoutAngle * 0.5f, Vector3.up) * (-transform.forward)) * Mathf.Min(hit.distance, Player.KNOCKOUT_RANGE), Color.blue);

        Physics.Raycast(transform.position, (Quaternion.AngleAxis(knockoutAngle * 0.5f, Vector3.up) * (-transform.forward)) * Mathf.Min(hit.distance, Player.KNOCKOUT_RANGE), out hit);
        Debug.DrawRay(transform.position, (Quaternion.AngleAxis(knockoutAngle * 0.5f, Vector3.up) * (-transform.forward)) * Mathf.Min(hit.distance, Player.KNOCKOUT_RANGE), Color.blue);
    }

    void OnDrawGizmos()
    {
        UnityEditor.Handles.color = Color.blue;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, hearingRange);
    }

}
