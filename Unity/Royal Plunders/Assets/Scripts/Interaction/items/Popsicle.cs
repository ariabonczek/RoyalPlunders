using UnityEngine;
using System.Collections;

public class Popsicle : MonoBehaviour, Iinteractable
{

    public float Range;

    private float velocity;

    public float SpeedDecceleration;

    public float Duration;

    private float currentDuration;

    private bool active;

    // Use this for initialization
    void Start()
    {
        active = false;
    }

    public void Place(float speed)
    {
        active = true;
        velocity = speed;
    }


    public void interact(InteractionButton button, GameObject interactor)
    {
        if (!active && interactor.GetComponent<GadgetManager>())
        {
            if (interactor.GetComponent<GadgetManager>().AddToSlot(GadgetManager.GadgetSlotType.Popsicle, this.gameObject))
                transform.position -= new Vector3(0, -100, 0);
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            currentDuration += Time.deltaTime;
            if(currentDuration>= Duration)
            {
                active = false;
                transform.position -= new Vector3(0, -100, 0);
                return;
            } 
            velocity -= SpeedDecceleration;
            if (velocity < 0)
                velocity = 0;
            transform.position += transform.forward * velocity * Time.deltaTime;

            GameObject[] obj = GameObject.FindGameObjectsWithTag("Guard");
            foreach (GameObject g in obj)
            {
                if (Vector3.Distance(transform.position, g.transform.position) < Range)
                    g.GetComponent<GuardAITest>().Distract();
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
