using UnityEngine;
using System.Collections;

public class Rail : MonoBehaviour, Iinteractable
{
    public GameObject playerRef;

    void Start()
    {
        playerRef = GameObject.FindWithTag("Player");
    }

    public void interact(GameObject interactor)
    {
        if (playerRef)
        {
            playerRef.transform.position = transform.position;
        }
        Debug.Log("Interacted!");
    }

    public string getTypeLabel()
    {
        return "Rail";
    }

    public bool isInstant()
    {
        return false;
    }
}
