using UnityEngine;
using System.Collections;

public class PlayerInteraction : MonoBehaviour {

    public float captureTime;
    public Transform respawnPoint;

    private bool vulnerable;

    void Start()
    {
        captureTime = 0;
        vulnerable = false;
    }

    public void interact(GameObject interactor)
    {
        if (!vulnerable)
        {
            vulnerable = true;
            captureTime = Time.time + captureTime;
        }
        else
        {
            if (Time.time >= captureTime)
            {
                // TODO call HUD interactionss
                transform.position = respawnPoint.position;
            }
        }
    }

    public string getTypeLabel()
    {
        return "Player";
    }

    public bool isInstant()
    {
        return false;
    }
}
