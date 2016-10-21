using UnityEngine;
using System.Collections;

public class Rail : MonoBehaviour, Iinteractable
{
    GameObject shell;

    static int idTracker = 1;
    private int id;
    public int ID { get{ return id; } }

    void Start()
    {
        shell = transform.parent.gameObject;
        id = idTracker++;
    }

    public void interact(InteractionButton button,  GameObject interactor)
    {
        if (button != InteractionButton.Y)
            return;

        Player player = interactor.GetComponent<Player>();
        Movement mover = interactor.GetComponent<Movement>();

        if (!player)
            return;

        if (player.RailID != id)
        {
            mover.rail = shell.transform.right * shell.transform.localScale.x;
            mover.railPos = transform.position - mover.rail / 2;
            mover.rotationLockDirection = transform.forward;
            mover.rotationLockRange = 0.01f;
            mover.lockMovementToRail = true;
            mover.lockPositionToRail = true;
            mover.lockRotation = true;
            player.assignRail(this);
        }
        else if (player.RailID == id)
        {
            mover.lockMovementToRail = false;
            mover.lockPositionToRail = false;
            mover.lockRotation = false;
            player.unassignRail();
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
