using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Interactor : MonoBehaviour
{
    // game objects to interact with
    // sorted as a priority list
    List<GameObject> listOfInteractables = new List<GameObject>();

    // unique type tag, set in the inspector for prefabs
    // None grants global permissions
    public string typeLabel = "None";

    // Called by a ConvexHull, telling the interactor what it can now interact with
    public void addInteractable(GameObject other)
    {
        // stop touching yourself
        if (gameObject == other)
            return;

        // if it is not in the list and is a valid target to interact with
        if (InteractionTable.canInteract(gameObject, other) && !listOfInteractables.Contains(other))
        {
            Iinteractable obj = other.GetComponent<Iinteractable>();
            if (obj.isInstant())
            {
                obj.interact(InteractionButton.NONE, gameObject);
                return;
            }
            else
            {
                GameManager.prompt.SetActive(true);
            }

            // add it to the list
            listOfInteractables.Insert(0, other);

            // if there is more than 1 item in the list
            if (listOfInteractables.Count > 1)
            {
                int i = 1; // sort starting at index 1, until the next item is equal or lower priority
                while (InteractionTable.priorityCheck(other, listOfInteractables[i]) < 0)
                {
                    // swap the items
                    GameObject temp = listOfInteractables[i];
                    listOfInteractables[i] = other;
                    listOfInteractables[i - 1] = temp;

                    // iterate forward, and break if there is no new item
                    if (++i >= listOfInteractables.Count)
                        break;
                }
            }
        }
        else if (InteractionTable.canInteract(gameObject, other))
        {
            Debug.Log("Problem with Listofinteractables contains");
        }
        else if (!listOfInteractables.Contains(other))
        {
            Debug.Log("Problem with can interact");
        }
    }

    // Called by a ConvexHull, telling the interactor that it can no longer interact with this item
    public void removeInteractable(GameObject other)
    {
        // only remove if it is in the list explicitly
        if (listOfInteractables.Contains(other))
            listOfInteractables.Remove(other);
    }

    // interact with the item at the top of the priority list
    public bool interact(InteractionButton button)
    {

        // if there is nothing to interact with, return false
        if (listOfInteractables.Count == 0)
            return false;

        // otherwise interact with it and return true
        listOfInteractables[0].GetComponent<Iinteractable>().interact(button, gameObject);
        return true;
    }
}