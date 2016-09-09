using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {

    private Quaternion startingRotation;

    public float rotation; // negative for left, positive for right
    public float rotationSpeed;

    public bool openCommand;

	// Use this for initialization
	void Start () 
    {
        startingRotation = transform.rotation;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (openCommand)
        {
            Open();
        }
        else
        {
            Close();
        }
	}

    public void Open()
    {
        Quaternion quat = Quaternion.Euler(0, rotation, 0);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, quat, Time.deltaTime * rotationSpeed);
    }

    public void Close()
    {
        openCommand = false;
        transform.localRotation = Quaternion.Lerp(transform.localRotation, startingRotation, Time.deltaTime * rotationSpeed);
    }
}
