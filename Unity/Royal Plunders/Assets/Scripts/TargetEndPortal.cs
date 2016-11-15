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
        if(col.gameObject.tag == "Player")
        {
            if(col.gameObject.GetComponent<Movement>().holding)
            {
                SceneManager.LoadScene(SceneManager.GetSceneAt(SceneManager.GetActiveScene().buildIndex +1).name);
            }
        }
    }
}
