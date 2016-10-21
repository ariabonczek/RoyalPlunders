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
        if (!beingEaten)
        {
            if (StartEating())
            {
                EatCake();
            }
        }
    }

    public void EatCake()
    {
        while ( eatingTimer < eatTime )
        {
            // send guard ai data to change state
            eatingTimer++;
        }
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