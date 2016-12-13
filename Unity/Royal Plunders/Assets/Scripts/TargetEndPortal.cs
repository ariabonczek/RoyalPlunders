using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TargetEndPortal : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.GetComponent<Movement>()) // if the object can move in our system
        {
            if(col.gameObject.GetComponent<Movement>().holding) // and is holding something
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // then the player captured the target, next level!
            }
        }
    }
}
