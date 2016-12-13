using UnityEngine;
using System.Collections;

public class NoiseMachine : MonoBehaviour, Iinteractable
{
    public float Range; // the range for the trap

    public float Duration; // how long the trap is active for

    private float currentDuration; // how long the trap has been active for

    private bool active; // if the trap is active

    private bool triggered; // if the trap has been triggered

    // Use this for initialization
    void Start()
    {
        active = false;
        triggered = false;
    }

    // place the trap
    public void Place(float speed)
    {
        active = true; // sets the internal state to active
    }


    public void interact(InteractionButton button, GameObject interactor)
    {
        // if the trap has not been set and the interactor has a gadget manager
        if (!active && interactor.GetComponent<GadgetManager>())
        {
            // try to add the gadget to the interactor's gadget manager
            if (interactor.GetComponent<GadgetManager>().AddToSlot(GadgetManager.GadgetSlotType.Noise, this.gameObject))
                transform.position -= new Vector3(0, -100, 0); // move it underground if successful
        }
    }


    // Update is called once per frame
    void Update()
    {
        // if the trap is going off
        if (active && triggered)
        {
            currentDuration += Time.deltaTime; // update the time it has been going off
            if (currentDuration >= Duration) // check if the trap has run it's full lifetime
            {
                active = false; // de-activate the trap
                transform.position -= new Vector3(0, -100, 0); // place it under the world
                return;
            }

            GameObject[] obj = GameObject.FindGameObjectsWithTag("Guard"); // get all the guards
            foreach (GameObject g in obj) // for each guard
            {
                // check if the guard is within range
                if (InRange(g))
                    g.GetComponent<GuardAITest>().HearsSound(this.gameObject); // if so, distract the guard
            }
        }
    }

    // simple distance to bool function
    // returns true if the object is in range and the trap is active
    public bool InRange(GameObject obj)
    {
        if (Vector3.Distance(obj.transform.position, transform.position) < Range && active)
            return true;
        else
            return false;
    }

    // trigger the trap
    public void Trigger()
    {
        triggered = true;
    }

    public string getTypeLabel()
    {
        return "Popsicle";
    }

    public bool isInstant()
    {
        return false;
    }
}
