using UnityEngine;
using System.Collections;

public class Controls : MonoBehaviour
{
    Camera mainCamera;
    ThirdPersonCamera cam;
    Interactor actor;
    Movement mover;
    bool readyToInteract = true;

	void Start ()
    {
        mainCamera = Camera.main;
        cam = mainCamera.GetComponent<ThirdPersonCamera>();
        actor = GetComponent<Interactor>();
        cam.setTarget(gameObject);
        mover = GetComponent<Movement>();

        InteractionTable.LoadTables("interactionTables"); // might be a good idea to move this to a game manager once we have one
	}
	
	void Update ()
    {
        if (Input.GetButton("A Button") && readyToInteract)
        {
            readyToInteract = false;
            actor.interact();
        }
        if (!Input.GetButton("A Button") && !readyToInteract)
            readyToInteract = true;
        
        cam.inputPitch = Input.GetAxis("Right Vertical");
        cam.inputYaw = Input.GetAxis("Right Horizontal");
        
        mover.sprint = Input.GetButton("X Button");
        mover.sneak = Input.GetButton("B Button");

        cam.sprinting = mover.sprint;
        cam.sneaking = mover.sneak;

        mover.direction.x = Input.GetAxis("Left Horizontal");
        mover.direction.z = -Input.GetAxis("Left Vertical");
        mover.relativeForward = mainCamera.transform.forward;
        mover.relativeForward.y = 0;
    }
}
