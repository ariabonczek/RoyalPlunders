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
