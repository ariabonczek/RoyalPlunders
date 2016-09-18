using UnityEngine;
using System.Collections;

public class Key : MonoBehaviour, Iinteractable
{
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
