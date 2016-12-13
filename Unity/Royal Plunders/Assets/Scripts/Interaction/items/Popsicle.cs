using UnityEngine;
using System.Collections;

public class Popsicle : MonoBehaviour, Iinteractable
{

    public float FreezeRange; // the range for the trap

    public float TriggerRange; // the trigger range for the trap

    private float velocity; // the speed at which the trap slides

    public float SpeedDecceleration; // the speed at which the speed un-speeds

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
        active = true;
        velocity = speed;
    }


    public void interact(InteractionButton button, GameObject interactor)
    {
        // if the trap has not been set and the interactor has a gadget manager
        if (!active && interactor.GetComponent<GadgetManager>())
        {
            // try to add the gadget to the interactor's gadget manager
            if (interactor.GetComponent<GadgetManager>().AddToSlot(GadgetManager.GadgetSlotType.Popsicle, this.gameObject))
                transform.position -= new Vector3(0, -100, 0); // move it underground if successful
        }
    }

    // check each guard to see if it triggers the popsicle
    private void CheckForTrigger()
    {
        GameObject[] obj = GameObject.FindGameObjectsWithTag("Guard"); // get all the guards
        foreach (GameObject g in obj) // for each guard
        {
            // check if the guard is within range
            if (Vector3.Distance(transform.position, g.transform.position) < TriggerRange)
                triggered = true; // if so, trigger the trap
        }
    }


    // Update is called once per frame
    void Update()
    {
        // if the trap is active
        if (active)
        {
            CheckForTrigger(); // try to trigger from the guards

            // if the trap is triggered
            if (triggered)
            {
                currentDuration += Time.deltaTime; // update the time it has been going off
                if (currentDuration >= Duration) // check if the trap has run it's full lifetime
                {
                    active = false; // de-activate the trap
                    transform.position -= new Vector3(0, -100, 0); // place it under the world
                    return;
                }

                velocity -= SpeedDecceleration; // un-speed the speed
                if (velocity < 0) // if too un-speeded
                    velocity = 0; // be un-speed
                transform.position += transform.forward * velocity * Time.deltaTime; // slide

                GameObject[] obj = GameObject.FindGameObjectsWithTag("Guard"); // get all the guards
                foreach (GameObject g in obj) // for each guard
                {
                    // check if the guard is within range
                    if (Vector3.Distance(transform.position, g.transform.position) < FreezeRange)
                        g.GetComponent<GuardAITest>().Distract(); // if so, distract the guard
                }
            }
        }
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
