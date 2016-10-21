using UnityEngine;
using System.Collections;

public class Guard : MonoBehaviour, Iinteractable
{
    InventoryManager inventory;

    void Start()
    {
        inventory = gameObject.GetComponent<InventoryManager>();
    }

    public void interact(InteractionButton button, GameObject interactor)
    {
        if (button == InteractionButton.Y)
        {
            InventoryManager invManager = interactor.GetComponent<InventoryManager>();

            if (invManager != null && inventory.GetNumKeys() > 0)
            {
                invManager.GainKey();
                inventory.LoseKey();
            }
        }

        if (button == InteractionButton.B)
        {
            // sneak attack!
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
