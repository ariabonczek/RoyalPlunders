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

    public void AdjustSoundLevel(int sound)
    {
        soundLevel = sound;
    }

    public void PlayerTriggeredSound()
    {
        GameObject[] obj = GameObject.FindGameObjectsWithTag("Guard");
        foreach (GameObject g in obj)
        {
            if (Vector3.Distance(transform.position, g.transform.position) < g.GetComponent<GuardAITest>().playerTriggerNoiseRange)
            {
                g.GetComponent<GuardAITest>().myState = GuardAITest.AIState.Suspcious;
                g.GetComponent<GuardAITest>().suspicionPoint = transform.position;
            }
        }
    }

    public int GetSoundLevel()
    {
        return soundLevel;
    }
}
