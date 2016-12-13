using UnityEngine;
using System.Collections;

public class NoiseMakerScript : MonoBehaviour {

    int soundLevel;

	// Use this for initialization
	void Start () {
        soundLevel = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    // adjust the sound level being produced
    public void AdjustSoundLevel(int sound)
    {
        soundLevel = sound;
    }

    // the player intentionally triggered a sound
    public void PlayerTriggeredSound()
    {
        GameObject[] obj = GameObject.FindGameObjectsWithTag("Guard"); // find all the guards
        foreach (GameObject g in obj) // for each guard
        {
            // if the guard can hear the noise (is in range)
            // and the guard is not currently stunned
            if (Vector3.Distance(transform.position, g.transform.position) < g.GetComponent<GuardAITest>().playerTriggerNoiseRange && g.GetComponent<GuardAITest>().myState != GuardAITest.AIState.Stunned)
            {
                g.GetComponent<GuardAITest>().myState = GuardAITest.AIState.Suspcious; // the guard becomes suspicious
                g.GetComponent<GuardAITest>().suspicionPoint = transform.position; // and investigates this current location
            }
        }
    }

    // public getter for the sound level
    public int GetSoundLevel()
    {
        return soundLevel;
    }
}
