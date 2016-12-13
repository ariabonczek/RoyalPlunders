using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour, Iinteractable
{
    private Quaternion startingRotation; // the initial rotation of the door
    private bool isOpen = false; // is the door open?
    private Transform hinge; // the point at which the door hinges from

    public float rotation = 90; // the amount to rotate (sign determines direction)
    public float rotationSpeed = 1; // how fast to turn

    public int numKeyReq = 0; // how many keys are required to open this door
    
    void Start()
    {
        hinge = transform.parent; // get the hinge of the door
        startingRotation = hinge.localRotation; // get the starting rotation
    }

    void Update()
    {
        if (isOpen) // if the door is open
        {
            Quaternion quat = Quaternion.Euler(0, startingRotation.eulerAngles.y + rotation, 0); // get the open rotation
            hinge.localRotation = Quaternion.Lerp(hinge.localRotation, quat, Time.deltaTime * rotationSpeed); // and lerp to it
        }
        else // if the door is closed
        {
            hinge.localRotation = Quaternion.Lerp(hinge.localRotation, startingRotation, Time.deltaTime * rotationSpeed); // lerp to the closed rotation
        }
    }

    // opens the door
    public void Open()
    {
        isOpen = true;
    }

    // closes the door
    public void Close()
    {
        isOpen = false;
    }

    // removes the lock from the door
    public void WipeKeyReq()
    {
        numKeyReq = 0;
    }

    public void interact(InteractionButton button, GameObject interactor)
    {
        if (button != InteractionButton.Y) // interact button
            return;

        InventoryManager invManager = interactor.GetComponent<InventoryManager>(); // get the inventory manager of the interactor

        if (invManager == null) // no inventory? let the door open no matter what
        {
            Open();
            return;
        }

        if (invManager.GetNumKeys() >= numKeyReq && !isOpen) // if the interactor has enough keys
            Open(); // open
        else if (isOpen) // else if it is already open
            Close(); // close
    }

    public string getTypeLabel()
    {
        return "Door";
    }

    public bool isInstant()
    {
        return false;
    }
}
