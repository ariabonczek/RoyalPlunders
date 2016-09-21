using UnityEngine;
using System.Collections;

public class SetJailPoint : MonoBehaviour 
{
    public Transform respawnPosition;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (other.GetComponent<PlayerInteraction>())
            {
                other.GetComponent<PlayerInteraction>().respawnPoint = respawnPosition;
            }
        }
    }
}
