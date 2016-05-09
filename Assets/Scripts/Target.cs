using UnityEngine;
using System.Collections;

using System.Collections.Generic;


public class Target : MonoBehaviour, IPickup
{
    List<Transform> hidingSpots;
    private bool travelling = false;
    private NavMeshAgent agent;

	// Use this for initialization
	void Start ()
    {
        agent = GetComponent<NavMeshAgent>();
        hidingSpots = new List<Transform>();
        GameObject[] spots = GameObject.FindGameObjectsWithTag("HidingSpot");
        foreach(GameObject g in spots)
        {
            hidingSpots.Add(g.transform);
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(!travelling)
        {
            agent.SetDestination(hidingSpots[(int)Mathf.Floor(Random.value * hidingSpots.Count)].position);
            travelling = true;
        }
        else
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                travelling = false;
            }

        }
    }

    void IPickup.Pickup(Player player)
    {
        
    }

    void IPickup.Throw(Player player)
    {
      
    }
}
