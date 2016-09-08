using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {

    public GameObject keyReq;

    public float rotation; // negative for left, positive for right
    public float rotationSpeed;

    public bool opened;
    public bool keyReqMet;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (keyReq == null)
        {
            keyReqMet = true;
            Open();
        }
	}

    public void Open()
    {
        Quaternion quat = Quaternion.Euler(0, rotation, 0);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, quat, Time.deltaTime * rotationSpeed );
    }
}
