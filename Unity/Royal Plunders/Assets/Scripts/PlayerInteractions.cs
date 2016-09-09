using UnityEngine;
using System.Collections;

public class PlayerInteractions : MonoBehaviour {

    public float interactionDistance;
    public bool interactionButtonPressed;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        interactionButtonPressed = Input.GetButton("Right Shoulder");

        // Keyboard in case controller doesn't work
        // if (Input.GetKeyDown(KeyCode.A))
        if (interactionButtonPressed)
        {
            RaycastHit rayHit = new RaycastHit();
            Ray ray = new Ray(transform.position, transform.forward);

            if (Physics.Raycast(ray, out rayHit) && rayHit.transform.tag == "Door")
            {
                if (this.gameObject.GetComponent<InventoryManager>()
                    && (rayHit.transform.gameObject.GetComponent<Door>().numKeyReq 
                        <= this.gameObject.GetComponent<InventoryManager>().GetNumKeys() ))
                {
                    rayHit.transform.gameObject.GetComponent<Door>().openCommand = true;

                    for (uint i = 0; i < rayHit.transform.gameObject.GetComponent<Door>().numKeyReq; ++i)
                    {
                        this.gameObject.GetComponent<InventoryManager>().LoseKey();
                        // TODO play animation or trigger hud here?
                    }

                    rayHit.transform.GetComponent<Door>().WipeKeyReq();
                }
            }

            if (this.GetComponent<MovementTest>())
            {
                if (this.GetComponent<MovementTest>().GetStealthWalkState())
                {
                    if (Physics.Raycast(ray, out rayHit) && rayHit.transform.tag == "Guard")
                    {
                        if (this.gameObject.GetComponent<InventoryManager>()
                            && rayHit.transform.GetComponent<InventoryManager>()
                            && rayHit.transform.GetComponent<InventoryManager>().GetNumKeys() > 0)
                        {
                            rayHit.transform.GetComponent<InventoryManager>().LoseKey();
                            this.gameObject.GetComponent<InventoryManager>().GainKey();
                        }
                    }
                }
            }
        }
	}

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Key")
        {
            Destroy(other.gameObject);
            if (this.gameObject.GetComponent<InventoryManager>())
            {
                this.gameObject.GetComponent<InventoryManager>().GainKey();
            }
        }
    }
}
