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
                    && this.gameObject.GetComponent<InventoryManager>().GetNumKeys() > 0)
                {
                    rayHit.transform.gameObject.GetComponent<Door>().openCommand = true;
                    this.gameObject.GetComponent<InventoryManager>().UseKey();
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
