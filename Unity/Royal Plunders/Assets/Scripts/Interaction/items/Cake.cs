using UnityEngine;
using System.Collections;

public class Cake : MonoBehaviour, Iinteractable
{
    public float eatingTimer = 0;
    public float eatTime;
    public bool beingEaten;

    public bool StartEating()
    {
        bool bRetVal = false;

        if (!beingEaten)
        {
            beingEaten = true;
            bRetVal = true;
        }

        return bRetVal;
    }

    public void interact(InteractionButton button, GameObject interactor)
    {
        if(interactor.GetComponent<GadgetManager>())
        {
            if (interactor.GetComponent<GadgetManager>().AddToSlot(GadgetManager.GadgetSlotType.Cake, this.gameObject))
                transform.position -= new Vector3(0,-100,0);
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