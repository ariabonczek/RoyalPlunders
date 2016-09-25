using UnityEngine;
using System.Collections;

public class Key : MonoBehaviour, Iinteractable
{
    void Update()
    {
        transform.position += Vector3.up * Mathf.Sin(Time.time * 5) * 0.004f;
        transform.rotation *= Quaternion.AngleAxis(1, Vector3.up);
    }

    public void interact(GameObject interactor)
    {
        InventoryManager invManager = interactor.GetComponent<InventoryManager>();

        if (invManager != null) // a player, right?
        {
            invManager.GainKey();
            DestroyObject(gameObject);
        }
    }

    public string getTypeLabel()
    {
        return "Key";
    }

    public bool isInstant()
    {
        return true;
    }
}
