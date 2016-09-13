using UnityEngine;
using System.Collections;

public class HolderScript : MonoBehaviour {

    public int DisplacementOnDrop;

    private GameObject HeldObject;

    private bool holdingButton;

    private bool clicked;

	// Use this for initialization
	void Start () {

	}

    // Update is called once per frame
    void Update() {
        if (Input.GetButton("B Button"))
        {
            if (!holdingButton)
            {
                holdingButton = true;

                clicked = true;
            }
            else
            {
                clicked = false;
            }
        }
        else if (!Input.GetButton("B Button"))
        {
            holdingButton = false;
            clicked = false;
        }
    }

    void OnTriggerStay(Collider col)
    {
        if(clicked && col.gameObject.GetComponent<HoldableScript>())
        {
            if(!col.gameObject.GetComponent<HoldableScript>().IsHeld())
                col.gameObject.GetComponent<HoldableScript>().BeginHold(this.gameObject);
            else
                col.gameObject.GetComponent<HoldableScript>().EndHold(DisplacementOnDrop);
        }
    }
}
