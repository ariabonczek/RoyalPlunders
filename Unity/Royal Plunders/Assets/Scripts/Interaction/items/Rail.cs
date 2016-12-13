using UnityEngine;
using System.Collections;

public class Rail : MonoBehaviour, Iinteractable
{
    GameObject shell; // the body of the rail that exists as the root object

    static int idTracker = 1; // each rail gets it's own ID, for rail switching coherancy
    private int id; // the id of the rail
    public int ID { get{ return id; } } // id exposure/getter

    void Start()
    {
        shell = transform.parent.gameObject; // get the shell
        id = idTracker++; // get a unique id
    }

    public void interact(InteractionButton button,  GameObject interactor)
    {
        if (button != InteractionButton.Y) // stealth interaction button
            return;

        Player player = interactor.GetComponent<Player>(); // cache the player
        Movement mover = interactor.GetComponent<Movement>(); // cache the mover

        if (!player) // if there is no player, exit out
            return;

        // if the player is not in this rail
        if (player.RailID != id)
        {
            mover.rail = shell.transform.right * shell.transform.localScale.x; // the Ray for movement restriction
            mover.railPos = transform.position - mover.rail / 2; // the Ray position
            mover.rotationLockDirection = transform.forward; // the restricted rotation direction
            mover.rotationLockRange = 0.01f; // how much wiggle room there is in rotation
            mover.lockMovementToRail = true; // can only move along this rail
            mover.lockPositionToRail = true; // can only move within this rail
            mover.lockRotation = true; // can not look away from this rail
            player.assignRail(this); // assign the player to this rail
        }
        else if (player.RailID == id) // the player is in this rail
        {
            mover.lockMovementToRail = false; // let the player move freely
            mover.lockPositionToRail = false; // let the player move outside the rail
            mover.lockRotation = false; // let the player rotate freely
            player.unassignRail(); // unbind the player from this rail
        }
    }

    public string getTypeLabel()
    {
        return "Rail";
    }

    public bool isInstant()
    {
        return false;
    }
}
