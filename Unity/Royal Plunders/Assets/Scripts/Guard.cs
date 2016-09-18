using UnityEngine;
using System.Collections;

public class Guard : MonoBehaviour, Iinteractable
{
    InventoryManager inventory;

    void Start()
    {
        inventory = gameObject.GetComponent<InventoryManager>();
    }

    public void interact(GameObject interactor)
    {
        InventoryManager invManager = interactor.GetComponent<InventoryManager>();

        if (invManager != null && inventory.GetNumKeys() > 0) // a player, right?
        {
            invManager.GainKey();
            inventory.LoseKey();
        }
    }

    public string getTypeLabel()
    {
        return "Guard";
    }

    public bool isInstant()
    {
        return false;
    }
}
