using UnityEngine;
using System.Collections;

public class GuardAITest : MonoBehaviour {

    NavMeshAgent agent;

    public GameObject player;

    public GameObject pathPoints;

    public float angleOfView;

    // the distance at which the guard will chase the player
    public float PlayerSpotDistance;

    // the distance from their original path destination where the guard will cut off a chase.
    public float MaxDistanceFromPoint;

    // the points for the guards patrol
    private Vector3[] pathLocations;

    private bool chasingPlayer;

    private int destPoint = 0;

    // Use this for initialization
    void Start () {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        chasingPlayer = false;
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
        if(player && PlayerInView() && Vector3.Distance(transform.position, player.transform.position) < PlayerSpotDistance && Vector3.Distance(transform.position, pathLocations[destPoint]) <= MaxDistanceFromPoint)
        {
            chasingPlayer = true;
        }
        Debug.DrawRay(transform.position, transform.forward, Color.red, 20);

        // while in the state of chasing the player
        if(chasingPlayer)
        {
            agent.destination = player.transform.position;

            // stop chasing if it strays too far off path
            if (Vector3.Distance(transform.position, pathLocations[destPoint]) > MaxDistanceFromPoint)
            {
                chasingPlayer = false;
                agent.destination = pathLocations[destPoint];
            }
        }
        
        // proceed to next patrol point when in range of current point
        if (pathPoints && Vector3.Distance(transform.position,pathLocations[destPoint])<1)
        {
            GotoNextPoint();
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
            // using the dot product formular where dot = |a||b|cos(theta) and solving for theta
            Vector2 forward = new Vector2(transform.forward.x, transform.forward.y);
            Vector2 toPlayer = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.y- transform.position.y);

            // the forward is already normalized, and by normalizing toPayer, we eliminate the magnitude part of the equation
            float dot = Vector2.Dot(forward, toPlayer.normalized);

            // now we have cos(theta) for dot, we just need to get the arc cosine to get theta.
            if(Mathf.Acos(dot) <=(Mathf.Deg2Rad*angleOfView/2))
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
