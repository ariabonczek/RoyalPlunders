using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Movement : MonoBehaviour
{
    // movement speeds
    public float baseSpeed = 2;
    // joystick
    public float speedChangeThreshold = 0.8f;
    public float slowWalkSpeed = 2;
    public float fastWalkSpeed = 3;
    // button speeds
    public float sprintSpeed = 4;
    public float sneakSpeed = 0.3f;
    public float carrySpeed = 0.5f;

    // movement vectors
    public Vector3 relativeForward;
    public Vector3 direction;

    // rail locking
    // rail defines the direction of resitricted movement
    public Vector3 rail;
    public bool lockMovementToRail;
    // if the rail becomes a plane, it's length is used with this position to confine movement to a plane rotated about the Y axis
    public Vector3 railPos;
    public bool lockPositionToRail;

    // rotation locking
    public Vector3 rotationLockDirection;
    public float rotationLockRange;
    public bool lockRotation;

    // movement states
    public bool sprint;
    public bool sneak;
    public bool holding;

    // interfacing components
    NoiseMakerScript noiseScript;
    Rigidbody body;

    // model
    GameObject model;
    public float modelTurnRate = 7.5f;

    void Start ()
    {
        noiseScript = GetComponent<NoiseMakerScript>();
        model = transform.FindChild("Model").gameObject;
        body = GetComponent<Rigidbody>();
    }
	
	void Update ()
    {
        relativeForward.Normalize();
        
        // calculate the angle difference between the relative orientation on the xz plane and what is essentially the direction character movement expects to move on
        float cameraAngleDiff = Vector3.Angle(Vector3.forward, relativeForward);
        // if the cross component relative to the xz plane normal (y) is positive or zero then we do nothing, but if it is negative then we negate the angle
        if (Vector3.Cross(Vector3.forward, relativeForward).y < 0)
            cameraAngleDiff *= -1;

        float speed = direction.magnitude;
        direction.Normalize();
        direction = Quaternion.AngleAxis(cameraAngleDiff, Vector3.up) * direction;
        direction.Normalize();

        Vector3 position = transform.position;

        rail.y = 0; // ignore the 3rd dimension

        // railing projection
        if (lockMovementToRail && rail != Vector3.zero)
        {
            direction = Vector3.Project(direction, rail).normalized;

            // ray limiting
            if (lockPositionToRail)
            {
                float oldY = position.y;
                position = ClosestPointOnRay(railPos, rail, position);
                position.y = oldY;
            }
        }

        direction *= baseSpeed * Time.deltaTime;
        
        int soundValue = 0;
        float appliedSpeed;

        if (holding)
        {
            soundValue = 3;
            appliedSpeed = carrySpeed;
        }
        else if (sprint)
        {
            soundValue = 4;
            appliedSpeed = sprintSpeed;
        }
        else if(sneak)
        {
            soundValue = 1;
            appliedSpeed = sneakSpeed;
        }
        else
        { 
            if (speed < speedChangeThreshold)
            {
                soundValue = 2;
                appliedSpeed = slowWalkSpeed;
            }
            else
            {
                soundValue = 3;
                appliedSpeed = fastWalkSpeed;
            }
        }

        if (direction == Vector3.zero)
            soundValue = 0;
        else
        {
            body.MovePosition(position + direction * appliedSpeed);

            Quaternion directionRotation = Quaternion.LookRotation(direction, Vector3.up);

            if (lockRotation)
            {
                float dirAngle = Vector3.Angle(direction, rotationLockDirection);
                if (dirAngle >= rotationLockRange)
                    directionRotation = Quaternion.Lerp(directionRotation, Quaternion.LookRotation(rotationLockDirection, Vector3.up), 1 - (rotationLockRange / dirAngle));
            }

            model.transform.rotation = Quaternion.Lerp(model.transform.rotation, directionRotation, Time.deltaTime * modelTurnRate);
        }

        if (noiseScript)
            noiseScript.AdjustSoundLevel(soundValue);
    }

    // might wanna move this later, its handy
    Vector3 ClosestPointOnRay(Vector3 rayPoint, Vector3 ray, Vector3 point)
    {
        Vector3 rayNorm = ray.normalized;
        float t = Vector3.Dot(rayNorm, point - rayPoint);

        if (t <= 0)
            return rayPoint;

        if (t >= ray.magnitude)
            return rayPoint + ray;

        return rayPoint + rayNorm * t;
    }

}
