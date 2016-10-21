using UnityEngine;
using System.Collections;

/*
 * B - Sneak Attack
 * X - Sprint
 * A - Use Gadget
 * Y - Interact
 * L1 - sneak
 * Left Stick - Movement
 * Right Stick - Camera Orbit
 * Right D-Pad - Gadget 2 Equip
 * Left D-Pad - Gadget 1 Equip 
 */

public class Controls : MonoBehaviour
{
    Camera mainCamera;
    ThirdPersonCamera cam;
    Interactor actor;
    Movement mover;

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
        if (Input.GetButtonDown("Y Button"))
            actor.interact(InteractionButton.Y);
        if (Input.GetButtonDown("B Button"))
            actor.interact(InteractionButton.B);

        cam.inputPitch = Input.GetAxis("Right Vertical");
        cam.inputYaw = Input.GetAxis("Right Horizontal");
        
        mover.sprint = Input.GetButton("X Button");
        mover.sneak = Input.GetButton("Left Shoulder");

        cam.sprinting = mover.sprint;
        cam.sneaking = mover.sneak;

        mover.direction.x = Input.GetAxis("Left Horizontal");
        mover.direction.z = -Input.GetAxis("Left Vertical");
        mover.relativeForward = mainCamera.transform.forward;
        mover.relativeForward.y = 0;
    }
}
