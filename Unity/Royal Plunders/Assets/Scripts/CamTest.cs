using UnityEngine;
using System.Collections;

public class CamTest : MonoBehaviour
{
    CameraController camCont;
    bool readyToTurn = true;

	void Start ()
    {
        camCont = Camera.main.GetComponent<CameraController>();
        camCont.targetTransform = transform;
	}
	
	void Update ()
    {
        if (Input.GetButton("Left Shoulder") && readyToTurn)
        {
            readyToTurn = false;
            camCont.snapLeft();
        }
        if (Input.GetButton("Right Shoulder") && readyToTurn)
        {
            readyToTurn = false;
            camCont.snapRight();
        }

        if (!Input.GetButton("Left Shoulder") && !readyToTurn && !Input.GetButton("Right Shoulder"))
            readyToTurn = true;

        camCont.freeLookX = Input.GetAxis("Right Horizontal");
        camCont.freeLookY = Input.GetAxis("Right Vertical");
	}
}
