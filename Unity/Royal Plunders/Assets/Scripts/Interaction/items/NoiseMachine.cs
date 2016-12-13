using UnityEngine;
using System.Collections;

public class NoiseMachine : MonoBehaviour, Iinteractable
{
    public float Range;

    public float Duration;

    private float currentDuration;

    private bool active;

    private bool triggered;

    // Use this for initialization
    void Start()
    {
        active = false;
        triggered = false;
    }

    public void Place(float speed)
    {
        active = true;
    }


    public void interact(InteractionButton button, GameObject interactor)
    {
        if (!active && interactor.GetComponent<GadgetManager>())
        {
            if (interactor.GetComponent<GadgetManager>().AddToSlot(GadgetManager.GadgetSlotType.Noise, this.gameObject))
                transform.position -= new Vector3(0, -100, 0);
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (active && triggered)
        {
            currentDuration += Time.deltaTime;
            if (currentDuration >= Duration)
            {
                active = false;
                transform.position -= new Vector3(0, -100, 0);
                return;
            }

            GameObject[] obj = GameObject.FindGameObjectsWithTag("Guard");
            foreach (GameObject g in obj)
            {
                if(InRange(g))
                {
                    g.GetComponent<GuardAITest>().HearsSound(this.gameObject);
                }
            }
        }
    }

    public bool InRange(GameObject obj)
    {
        if (Vector3.Distance(obj.transform.position, transform.position) < Range && active)
            return true;
        else
            return false;
    }

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
