using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovementTest : MonoBehaviour {

    public bool inputDisabled;

    public float baseSpeed;

    // the stick amount threshold to switch from slow to fast walk
    public float speedChangeThreshold;

    // movement speeds
    public float slowWalkSpeed;

    public float fastWalkSpeed;

    public float runSpeed;

    public float stealthWalkSpeed;

    public float holdingWalkSpeed;

    // booleans for the moveMode selected by the player
    private bool runButtonPressed;

    private bool stealthWalkButtonPressed;

    public bool GetStealthWalkState()
    {
        return stealthWalkButtonPressed;
    }

    // float used to rotate the movement vector relative to camera orientation
    private float cameraAngleDiff;

    // the vector that tracks the raw movement given by the stick and rotated to match camera orientation
    private Vector3 rawMovementVec;

    // a reference to the camera in the scene
    private GameObject myCamera;

    // Use this for initialization
    void Start () {
        // initializations
        cameraAngleDiff = 0;
        rawMovementVec = new Vector3();
        inputDisabled = false;

        // gets the camera reference from the scene
        myCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }
	
	// Update is called once per frame
	void Update () {
        if (inputDisabled)
            return;

        // set the booleans for stealth walk and run buttons
        runButtonPressed = Input.GetButton("A Button");

        stealthWalkButtonPressed = Input.GetButton("B Button");


        // Get the raw movement vector and adjust it to the camera angle
        {
            // calculate the angle difference between the camera orientation on the xz plane and what is essentially the direction character movement expects to move on
            cameraAngleDiff = Vector3.Angle(new Vector3(0, 0, 1), new Vector3(myCamera.transform.forward.x, 0, myCamera.transform.forward.z));

            // use cross product to get the sign of the angle at hand
            Vector3 cross = Vector3.Cross(new Vector3(0, 0, 1), new Vector3(myCamera.transform.forward.x, 0, myCamera.transform.forward.z));

            // if the cross component relative to the xz plane normal (y) is positive or zero then we do nothing, but if it is negative then we negate the angle
            if (cross.y < 0)
            {
                cameraAngleDiff = -cameraAngleDiff;
            }

            // we get the movement vector using the left analog stick input and the character's orientation
            rawMovementVec.x = (Vector3.left * -(Input.GetAxis("Left Horizontal"))).x;
            rawMovementVec.z = (Vector3.forward * -(Input.GetAxis("Left Vertical"))).z;

            // we then rotate the movement vector by the camera angle difference we've just calculated
            rawMovementVec = Quaternion.AngleAxis(cameraAngleDiff, Vector3.up) * rawMovementVec;
        }

        // here we interpret the speed the player has put in via stick and button control, factor it in with base speed and time delta, and apply it as the new speed
        {
            float baseSpeedWithDelta = (baseSpeed * Time.deltaTime);

            // check if this object has a holder script and if they are holding anything
            HolderScript holder = GetComponent<HolderScript>();

            if(holder && holder.IsHolding())
            {
                this.transform.position += rawMovementVec.normalized * holdingWalkSpeed * baseSpeedWithDelta;
            }
            // check if the movement buttons have been pressed, with priority given to running
            else if (runButtonPressed)
            {
                this.transform.position += rawMovementVec.normalized * runSpeed * baseSpeedWithDelta;
                // Debug.Log("Running");
            }
            else if(stealthWalkButtonPressed)
            {
                this.transform.position += rawMovementVec.normalized * stealthWalkSpeed * baseSpeedWithDelta;
                // Debug.Log("Stealth Walking");
            }
            else
            { 
                float appliedSpeed;
                if (rawMovementVec.magnitude < speedChangeThreshold)
                {
                    appliedSpeed = slowWalkSpeed;
                    // Debug.Log("Slow Walk");
                }
                else
                {
                    appliedSpeed = fastWalkSpeed;
                    // Debug.Log("Fast Walk");
                }
                this.transform.position += rawMovementVec.normalized * appliedSpeed * baseSpeedWithDelta;
            }
        }
    }
}
