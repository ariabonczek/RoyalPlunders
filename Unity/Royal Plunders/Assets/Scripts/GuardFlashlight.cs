using UnityEngine;
using System.Collections;

public class GuardFlashlight : MonoBehaviour {

    GuardAITest guardScript;
    Light myLight;

	// Use this for initialization
	void Start () {
        guardScript = transform.parent.GetComponent<GuardAITest>();
        myLight = GetComponent<Light>();

        myLight.spotAngle =guardScript.angleOfView * 1.6f;
        myLight.range = guardScript.PlayerSpotDistance;

        if(myLight.spotAngle>179)
        {
            myLight.spotAngle = 179;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
