using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class KeyText : MonoBehaviour {

    public GameObject player;

	// Update is called once per frame
	void Update () 
    {
        InventoryManager invManager = player.GetComponent<InventoryManager>();
        Text text = gameObject.GetComponent<Text>();

        text.text = invManager.GetNumKeys().ToString();
	}
}
