using UnityEngine;
using System.Collections;

public class Controls : MonoBehaviour
{
    Camera mainCamera;
    CameraController camCont;
    Interactor actor;
    Movement mover;
    bool readyToTurn = true;
    bool readyToInteract = true;

	void Start ()
    {
        mainCamera = Camera.main;
        camCont = mainCamera.GetComponent<CameraController>();
        actor = GetComponent<Interactor>();
        camCont.targetTransform = transform;
        mover = GetComponent<Movement>();

        InteractionTable.LoadTables("interactionTables"); // might be a good idea to move this to a game manager once we have one
	}
	
	void Update ()
    {
        if (Input.GetButton("Left Shoulder") && readyToTurn)
        {
            readyToTurn = false;
            camCont.angleSnapIndex++;
        }
        if (Input.GetButton("Right Shoulder") && readyToTurn)
        {
            readyToTurn = false;
            camCont.angleSnapIndex--;
        }

        if (!Input.GetButton("Left Shoulder") && !readyToTurn && !Input.GetButton("Right Shoulder"))
            readyToTurn = true;

        if (Input.GetButton("A Button") && readyToInteract)
        {
            readyToInteract = false;
            actor.interact();
        }
        if (!Input.GetButton("A Button") && !readyToInteract)
            readyToInteract = true;

        camCont.freeLookX = Input.GetAxis("Right Horizontal");
        camCont.freeLookY = Input.GetAxis("Right Vertical");

        mover.sprint = Input.GetButton("X Button");
        mover.sneak = Input.GetButton("B Button");
        mover.direction.x = Input.GetAxis("Left Horizontal");
        mover.direction.z = -Input.GetAxis("Left Vertical");
        mover.relativeForward = mainCamera.transform.forward;
        mover.relativeForward.y = 0;
    }
}
