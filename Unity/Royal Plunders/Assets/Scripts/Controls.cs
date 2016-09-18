using UnityEngine;
using System.Collections;

public class Controls : MonoBehaviour
{
    CameraController camCont;
    Interactor actor;
    bool readyToTurn = true;
    bool readyToInteract = true;

	void Start ()
    {
        camCont = Camera.main.GetComponent<CameraController>();
        actor = gameObject.GetComponent<Interactor>();
        camCont.targetTransform = transform;
	}
	
	void Update ()
    {
        if (Input.GetButton("Left Shoulder") && readyToTurn)
        {
            readyToTurn = false;
            camCont.angleSnapIndex--;
        }
        if (Input.GetButton("Right Shoulder") && readyToTurn)
        {
            readyToTurn = false;
            camCont.angleSnapIndex++;
        }

        if (!Input.GetButton("Left Shoulder") && !readyToTurn && !Input.GetButton("Right Shoulder"))
            readyToTurn = true;

        if (Input.GetButton("Fire1") && readyToInteract)
        {
            readyToInteract = false;
            actor.interact();
        }
        if (!Input.GetButton("Fire1") && !readyToInteract)
            readyToInteract = true;

        camCont.freeLookX = Input.GetAxis("Right Horizontal");
        camCont.freeLookY = Input.GetAxis("Right Vertical");
	}
}
