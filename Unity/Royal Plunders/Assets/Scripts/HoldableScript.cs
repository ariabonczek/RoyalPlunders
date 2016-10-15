using UnityEngine;
using System.Collections;

public class HoldableScript : MonoBehaviour {

    public float YOffsetFromOwner;

    public float originalY;

    private bool held;

    GameObject owner;

	// Use this for initialization
	void Start () {
        held = false;
	}
	
	// Update is called once per frame
	void Update () {
        if(held)
        {
            this.transform.position = new Vector3(owner.transform.position.x, owner.transform.position.y + YOffsetFromOwner, owner.transform.position.z);
            GetComponent<Collider>().enabled = false;
        }
    }

    // this function initiates the hold by changing the held variable and disabling movement if this object moves
    public void BeginHold(GameObject newOwner)
    {
        originalY = transform.position.y;
        held = true;
        owner = newOwner;

        Movement mover = gameObject.GetComponent<Movement>();
        if (mover)
            mover.enabled = false;
    }

    // this function restores movement, returns the held boolean to false and places the item in front of the player
    public void EndHold(float displacement)
    {
        held = false;

        Movement mover = gameObject.GetComponent<Movement>();
        if (mover)
            mover.enabled = true;

        this.transform.position = owner.transform.position + owner.transform.GetChild(0).forward * displacement;
        transform.position.Set(transform.position.x, originalY, transform.position.z);
        GetComponent<Collider>().enabled = true;
    }

     public bool IsHeld()
    {
        return held;
    }
}
