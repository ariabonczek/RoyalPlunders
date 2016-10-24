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

    string[] joys;
    enum Controllers { XBOX, PS4, NONE};

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
        Controllers controller = getControllerType();

        if (controller == Controllers.XBOX)
        {
            if (Input.GetButtonDown("B"))
                actor.interact(InteractionButton.B);

            if (Input.GetButtonDown("A"))
                GetComponent<GadgetManager>().UseGadget();

            cam.inputPitch = Input.GetAxis("Right Vertical XBone");
            cam.inputYaw = Input.GetAxis("Right Horizontal XBone");

            mover.sprint = Input.GetButton("X XBone");
        }

        if (controller == Controllers.PS4)
        {
            if (Input.GetButtonDown("Circle"))
                actor.interact(InteractionButton.B);
            if (Input.GetButtonDown("X PS4"))
                GetComponent<GadgetManager>().UseGadget();

            cam.inputPitch = Input.GetAxis("Right Vertical PS4");
            cam.inputYaw = Input.GetAxis("Right Horizontal PS4");

            mover.sprint = Input.GetButton("Square");
        }

        if (controller != Controllers.NONE)
        {
            if (Input.GetButtonDown("Y / Triangle"))
                actor.interact(InteractionButton.Y);

            mover.sneak = Input.GetButton("L1");

            mover.direction.x = Input.GetAxis("Left Horizontal");
            mover.direction.z = -Input.GetAxis("Left Vertical");
        }

        cam.sprinting = mover.sprint;
        cam.sneaking = mover.sneak;

        mover.relativeForward = mainCamera.transform.forward;
        mover.relativeForward.y = 0;
    }

    Controllers getControllerType()
    {
        Controllers controller = Controllers.NONE;

        joys = Input.GetJoystickNames();
        for (int i = 0; i < joys.Length; ++i)
        {
            if (joys[i].IndexOf("Windows") != -1)
            {
                controller = Controllers.XBOX;
                break;
            }
            else if (joys[i] == "Wireless Controller")
            {
                controller = Controllers.PS4;
            }
        }

        return controller;
    }
}
