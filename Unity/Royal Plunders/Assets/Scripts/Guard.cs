using UnityEngine;
using System.Collections;

public class Guard : MonoBehaviour {

    private bool inCH;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (inCH &&
            GameObject.FindWithTag("Player").GetComponent<PlayerInteractions>().interactionButtonPressed)
        {
            Interact();
        }
	}

    public void Interact()
    {
        if (this.GetComponent<InventoryManager>().LoseKey())
        {
            GameObject.FindWithTag("Player").GetComponent<InventoryManager>().GainKey();
        }
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
