using UnityEngine;
using System.Collections;
using System;

public class Box : MonoBehaviour, Iinteractable
{
    Movement mover;
    Collider bodyCollider;

    void Start()
    {
        bodyCollider = GetComponent<Collider>();
    }

    void Update()
    {
        if (mover)
        {
            Vector3 offset = mover.transform.position + mover.rail * -1;
            offset.y = transform.position.y;
            transform.position = offset;
            mover.railPos = offset;
        }
	}

    public string getTypeLabel()
    {
        return "Box";
    }

    public void interact(GameObject interactor)
    {
        if (mover)
        {
            mover.lockMovementToRail = false;
            mover.lockPositionToRail = false;
            mover.lockRotation = false;

            Physics.IgnoreCollision(bodyCollider, interactor.GetComponent<Collider>(), false);

            mover = null;
            return;
        }

        mover = interactor.GetComponent<Movement>();

        if (!mover)
            return;

        Vector3 offset = interactor.transform.position - transform.position;
        float dot = Vector3.Dot(offset.normalized, Vector3.forward);
        mover.rail = Mathf.Abs(dot) > 0.5f ? Vector3.forward * Mathf.Sign(dot) : Vector3.right * -Mathf.Sign(dot);
        mover.rail *= Vector3.Dot(offset, mover.rail);
        mover.railPos = transform.position;
        mover.rotationLockDirection = mover.rail * -1;
        mover.rotationLockRange = 0.01f;

        mover.lockMovementToRail = true;
        mover.lockPositionToRail = true;
        mover.lockRotation = true;

        Physics.IgnoreCollision(bodyCollider, interactor.GetComponent<Collider>(), true);
    }

    public bool isInstant()
    {
        return false;
    }
}
