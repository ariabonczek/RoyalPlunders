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

    private bool chasingPlayer;

    private bool suspicious;

    private bool suspiciousPrev;

    private Vector3 suspicionPoint;

    private int destPoint = 0;

    // Use this for initialization
    void Start () {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        chasingPlayer = false;
        suspicious = false;
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
        // chase the player if they are in range and you are not too far from your patrol route
        if(player && PlayerInView() && Vector3.Distance(transform.position, pathLocations[destPoint]) <= MaxDistanceFromPoint)
        {
            chasingPlayer = true;
        }
        Debug.DrawRay(transform.position, transform.forward, Color.red, 20);

        // chase the player if they are in range and making a loud enough sound
        NoiseMakerScript noiseScript = player.GetComponent<NoiseMakerScript>();
        if(noiseScript)
        {
            switch(noiseScript.GetSoundLevel())
            {
                case 1:
                    suspicious |= (Vector3.Distance(transform.position, player.transform.position) < sneakWalkDetectionRange);
                    break;
                case 2:
                    suspicious |= (Vector3.Distance(transform.position, player.transform.position) < slowWalkDetectionRange);
                    break;
                case 3:
                    suspicious |= (Vector3.Distance(transform.position, player.transform.position) < fastWalkDetectionRange);
                    break;
                case 4:
                    suspicious |= (Vector3.Distance(transform.position, player.transform.position) < runDetectionRange);
                    break;
                default:

                    break;
            }
            if(suspicious && !suspiciousPrev)
            {
                suspicionPoint = player.transform.position;
            }
        }

        // while in the state of chasing the player
        if(chasingPlayer)
        {
            suspicious = false;
            transform.GetChild(0).position = transform.position + new Vector3(0, 2, 0);
            transform.GetChild(2).position = transform.position + new Vector3(0, -100, 0);
            transform.GetChild(1).position = transform.position + new Vector3(0, -100, 0);
            agent.destination = player.transform.position;

            // stop chasing if it strays too far off path
            if (Vector3.Distance(transform.position, pathLocations[destPoint]) > MaxDistanceFromPoint)
            {
                chasingPlayer = false;
                agent.destination = pathLocations[destPoint];
            }
        }
        else if (suspicious && Vector3.Distance(transform.position, pathLocations[destPoint]) <= MaxDistanceFromPoint)
        {
            transform.GetChild(1).position = transform.position + new Vector3(0, 2, 0);
            transform.GetChild(0).position = transform.position + new Vector3(0, -100, 0);
            transform.GetChild(2).position = transform.position + new Vector3(0, -100, 0);
            agent.destination = suspicionPoint;
            if(Vector3.Distance(transform.position,suspicionPoint)<1)
            {
                suspicious = false;
                agent.destination = pathLocations[destPoint];
            }
        }
        else
        {
            transform.GetChild(2).position = transform.position + new Vector3(0, 2, 0);
            transform.GetChild(0).position = transform.position + new Vector3(0, -100, 0);
            transform.GetChild(1).position = transform.position + new Vector3(0, -100, 0);
        }
        
        // proceed to next patrol point when in range of current point
        if (pathPoints && Vector3.Distance(transform.position,pathLocations[destPoint])<1)
        {
            GotoNextPoint();
        }
        suspiciousPrev = suspicious;
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
