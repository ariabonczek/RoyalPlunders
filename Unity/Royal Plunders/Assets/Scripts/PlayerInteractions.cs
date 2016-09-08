using UnityEngine;
using System.Collections;

public class PlayerInteractions : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Key")
        {
            Destroy(other.gameObject);
        }
    }
}
