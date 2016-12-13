using UnityEngine;
using System.Collections;

public class GadgetManager : MonoBehaviour {

    public enum GadgetSlotType { Cake, Jazz, Popsicle, Noise, Trigger, Empty};

    bool slot1Chosen;

    GadgetSlotType slot1;

    GameObject slot1Obj;

    GadgetSlotType slot2;

    GameObject slot2Obj;

    GameObject popsicle;

    GameObject cake;

    GameObject noiseMaker;

    GameObject jazzMaker;

    GameObject triggerSign;

	// Use this for initialization
	void Start () {
        slot1 = GadgetSlotType.Empty;
        slot2 = GadgetSlotType.Empty;
        slot1Chosen = true;
        popsicle = transform.GetChild(2).gameObject;
        cake = transform.GetChild(3).gameObject;
        noiseMaker = transform.GetChild(4).gameObject;
        jazzMaker = transform.GetChild(5).gameObject;
        triggerSign = transform.GetChild(6).gameObject;
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
        if (slot1Chosen && slot1 != GadgetSlotType.Empty && slot1Obj)
        {
            if(slot1 == GadgetSlotType.Trigger)
            {
                if (slot1Obj.GetComponent<Jazz>())
                {
                    slot1Obj.GetComponent<Jazz>().Trigger();
                    slot1 = GadgetSlotType.Empty;
                    return;
                }
                if (slot1Obj.GetComponent<NoiseMachine>())
                {
                    slot1Obj.GetComponent<NoiseMachine>().Trigger();
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
            if (slot1Obj.GetComponent<NoiseMachine>())
            {
                slot1Obj.GetComponent<NoiseMachine>().Place(GetComponent<Movement>().currentSpeed);
                slot1 = GadgetSlotType.Trigger;
            }
        }
        else if (slot2 != GadgetSlotType.Empty && slot2Obj)
        {
            if (slot2 == GadgetSlotType.Trigger)
            {
                if (slot2Obj.GetComponent<Jazz>())
                {
                    slot2Obj.GetComponent<Jazz>().Trigger();
                    slot2 = GadgetSlotType.Empty;
                    return;
                }
                if (slot2Obj.GetComponent<NoiseMachine>())
                {
                    slot2Obj.GetComponent<NoiseMachine>().Trigger();
                    slot2 = GadgetSlotType.Empty;
                    return;
                }
            }
            slot2 = GadgetSlotType.Empty;
            slot2Obj.transform.position = transform.GetChild(0).position + (transform.GetChild(0).forward * slot2Obj.transform.GetChild(0).localScale.z) + new Vector3(0, .4f, 0);
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
            if (slot2Obj.GetComponent<NoiseMachine>())
            {
                slot2Obj.GetComponent<NoiseMachine>().Place(GetComponent<Movement>().currentSpeed);
                slot2 = GadgetSlotType.Trigger;
            }
        }

        if (slot1Chosen && slot1 == GadgetSlotType.Empty && slot2 != GadgetSlotType.Empty)
        {
            slot1Chosen = false;
        }
        if (!slot1Chosen && slot1 != GadgetSlotType.Empty && slot2 == GadgetSlotType.Empty)
        {
            slot1Chosen = true;
        }
    }

    public GadgetSlotType GetPrimaryGadget()
    {
        if (slot1Chosen)
            return slot1;
        else
            return slot2;
    }

    public GadgetSlotType GetSecondaryGadget()
    {
        if (slot1Chosen)
            return slot2;
        else
            return slot1;
    }

    void HandleActiveGadgetShowcase()
    {
        if(slot1Chosen)
        {
            if (slot1 != GadgetSlotType.Empty)
            {
                switch (slot1)
                {
                    case GadgetSlotType.Cake:
                        cake.transform.position = transform.position + new Vector3(0, 2, 0);
                        noiseMaker.transform.position = transform.position + new Vector3(0, -100, 0);
                        jazzMaker.transform.position = transform.position + new Vector3(0, -100, 0);
                        popsicle.transform.position = transform.position + new Vector3(0, -100, 0);
                        triggerSign.transform.position = transform.position + new Vector3(0, -100, 0);
                        break;

                    case GadgetSlotType.Popsicle:
                        cake.transform.position = transform.position + new Vector3(0, -100, 0);
                        noiseMaker.transform.position = transform.position + new Vector3(0, -100, 0);
                        jazzMaker.transform.position = transform.position + new Vector3(0, -100, 0);
                        popsicle.transform.position = transform.position + new Vector3(0, 2, 0);
                        triggerSign.transform.position = transform.position + new Vector3(0, -100, 0);
                        break;

                    case GadgetSlotType.Jazz:
                        cake.transform.position = transform.position + new Vector3(0, -100, 0);
                        noiseMaker.transform.position = transform.position + new Vector3(0, -100, 0);
                        jazzMaker.transform.position = transform.position + new Vector3(0, 2, 0);
                        popsicle.transform.position = transform.position + new Vector3(0, -100, 0);
                        triggerSign.transform.position = transform.position + new Vector3(0, -100, 0);
                        break;

                    case GadgetSlotType.Noise:
                        cake.transform.position = transform.position + new Vector3(0, -100, 0);
                        noiseMaker.transform.position = transform.position + new Vector3(0, 2, 0);
                        jazzMaker.transform.position = transform.position + new Vector3(0, -100, 0);
                        popsicle.transform.position = transform.position + new Vector3(0, -100, 0);
                        triggerSign.transform.position = transform.position + new Vector3(0, -100, 0);
                        break;

                    case GadgetSlotType.Trigger:
                        cake.transform.position = transform.position + new Vector3(0, -100, 0);
                        noiseMaker.transform.position = transform.position + new Vector3(0, -100, 0);
                        jazzMaker.transform.position = transform.position + new Vector3(0, -100, 0);
                        popsicle.transform.position = transform.position + new Vector3(0, -100, 0);
                        triggerSign.transform.position = transform.position + new Vector3(0, 3.5f, 0);
                        break;

                    default:
                        break;
                }
            }
            else
            {
                cake.transform.position = transform.position + new Vector3(0, -100, 0);
                noiseMaker.transform.position = transform.position + new Vector3(0, -100, 0);
                jazzMaker.transform.position = transform.position + new Vector3(0, -100, 0);
                popsicle.transform.position = transform.position + new Vector3(0, -100, 0);
                triggerSign.transform.position = transform.position + new Vector3(0, -100, 0);
            }
        }
        else
        {
            if (slot2 != GadgetSlotType.Empty)
            {
                switch (slot2)
                {
                    case GadgetSlotType.Cake:
                        cake.transform.position = transform.position + new Vector3(0, 2, 0);
                        noiseMaker.transform.position = transform.position + new Vector3(0, -100, 0);
                        jazzMaker.transform.position = transform.position + new Vector3(0, -100, 0);
                        popsicle.transform.position = transform.position + new Vector3(0, -100, 0);
                        triggerSign.transform.position = transform.position + new Vector3(0, -100, 0);
                        break;

                    case GadgetSlotType.Popsicle:
                        cake.transform.position = transform.position + new Vector3(0, -100, 0);
                        noiseMaker.transform.position = transform.position + new Vector3(0, -100, 0);
                        jazzMaker.transform.position = transform.position + new Vector3(0, -100, 0);
                        popsicle.transform.position = transform.position + new Vector3(0, 2, 0);
                        triggerSign.transform.position = transform.position + new Vector3(0, -100, 0);
                        break;

                    case GadgetSlotType.Jazz:
                        cake.transform.position = transform.position + new Vector3(0, -100, 0);
                        noiseMaker.transform.position = transform.position + new Vector3(0, -100, 0);
                        jazzMaker.transform.position = transform.position + new Vector3(0, 2, 0);
                        popsicle.transform.position = transform.position + new Vector3(0, -100, 0);
                        triggerSign.transform.position = transform.position + new Vector3(0, -100, 0);
                        break;

                    case GadgetSlotType.Noise:
                        cake.transform.position = transform.position + new Vector3(0, -100, 0);
                        noiseMaker.transform.position = transform.position + new Vector3(0, 2, 0);
                        jazzMaker.transform.position = transform.position + new Vector3(0, -100, 0);
                        popsicle.transform.position = transform.position + new Vector3(0, -100, 0);
                        triggerSign.transform.position = transform.position + new Vector3(0, -100, 0);
                        break;

                    case GadgetSlotType.Trigger:
                        cake.transform.position = transform.position + new Vector3(0, -100, 0);
                        noiseMaker.transform.position = transform.position + new Vector3(0, -100, 0);
                        jazzMaker.transform.position = transform.position + new Vector3(0, -100, 0);
                        popsicle.transform.position = transform.position + new Vector3(0, -100, 0);
                        triggerSign.transform.position = transform.position + new Vector3(0, 3.5f, 0);
                        break;

                    default:
                        break;
                }
            }
            else
            {
                cake.transform.position = transform.position + new Vector3(0, -100, 0);
                noiseMaker.transform.position = transform.position + new Vector3(0, -100, 0);
                jazzMaker.transform.position = transform.position + new Vector3(0, -100, 0);
                popsicle.transform.position = transform.position + new Vector3(0, -100, 0);
                triggerSign.transform.position = transform.position + new Vector3(0, -100, 0);
            }
        }
    }

    // Update is called once per frame
    void Update () {
        HandleActiveGadgetShowcase();
	}

    public void Reset()
    {
        slot1 = GadgetSlotType.Empty;
        slot2 = GadgetSlotType.Empty;
        slot1Obj = null;
        slot2Obj = null;
    }
}
