using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Movement : MonoBehaviour
{
    public float currentSpeed = 0;
    // movement speeds
    public float baseSpeed = 2;
    // joystick
    // ranges of movement ont he joystick
    public float speedChangeThreshold1To2 = 0.3f;
    public float speedChangeThreshold2To3 = 0.6f;
    public float speedChangeThreshold3To4 = 0.8f;
    // ranges of walk speeds
    public float walkSpeed1 = 2;
    public float walkSpeed2 = 3;
    public float walkSpeed3 = 4;
    public float walkSpeed4 = 5;
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
        noiseScript = GetComponent<NoiseMakerScript>(); // cache the noise maker component
        model = transform.FindChild("Model").gameObject; // get reference to the model to rotate it
        body = GetComponent<Rigidbody>(); // the rigidbody, for applying movement
    }
	
	void Update ()
    {
        relativeForward.Normalize(); // clean the normal
        
        // calculate the angle difference between the relative orientation on the xz plane and what is essentially the direction character movement expects to move on
        float cameraAngleDiff = Vector3.Angle(Vector3.forward, relativeForward);
        // if the cross component relative to the xz plane normal (y) is positive or zero then we do nothing, but if it is negative then we negate the angle
        if (Vector3.Cross(Vector3.forward, relativeForward).y < 0)
            cameraAngleDiff *= -1;

        // cache the speed
        float speed = direction.magnitude;
        direction.Normalize(); // turn direction into a normal
        direction = Quaternion.AngleAxis(cameraAngleDiff, Vector3.up) * direction; // rotate it to be relative to the player
        direction.Normalize(); // clean the normal

        Vector3 position = transform.position; // current position

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

        // rescale direction into velocity
        direction *= baseSpeed * Time.deltaTime;
        
        int soundValue = 0;
        float appliedSpeed;

        // large logic tree for varying speed and sound levels via buttons and states
        if (holding) // holding the target
        {
            soundValue = 3;
            appliedSpeed = carrySpeed;
        }
        else if (sprint) // is sprinting
        {
            soundValue = 6;
            appliedSpeed = sprintSpeed;
        }
        else if(sneak) // is sneaking
        {
            soundValue = 1;
            appliedSpeed = sneakSpeed;
        }
        else
        {
            // large logic tree for varying speed and sound levels via the joystick
            if (speed < speedChangeThreshold1To2)
            {
                soundValue = 2;
                appliedSpeed = walkSpeed1;
            }
            else if( speed < speedChangeThreshold2To3)
            {
                soundValue = 3;
                appliedSpeed = walkSpeed2;
            }
            else if( speed < speedChangeThreshold3To4)
            {
                soundValue = 4;
                appliedSpeed = walkSpeed3;
            }
            else
            {
                soundValue = 5;
                appliedSpeed = walkSpeed4;
            }
        }

        if (direction == Vector3.zero) // if the direction is null (player not moving)
        {
            soundValue = 0; // no sound
            currentSpeed = 0; // no speed
        }
        else // the player is moving
        {
            body.MovePosition(position + direction * appliedSpeed); // move the player

            Quaternion directionRotation = Quaternion.LookRotation(direction, Vector3.up); // model rotation

            // locking the rotation to a rail
            if (lockRotation)
            {
                float dirAngle = Vector3.Angle(direction, rotationLockDirection); // angle between the desired direction and current direction
                // if the delta is greater than permitted
                if (dirAngle >= rotationLockRange) // then Lerp the rotation towards the locked range
                    directionRotation = Quaternion.Lerp(directionRotation, Quaternion.LookRotation(rotationLockDirection, Vector3.up), 1 - (rotationLockRange / dirAngle));
            }

            // Lerp the rotation of the model
            model.transform.rotation = Quaternion.Lerp(model.transform.rotation, directionRotation, Time.deltaTime * modelTurnRate);
            currentSpeed = appliedSpeed; // let other components know your current speed
        }

        if (noiseScript) // if the noise script is there
            noiseScript.AdjustSoundLevel(soundValue); // apply the sound value to the sound level
    }

    // might wanna move this later, its handy
    // find the closest point on a ray, keeping distance in mind
    Vector3 ClosestPointOnRay(Vector3 rayPoint, Vector3 ray, Vector3 point)
    {
        Vector3 rayNorm = ray.normalized; // create a direction
        float t = Vector3.Dot(rayNorm, point - rayPoint); // dot it with the offset of the point to project

        if (t <= 0) // if it is behind the ray
            return rayPoint; // then return the ray point

        if (t >= ray.magnitude) // if it is beyond the ray
            return rayPoint + ray; // return the end of the ray

        return rayPoint + rayNorm * t; // otherwise, return the ray point at t distance down the ray
    }

}
