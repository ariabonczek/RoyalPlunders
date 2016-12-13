using UnityEngine;
using System.Collections;

public class Holdable : MonoBehaviour, Iinteractable
{
    public float YOffsetFromOwner = 3; // vertical offset from the player
    private float originalY;
    private bool held = false; // if this is being held

    public GameObject alarmSystem;

    Movement mover; // this object's mover
    Collider col; // this object's collider
    GameObject owner; // this object's owner (if held)

    void Start()
    {
        mover = GetComponent<Movement>(); // cache the mover
        col = GetComponent<Collider>(); // cache the collider
    }
	
	void Update()
    {
        if(owner && held) // if there is an owner and I am being held
            transform.position = owner.transform.position + Vector3.up * YOffsetFromOwner; // move appropriately
    }

    // getter for held
     public bool IsHeld()
    {
        return held;
    }

    public void interact(InteractionButton button, GameObject interactor)
    {
        if (button != InteractionButton.Y) // interact button
            return;

        if (mover) // if I have an owner
            mover.enabled = held; // let me move again
        col.enabled = held; // and enable my collider

        if (!held) // if I am not held
        {
            originalY = transform.position.y;
            owner = interactor; // my owner is the interactor

            if(alarmSystem && alarmSystem.GetComponent<AlarmSystem>()) // if there is an alarm system
                alarmSystem.GetComponent<AlarmSystem>().TurnOnAlarm(); // turn it on
        }
        else // if I am held
        {
            transform.position = owner.transform.position + owner.transform.GetChild(0).forward;
            transform.position.Set(transform.position.x, originalY, transform.position.z);
        }

        held = !held; // toggle the hold

        Movement player = interactor.GetComponent<Movement>(); // get the mover of the interator
        if (player) // if it is there
            player.holding = held; // set the holding to the held
    }

    public string getTypeLabel()
    {
        return "Holdable";
    }

    public bool isInstant()
    {
        return false;
    }

    // reset this object
    public void Reset()
    {
        held = false; // no longer held
        if (owner) // if there is an owner
        {
            Movement player = owner.GetComponent<Movement>(); // try to get it's mover
            if (player) // if so
                player.holding = held; // let it know it is no longer holding anything
        }
        owner = null; // I am free
    }
}
