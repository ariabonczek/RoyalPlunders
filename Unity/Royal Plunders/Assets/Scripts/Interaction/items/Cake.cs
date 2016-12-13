using UnityEngine;
using System.Collections;

public class Cake : MonoBehaviour, Iinteractable
{
    public float eatingTimer = 0; // unused timer for eating
    public float eatTime; // unused eating time range
    public bool beingEaten; // if the cake is being eaten

    // start the eating process of the cake
    // returns true if the cake was not being eaten before
    public bool StartEating()
    {
        bool bRetVal = false;

        if (!beingEaten) // if the cake is not being eaten
        {
            beingEaten = true; // make it be eaten
            bRetVal = true;
        }

        return bRetVal;
    }

    public void interact(InteractionButton button, GameObject interactor)
    {
        if(interactor.GetComponent<GadgetManager>()) // if the interactor has a gadget manager
        {
            // if the player successfully added the cake to the gadget manager
            if (interactor.GetComponent<GadgetManager>().AddToSlot(GadgetManager.GadgetSlotType.Cake, this.gameObject))
                transform.position -= new Vector3(0,-100,0); // move it under the ground
        }
    }

    public void EatCake()
    {
    }

    public string getTypeLabel()
    {
        return "Cake";
    }

    public bool isInstant()
    {
        return false;
    }
}