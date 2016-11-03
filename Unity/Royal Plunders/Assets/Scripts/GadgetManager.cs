using UnityEngine;
using System.Collections;

public class GadgetManager : MonoBehaviour {

    public enum GadgetSlotType { Cake, Jazz, Popsicle, Trigger, Empty};

    bool slot1Chosen;

    GadgetSlotType slot1;

    GameObject slot1Obj;

    GadgetSlotType slot2;

    GameObject slot2Obj;

	// Use this for initialization
	void Start () {
        slot1 = GadgetSlotType.Empty;
        slot2 = GadgetSlotType.Empty;
        slot1Chosen = true;
	}

    public bool AddToSlot(GadgetSlotType gadget, GameObject obj)
    {
        if(slot1 == GadgetSlotType.Empty)
        {
            slot1 = gadget;
            slot1Obj = obj;
            return true;
        }
        else if (slot2 == GadgetSlotType.Empty)
        {
            slot2 = gadget;
            slot2Obj = obj;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void UseGadget()
    {
        if(slot1Chosen && slot1 != GadgetSlotType.Empty)
        {
            if(slot1 == GadgetSlotType.Trigger)
            {
                if (slot1Obj.GetComponent<Jazz>())
                {
                    slot1Obj.GetComponent<Jazz>().Trigger();
                    slot1 = GadgetSlotType.Empty;
                    return;
                }
            }
            slot1 = GadgetSlotType.Empty;
            slot1Obj.transform.position = transform.GetChild(0).position + (transform.GetChild(0).forward * slot1Obj.transform.GetChild(0).localScale.z) + new Vector3(0,.4f,0);
            slot1Obj.transform.forward = transform.GetChild(0).forward;
            if (slot1Obj.GetComponent<Popsicle>())
            {
                slot1Obj.GetComponent<Popsicle>().Place(GetComponent<Movement>().currentSpeed);
            }
            if(slot1Obj.GetComponent<Jazz>())
            {
                slot1Obj.GetComponent<Jazz>().Place(GetComponent<Movement>().currentSpeed);
                slot1 = GadgetSlotType.Trigger;
            }
        }
        else if (slot2 != GadgetSlotType.Empty)
        {
            if (slot2 == GadgetSlotType.Trigger)
            {
                if (slot2Obj.GetComponent<Jazz>())
                {
                    slot2Obj.GetComponent<Jazz>().Trigger();
                    slot2 = GadgetSlotType.Empty;
                    return;
                }
            }
            slot2 = GadgetSlotType.Empty;
            slot2Obj.transform.position = transform.GetChild(0).position + (transform.GetChild(0).forward * slot1Obj.transform.GetChild(0).localScale.z) + new Vector3(0, .4f, 0);
            slot2Obj.transform.forward = transform.GetChild(0).forward;
            if (slot2Obj.GetComponent<Popsicle>())
            {
                slot2Obj.GetComponent<Popsicle>().Place(GetComponent<Movement>().currentSpeed);
            }
            if (slot2Obj.GetComponent<Jazz>())
            {
                slot2Obj.GetComponent<Jazz>().Place(GetComponent<Movement>().currentSpeed);
                slot2 = GadgetSlotType.Trigger;
            }
        }
    }

    // Update is called once per frame
    void Update () {
	
	}
}
