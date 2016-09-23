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

    public int GetSoundLevel()
    {
        return soundLevel;
    }
}
