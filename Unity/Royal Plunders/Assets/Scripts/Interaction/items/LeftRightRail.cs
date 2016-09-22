using UnityEngine;
using System.Collections;

public class LeftRightRail : MonoBehaviour, Iinteractable
{
    public GameObject playerRef;
    public GameObject cameraRef;
    public GameObject endingOne;
    public GameObject endingTwo;

    void Start()
    {
        playerRef = GameObject.FindWithTag("Player");
    }

    public void interact(GameObject interactor)
    {
        if (playerRef)
        {
            playerRef.GetComponent<MovementTest>().sidewaysLocked = !playerRef.GetComponent<MovementTest>().sidewaysLocked;
        }

        if (endingOne && endingTwo)
        {
            endingOne.GetComponent<BoxCollider>().isTrigger = !endingOne.GetComponent<BoxCollider>().isTrigger;
            endingTwo.GetComponent<BoxCollider>().isTrigger = !endingTwo.GetComponent<BoxCollider>().isTrigger;
        }

        if (cameraRef)
        {
            cameraRef.GetComponent<CameraController>().immersiveLook = !cameraRef.GetComponent<CameraController>().immersiveLook;
        }
    }

    public string getTypeLabel()
    {
        return "Rail";
    }

    public bool isInstant()
    {
        return false;
    }
}