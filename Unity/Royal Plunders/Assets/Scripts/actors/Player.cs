using UnityEngine;
using System.Collections;

// is this a good idea?
[RequireComponent(typeof(Controls))]
[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(InventoryManager))]
[RequireComponent(typeof(NoiseMakerScript))]
public class Player : MonoBehaviour, Iinteractable
{
    public float captureTime;
    public Transform respawnPoint;

    private bool vulnerable;

    private int railID = 0; // which rail the player is on
    public int RailID { get { return railID;} } // get the rail ID

    void Start()
    {
        captureTime = 0;
        vulnerable = false;
    }

    public void interact(InteractionButton button, GameObject interactor)
    {
        if (!vulnerable)
        {
            vulnerable = true;
            captureTime = Time.time + captureTime;
        }
        else
        {
            if (Time.time >= captureTime)
            {
                // TODO call HUD interactionss
                transform.position = respawnPoint.position;
            }
        }
    }

    public string getTypeLabel()
    {
        return "Player";
    }

    public bool isInstant()
    {
        return false;
    }

    // make the player attached to this rail
    public void assignRail(Rail rail)
    {
        railID = rail.ID;
    }

    // free the player from the rail
    public void unassignRail()
    {
        railID = 0;
    }
}
