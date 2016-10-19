using UnityEngine;
using System.Collections;

public class HolderScript : MonoBehaviour {

    public int DisplacementOnDrop;

    private GameObject HeldObject;

    private bool holdingObject;

    private bool holdingButton;

    private bool clicked;

	// Use this for initialization
	void Start () {

	}

    // Update is called once per frame
    void Update() {
        if (Input.GetButtonDown("A Button"))
        {
            if (!holdingButton)
            {
                //Debug.Log("PRessed");
                holdingButton = true;

                clicked = true;
            }
            else
            {
                //Debug.Log("Held");
                clicked = false;
            }
        }
        else
        {
            holdingButton = false;
            clicked = false;
            //Debug.Log("NotPRessed");
        }

        if (clicked && HeldObject)
        {
            if (HeldObject.GetComponent<HoldableScript>().IsHeld())
            {
                HeldObject.GetComponent<HoldableScript>().EndHold(DisplacementOnDrop);
                holdingObject = false;
                HeldObject = null;
                //Debug.Log("DOWN");
                clicked = false;
            }
        }
    }

    void OnTriggerStay(Collider col)
    {
        if(clicked && col.gameObject.GetComponent<HoldableScript>())
        {
            if (!col.gameObject.GetComponent<HoldableScript>().IsHeld())
            {
                col.gameObject.GetComponent<HoldableScript>().BeginHold(this.gameObject);
                holdingObject = true;
                HeldObject = col.gameObject;
            }
        }
    }

    public bool IsHolding()
    {
        return holdingObject;
    }
}
