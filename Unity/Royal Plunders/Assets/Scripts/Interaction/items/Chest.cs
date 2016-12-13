using UnityEngine;
using System.Collections;
using System;

public class Chest : MonoBehaviour, Iinteractable
{
    public GameObject itemPrefab; // the item to spawn
    bool isOpen = false; // if the chest is open
    public int numberOfItems; // how many items are in the chest
    private int originalNumItems; // how many items belong in the chest

    void Start()
    {
        originalNumItems = numberOfItems; // set how many items belong
    }

    public string getTypeLabel()
    {
        return "Chest";
    }

    public void interact(InteractionButton button, GameObject interactor)
    {
        if (button != InteractionButton.Y) // interact button
            return;

        // if the chest is not open and there is an item set for spawning
        if (!isOpen && itemPrefab)
        {
            Vector3 pos = transform.position + transform.forward * transform.localScale.z; // position in fron of the chest
            pos += transform.forward * itemPrefab.transform.localScale.z; // move said position ahead enough to fit the spawned item
            
            GameObject.Instantiate(itemPrefab, pos, Quaternion.LookRotation(transform.forward, Vector3.up)); // spawn the item
            numberOfItems--; // decriment the items held within
            if(numberOfItems<=0) // if the chest has run out of items
                isOpen = true; // it is now considered to be opened
        }
    }

    // reset the chest for spawning more items
    public void Reset()
    {
        isOpen = false; // close the chest
        numberOfItems = originalNumItems; // reset the items held within
    }

    public bool isInstant()
    {
        return false;
    }
}
