using UnityEngine;
using System.Collections;

public class InteractPrompt : MonoBehaviour {

    Vector3 screenPos;
    GameObject player;

	// Use this for initialization
	void Update () {
        player = GameObject.Find("Player");

        if (player)
        {
            Vector3 pos = player.transform.position;
            screenPos = Camera.main.WorldToScreenPoint(pos);
        }
	}
}
