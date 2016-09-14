using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour, IInteraction {

    private Quaternion startingRotation;

    public float rotation; // negative for left, positive for right
    public float rotationSpeed;

    public int numKeyReq;

    public bool isOpen;
    private bool inCH;

	// Use this for initialization
	void Start () 
    {
        startingRotation = transform.rotation;
        isOpen = false;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (GameObject.FindWithTag("Player").GetComponent<PlayerInteractions>().interactionButtonPressed && inCH)
        {
            Interact();
        }
	}

    public void Open()
    {
        Quaternion quat = Quaternion.Euler(0, rotation, 0);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, quat, Time.deltaTime * rotationSpeed);
    }

    public void Close()
    {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, startingRotation, Time.deltaTime * rotationSpeed);
    }

    public void WipeKeyReq()
    {
        numKeyReq = 0;
    }

    public void Interact()
    {
        Open();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            inCH = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            inCH = false;
        }
    }
}