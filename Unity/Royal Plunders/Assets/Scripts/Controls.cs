using UnityEngine;
using System.Collections;

/*
 * B - Interact
 * X - Sprint
 * A - Use Gadget
 * Y - Grab the target
 * L1 - sneak
 * R1 - trigger sound
 * Left Stick - Movement
 * Right Stick - Camera Orbit
 * Right D-Pad - Gadget 2 Equip
 * Left D-Pad - Gadget 1 Equip 
 */

public class Controls : MonoBehaviour
{
    public GameObject gameManager; // used for accessing the pause menu

    Camera mainCamera; // handle camera motion
    ThirdPersonCamera cam; // handle camera controls
    Interactor actor; // handle player interaction
    Movement mover; // handle player movement
    NoiseMakerScript noiseMaker; // handle player noise

    string[] joys; // joystick poll array
    enum Controllers { XBOX, PS4, NONE}; // supported joysticks (NONE means you can't play well)

	void Start ()
    {
        mainCamera = Camera.main; // cache the main camera
        cam = mainCamera.GetComponent<ThirdPersonCamera>(); // cache the cam controls
        actor = GetComponent<Interactor>(); // cache the player's interaction
        cam.setTarget(gameObject); // link the camera to the player
        mover = GetComponent<Movement>(); // cache the movement component
        noiseMaker = GetComponent<NoiseMakerScript>(); // cache the noise maker
        InteractionTable.LoadTables("interactionTables"); // might be a good idea to move this to a game manager once we have one
	}
	
	void Update ()
    {
        Controllers controller = getControllerType(); // poll for the controller being used

        if (controller == Controllers.XBOX) // XBOX bindings
        {
            if (!gameManager.GetComponent<PauseMenu>().menuOpen) // if the menu is not open
            {
                if (Input.GetButtonDown("B")) // interact
                    actor.interact(InteractionButton.B);

                if (Input.GetButtonDown("A")) // use gadget
                    GetComponent<GadgetManager>().UseGadget();
            }
            else
            {                
                if (Input.GetButtonDown("B")) // go back in the menu
                    gameManager.GetComponent<PauseMenu>().CloseSceneSelect();
            }
                

            cam.inputPitch = Input.GetAxis("Right Vertical XBone"); // look up and down
            cam.inputYaw = Input.GetAxis("Right Horizontal XBone"); // look left and right

            mover.sprint = Input.GetButton("X XBone"); // sprint
        }

        if (controller == Controllers.PS4) // PS4 bindings
        {
            if (!gameManager.GetComponent<PauseMenu>().menuOpen) // if the menu is not open
            {
                if (Input.GetButtonDown("Circle")) // interact
                    actor.interact(InteractionButton.B);

                if (Input.GetButtonDown("X PS4")) // use gadget
                    GetComponent<GadgetManager>().UseGadget();
            }
            else
            {
                if (Input.GetButtonDown("Circle")) // go back in the menu
                    gameManager.GetComponent<PauseMenu>().CloseSceneSelect();
            }

            cam.inputPitch = Input.GetAxis("Right Vertical PS4"); // look up and down
            cam.inputYaw = Input.GetAxis("Right Horizontal PS4"); // look left and right

            mover.sprint = Input.GetButton("Square"); // sprint
        }

        if (controller != Controllers.NONE) // if there is a controller plugged in
        {
            if (Input.GetButtonDown("Y / Triangle")) // grab the target
                actor.interact(InteractionButton.Y);

            if(Input.GetButton("R1")) // make a sound
                noiseMaker.PlayerTriggeredSound();

            if (Input.GetButtonDown("Start Button") && gameManager) // pause the game
                gameManager.GetComponent<PauseMenu>().HandleMenu();

            mover.sneak = Input.GetButton("L1"); // sneak
            mover.direction.z = -Input.GetAxis("Left Vertical"); // move up and down
            mover.direction.x = Input.GetAxis("Left Horizontal"); // move left and right
        }

        cam.sprinting = mover.sprint; // apply sprint to the camera
        cam.sneaking = mover.sneak; // apply sneak to the camera

        mover.relativeForward = mainCamera.transform.forward; // orient the controller's forward
        mover.relativeForward.y = 0; // normalize it to the XZ plane
    }

    // finds out which controller is in use
    Controllers getControllerType()
    {
        Controllers controller = Controllers.NONE; // default status

        joys = Input.GetJoystickNames(); // Unity controller report
        for (int i = 0; i < joys.Length; ++i) // for each controller
        {
            if (joys[i].IndexOf("Windows") != -1) // if it is an xbox controller
            {
                controller = Controllers.XBOX;
                break;
            }
            else if (joys[i] == "Wireless Controller") // if it is a PS4 controller
            {
                controller = Controllers.PS4;
            }
        }

        return controller;
    }
}
