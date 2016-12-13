using UnityEngine;
using System.Collections;

public class Key : MonoBehaviour, Iinteractable
{
    void Update()
    {
        // hover effects
        transform.position += Vector3.up * Mathf.Sin(Time.time * 5) * 0.004f;
        transform.rotation *= Quaternion.AngleAxis(1, Vector3.up);
    }

    public void interact(InteractionButton button, GameObject interactor)
    {
        InventoryManager invManager = interactor.GetComponent<InventoryManager>();

        if (invManager != null) // if they have an inventory
        {
            invManager.GainKey(); // give it a key
            DestroyObject(gameObject); // and destroy yourself
        }
    }

    public string getTypeLabel()
    {
        return "Key";
    }

    public bool isInstant()
    {
        return true; // this is a passive pickup
    }
}
