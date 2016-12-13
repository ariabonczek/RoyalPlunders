using UnityEngine;
using System.Collections;
using System;

public class Box : MonoBehaviour, Iinteractable
{
    Movement mover; // the mover to restrain
    Collider bodyCollider; // this box's collider

    void Start()
    {
        bodyCollider = GetComponent<Collider>(); // cache the collider
    }

    void Update()
    {
        if (mover) // if somone is pushing/pulling this box
        {
            Vector3 offset = mover.transform.position + mover.rail * -1; // move along the mover's rail
            offset.y = transform.position.y; // don't freak out on the Y axis
            transform.position = offset; // move to the offset
            mover.railPos = offset; // move the rail
            // essentially a rail has been made in the mover and the box is making sure that rail follows the box
            // this effectively constrains the mover from ever leaving the box while pushing/pulling it in cardinal directions
        }
	}

    public string getTypeLabel()
    {
        return "Box";
    }

    public void interact(InteractionButton button, GameObject interactor)
    {
        if (button != InteractionButton.Y) // the interact button
            return;

        if (mover) // if something is already moving the box
        {
            // free the mover
            mover.lockMovementToRail = false;
            mover.lockPositionToRail = false;
            mover.lockRotation = false;

            // let the mover collide with me again
            Physics.IgnoreCollision(bodyCollider, interactor.GetComponent<Collider>(), false);

            mover = null; // I have no mover
            return; // early exit
        }

        // I have a mover
        mover = interactor.GetComponent<Movement>();

        if (!mover) // or maybe I don't
            return;

        Vector3 offset = interactor.transform.position - transform.position; // initial vector from box to interactor
        float dot = Vector3.Dot(offset.normalized, Vector3.forward); // the "angle" off from the global forward
        mover.rail = Mathf.Abs(dot) > 0.5f ? Vector3.forward * Mathf.Sign(dot) : Vector3.right * -Mathf.Sign(dot); // figure out which side of the box the interactor is on
        mover.rail *= Vector3.Dot(offset, mover.rail); // cast the original offset onto the normal of the box face the interactor is on
        mover.railPos = transform.position; // the rail needs to be where the box is
        mover.rotationLockDirection = mover.rail * -1; // look at the box when you interact with it
        mover.rotationLockRange = 0.01f; // can't look away

        // restrict their movement
        mover.lockMovementToRail = true;
        mover.lockPositionToRail = true;
        mover.lockRotation = true;

        // turn off collisions between the box and the interactor, lest they fly into the sunset
        Physics.IgnoreCollision(bodyCollider, interactor.GetComponent<Collider>(), true);
    }

    public bool isInstant()
    {
        return false;
    }
}
